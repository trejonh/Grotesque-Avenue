using Horse.Engine.Utils;
using Horse.Server.Screens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Horse.Server.Helpers;

namespace Horse.Server.Core
{
    /// <summary>
    /// Master socket manager
    /// </summary>
    public class ServerSocketManagerMaster
    {
        /// <summary>
        /// Listen for incoming connections
        /// </summary>
        private static bool _listen;


        /// <summary>
        /// The listener for tcp connections
        /// </summary>
        private static TcpListener _listener;

        /// <summary>
        /// The connected mobile players
        /// </summary>
        public static List<NetworkMobilePlayer> Players
        {
            get
            {
                lock (_mobilePlayers)
                {
                    return _mobilePlayers;
                }
            }
        }

        /// <summary>
        /// Handler for when a client disconnects
        /// </summary>
        public event EventHandler PlayerDisconnected;

        /// <summary>
        /// Thread to check for incoming messages and if a client disconnects
        /// </summary>
        private static Thread _checkConnectionThread;

        /// <summary>
        /// Salt used for hashing the device id
        /// </summary>
        private static byte[] _salt;

        private static bool _stopped;

        public static bool IsGameThreadControllingInput;

        private static List<NetworkMobilePlayer> _mobilePlayers;


        /// <summary>
        /// Initiate the connection checking thread
        /// </summary>
        public ServerSocketManagerMaster()
        {
            new RNGCryptoServiceProvider().GetBytes(_salt = new byte[16]);
            _checkConnectionThread = new Thread(PollMobileClients)
                {Priority = ThreadPriority.Lowest, IsBackground = true, Name = "Client Connection Checker"};
            _checkConnectionThread.Start();
        }

        /// <summary>
        /// Iterate through connected clients checking for
        /// closed connections or incoming messages
        /// </summary>
        private async void PollMobileClients()
        {
            try
            {
                var sb = new StringBuilder();
                while (ServerGameWindowMaster.GameWindow.IsOpen)
                {
                    if (IsGameThreadControllingInput)
                        continue;
                    if (_mobilePlayers == null || _mobilePlayers.Count == 0)
                        continue;
                    NetworkMobilePlayer[] tempArr;
                    lock (_mobilePlayers)
                    {
                        tempArr = _mobilePlayers.ToArray();
                    }
                    foreach (var player in tempArr)
                    {
                        if (player == null)
                        {
                            continue;
                        }
                        if (player.Client == null)
                        {
                            lock (_mobilePlayers)
                            {
                                _mobilePlayers.Remove(player);
                            }
                            OnPlayerDisconnected(new EventArgs());
                            continue;
                        }
                        if (player.Client.Connected == false)
                        {
                            player.Client.Close();
                            lock (_mobilePlayers)
                            {
                                _mobilePlayers.Remove(player);
                            }
                            OnPlayerDisconnected(new EventArgs());
                        }
                        if (player.Client.Available <= 0) continue;
                        var clientStream = player.Client.GetStream();
                        while (clientStream.DataAvailable)
                        {
                            var bytes = new byte[player.Client.ReceiveBufferSize];

                            // Read can return anything from 0 to numBytesToRead. 
                            // This method blocks until at least one byte is read.
                            await clientStream.ReadAsync(bytes, 0, player.Client.ReceiveBufferSize);
                            var str = Encoding.UTF8.GetString(bytes);
                            sb.Append(str);
                            if (sb.ToString().Contains("ENDTRANS"))
                                break;
                        }
                        LogManager.Log(player.Name + " " + player.DeviceId + " sent:" + sb);
                        sb.Replace("$", "").Replace("\0", "").Replace("\u001d", "");
                        var messages = StringHelper.ReplaceAndToArray(sb.ToString(), "ENDTRANS");
                        foreach (var message in messages)
                            ProcessMessage(player.Client, message);
                        sb.Clear();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                LogManager.LogError("Aborting connection checking thread");
            }
        }

        /// <summary>
        /// Proccess the incoming message
        /// </summary>
        /// <param name="client">The client that sent the message</param>
        /// <param name="message">The sent message</param>
        private void ProcessMessage(TcpClient client, string message)
        {
            if (message.Contains(MessageType.Cmd))
            {
                var cmd = message.Substring(message.IndexOf(MessageType.Cmd, StringComparison.Ordinal) + 4)
                                 .Trim()
                                 .ToLower();
                if (cmd.Contains("playgame:"))
                {
                    GameSelectionScreen.PlayGame(cmd.Substring(cmd.IndexOf(":", StringComparison.Ordinal)+1));
                }
                else
                {
                    switch (cmd)
                    {
                        case "getplayerlist":
                            SendPlayerList(client);
                            break;
                        case "startgame":
                            StartGame(client);
                            break;
                        default:
                            LogManager.LogWarning("Command: " + cmd + " not found");
                            break;
                    }
                }
            }
            else if (message.Contains(MessageType.Data))
            {

            }
            else
            {
                LogManager.Log("Message from client: " + message.Replace(MessageType.Info, ""));
            }
        }

        private void StartGame(TcpClient client)
        {
            if (_mobilePlayers.Count < 2)
            {
                SendMessage(MessageType.Err + " Not enough players", client.GetStream());
                LogManager.LogWarning("Not enough players to start");
                return;
            }
            StopListening();
            ServerGameWindowMaster.ChangeScreen(GameSelectionScreen.GetInstance());
        }

        /// <summary>
        /// Send the list of currently connected players to the client
        /// </summary>
        /// <param name="client">The client to send the list to</param>
        private static void SendPlayerList(TcpClient client)
        {
            try
            {
                var sb = new StringBuilder(MessageType.Info + " playerList[");
                foreach (var player in _mobilePlayers)
                {
                    sb.Append("player: ");
                    sb.Append(player.Name);
                    sb.Append(",");
                    sb.Append(player.DeviceId);
                    sb.Append(player.IsVip ? ",true" : ",false");
                    sb.Append(player.IsNext ? ",true" : ",false");
                    sb.Append(player.IsCurrentlyPlaying ? ",true" : ",false");
                }
                sb.Append(" ]");
                SendMessage(sb.ToString(), client.GetStream());
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// List for incoming connections
        /// </summary>
        public static void Listen()
        {
            if (_listen || _stopped)
                return;
            if (_mobilePlayers != null && _mobilePlayers.Count > 0)
                CloseExistingConnections();
            _mobilePlayers = new List<NetworkMobilePlayer>();
            if (_listener != null)
            {
                _listener.Server.Close();
                _listener.Stop();
            }
            _listener = new TcpListener(IPAddress.Any, 54000);
            _listener.Start();
            _listen = true;
            Console.WriteLine(@"listening");
            StartAccept();
        }

        /// <summary>
        /// Handler for when a player disconnects
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPlayerDisconnected(EventArgs e)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(this, e);
        }

        /// <summary>
        /// Close all existing connections
        /// </summary>
        private static void CloseExistingConnections()
        {
            if (_mobilePlayers == null)
                return;
            foreach (var player in _mobilePlayers)
            {
                if (player?.Client != null && player.Client.Connected)
                    player.Client.Close();
            }
        }

        /// <summary>
        /// Stop listening for incoming connections
        /// </summary>
        public static void StopListening()
        {
            _listen = false;
            _stopped = true;
            try
            {
                _listener?.Stop();
            }
            catch (SocketException ex)
            {
                LogManager.LogError("Listener stop exception. error code: " + ex.ErrorCode);
            }
        }

        /// <summary>
        /// Close all connections and stop checking for messages
        /// </summary>
        public static void CloseAllConnections()
        {
            CloseExistingConnections();
            StopListening();
            _checkConnectionThread?.Abort();
        }

        /// <summary>
        /// Start accepting connections
        /// </summary>
        private static void StartAccept()
        {
            if (_stopped)
                return;
            if (_listen == false)
                _listener.Start();
            _listener.BeginAcceptTcpClient(HandleAsyncConnection, _listener);
        }

        /// <summary>
        /// Handle the connetion
        /// </summary>
        /// <param name="res"></param>
        private static void HandleAsyncConnection(IAsyncResult res)
        {
            if (_listen == false && _mobilePlayers.Count >= 8)
                return;
            StartAccept(); //listen for new connections again
            TcpClient client;
            try
            {
                client = _listener.EndAcceptTcpClient(res);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.Message + "<br/>" + ex.StackTrace);
                return;
            }
            Console.WriteLine(@"connected");
            var clientStream = client.GetStream();
            var sb = new StringBuilder();
            var continueToRead = true;
            while (continueToRead)
            {
                var bytes = new byte[client.ReceiveBufferSize];

                // Read can return anything from 0 to numBytesToRead. 
                // This method blocks until at least one byte is read.
                clientStream.Read(bytes, 0, client.ReceiveBufferSize);
                var str = Encoding.UTF8.GetString(bytes);
                sb.Append(str);
                if (str.Contains("ENDTRANS") || sb.ToString().Contains("ENDTRANS"))
                    continueToRead = false;
            }
            Console.WriteLine(@"Received: {0}", sb);
            LogManager.Log("Received: "+sb);
            sb.Replace("$", "").Replace("\0", "").Replace("\u001d","");
            var messages = StringHelper.ReplaceAndToArray(sb.ToString(), "ENDTRANS");
            CreatePlayer(client, messages[0]);

        }

        /// <summary>
        /// Create a mobile player if the message is valid
        /// </summary>
        /// <param name="client">The connected mobile client</param>
        /// <param name="message">The messesage the client sent</param>
        private static void CreatePlayer(TcpClient client, string message)
        {
            var clientDetails = message.Split(',');
            if (clientDetails.Length != 2)
            {
                LogManager.LogError("Invalid data sent from mobile client, closing connection");
                client.Close();
            }
            var pbkdf2 = new Rfc2898DeriveBytes(clientDetails[1], _salt, 1000);
            var hash = pbkdf2.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(_salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var clientStream = client.GetStream();

            LogManager.Log("Adding player : " + clientDetails[0] + " " + clientDetails[1]);
            SendMessage(MessageType.Info + " OK " + Convert.ToBase64String(hashBytes), clientStream);
            var mobPlay = new NetworkMobilePlayer(client, clientDetails[0],
                Convert.ToBase64String(hashBytes));
            if (_mobilePlayers.Count == 0)
            {
                mobPlay.IsVip = true;
                mobPlay.IsCurrentlyPlaying = true;
            }
            if (_mobilePlayers.Count == 1)
                mobPlay.IsNext = true;
            _mobilePlayers.Add(mobPlay);
            if (ServerGameWindowMaster.CurrentScreen.GetType() == typeof(LobbyScreen))
                ((LobbyScreen) ServerGameWindowMaster.CurrentScreen).AddPlayer(mobPlay);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="stream">Stream inwhich to send the message on</param>
        public static void SendMessage(string message, NetworkStream stream)
        {
            if (!stream.CanWrite) return;
            var bytesToWrite = Encoding.UTF8.GetBytes(message + " ENDTRANS");
            stream.Write(bytesToWrite, 0, bytesToWrite.Length);
            Console.WriteLine(@"connected");
            LogManager.Log("Sent message: " + message);
        }

        public static void MoveNextPlayerFlag()
        {
            lock (_mobilePlayers)
            {
                foreach (var t in _mobilePlayers)
                {
                    if (t.IsCurrentlyPlaying)
                    {
                        t.IsCurrentlyPlaying = false;
                    }
                }
                for (var i = 0; i < _mobilePlayers.Count; i++)
                {
                    if (_mobilePlayers[i].IsNext == false) continue;
                    _mobilePlayers[i].IsCurrentlyPlaying = true;
                    _mobilePlayers[i].IsNext = false;
                    if (i + 1 >= _mobilePlayers.Count)
                    {
                        _mobilePlayers[0].IsCurrentlyPlaying = false;
                        _mobilePlayers[0].IsNext = true;
                        break;
                    }
                    _mobilePlayers[i + 1].IsCurrentlyPlaying = false;
                    _mobilePlayers[i + 1].IsNext = true;
                    break;
                }
            }
        }
        public static void SendAll(string messageType, string message)
        {
            if (messageType.Equals(MessageType.Cmd))
            {
                switch (message)
                {
                    case "getplayerlist":
                        lock (_mobilePlayers)
                        {
                            foreach (var player in _mobilePlayers)
                                SendPlayerList(player.Client);
                        }
                        break;
                    case "startgame":
                        break;
                    default:
                        lock (_mobilePlayers)
                        {
                            foreach (var player in _mobilePlayers) {
                                SendMessage(MessageType.Cmd+" "+message, player.Client.GetStream());
                            }
                        }
                        break;
                }
            }
            else if (messageType.Equals(MessageType.Data))
            {

            }
        }

    }
}
