using Horse.Engine.Utils;
using Horse.Server.Screens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Horse.Server.Core
{
    /// <summary>
    /// Master socket manager
    /// </summary>
    public  class ServerSocketManagerMaster
    {
        /// <summary>
        /// Listen for incoming connections
        /// </summary>
        private static bool _listen;

        /// <summary>
        /// The connected mobile clients
        /// </summary>
        private static List<TcpClient> _mobileClients;

        /// <summary>
        /// The listener for tcp connections
        /// </summary>
        private static TcpListener _listener;

        /// <summary>
        /// The connected mobile players
        /// </summary>
        public static List<NetworkMobilePlayer> MobilePlayers;

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

        /// <summary>
        /// Initiate the connection checking thread
        /// </summary>
        public ServerSocketManagerMaster()
        {
            new RNGCryptoServiceProvider().GetBytes(_salt = new byte[16]);
            _checkConnectionThread = new Thread(PollMobileClients)
            { Priority = ThreadPriority.Lowest , IsBackground = true, Name = "Client Connection Checker"};
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
                    if (MobilePlayers == null || MobilePlayers.Count == 0)
                        continue;
                    NetworkMobilePlayer[] tempArr;
                    lock (MobilePlayers)
                    {
                        tempArr = MobilePlayers.ToArray();
                    }
                    foreach (var player in tempArr)
                    {
                        if (player == null)
                        {
                            continue;
                        }
                        if (player.Client == null)
                        {
                            lock (MobilePlayers)
                            {
                                MobilePlayers.Remove(player);
                            }
                            OnPlayerDisconnected(new EventArgs());
                            continue;
                        }
                        if (player.Client.Connected == false)
                        {
                            player.Client.Close();
                            lock (MobilePlayers)
                            {
                                MobilePlayers.Remove(player);
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
                        ProcessMessage(player.Client,sb.Replace(" ENDTRANS","").ToString());
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
            StringBuilder sb = new StringBuilder(message);
            sb.Replace("$", "").Replace("\0", "");
            message = sb.ToString();
            if (message.Contains(MessageType.Cmd))
            {
                var cmd = message.Substring(message.IndexOf(MessageType.Cmd, StringComparison.Ordinal)+4).Trim().ToLower();
                switch (cmd) {
                    case "getplayerlist":
                        SendPlayerList(client);
                        break;
                    default:
                        LogManager.LogWarning("Command: "+cmd+" not found");
                        break;
                }
            }
            else if (message.Contains(MessageType.Data))
            {

            }
            else
            {
                LogManager.Log("Message from client: "+message.Replace(MessageType.Info,""));
            }
        }

        /// <summary>
        /// Send the list of currently connected players to the client
        /// </summary>
        /// <param name="client">The client to send the list to</param>
        private void SendPlayerList(TcpClient client)
        {
            try
            {
                var sb = new StringBuilder(MessageType.Info+" playerList[");
                foreach (var player in MobilePlayers)
                {
                    sb.Append("player: ");
                    sb.Append(player.Name);
                    sb.Append(",");
                    sb.Append(player.DeviceId);
                    sb.Append(player.IsVip ? ",true" : ",false");
                    sb.Append(player.IsNext ? ",true" : ",false");
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
            if (_listen)
                return;
            if (MobilePlayers != null && MobilePlayers.Count > 0 || _mobileClients != null && _mobileClients.Count > 0)
                CloseExistingConnections();
            MobilePlayers = new List<NetworkMobilePlayer>();
            _mobileClients = new List<TcpClient>();
            if (_listener != null)
            {
                _listener.Server.Close();
                _listener.Stop();
            }
            _listener = new TcpListener(IPAddress.Any, 54000);
            _listener.Start();
            _listen = true;
            Console.WriteLine("Listening...");
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
            foreach (var player in MobilePlayers)
            {
                if (player?.Client != null && player.Client.Connected)
                    player.Client.Close();
            }
            foreach (var client in _mobileClients)
            {
                if (client != null && client.Connected)
                    client.Close();
            }
        }

        /// <summary>
        /// Stop listening for incoming connections
        /// </summary>
        public static void StopListening()
        {
            _listen = false;
            try
            {
                _listener.Stop();
            }
            catch (SocketException ex)
            {
                LogManager.LogError("Listener stop exception. error code: "+ex.ErrorCode);
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

            if (_listener == null || _listener.Server.Connected == false)
            {
                LogManager.LogError("Error with listener");
                return;
            }
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
            if (_listen == false && _mobileClients.Count >= 8)
                return;
            StartAccept(); //listen for new connections again
            if (_listener == null || _listener.Server.Connected == false)
            {
                LogManager.LogError("Error with listener");
                return;
            }
            var client = _listener.EndAcceptTcpClient(res);
            _mobileClients.Add(client);
            //proceed
            var clientStream = client.GetStream();
            var sb = new StringBuilder();
            var continueToRead = true;
            while (continueToRead){
                var bytes = new byte[client.ReceiveBufferSize];

                // Read can return anything from 0 to numBytesToRead. 
                // This method blocks until at least one byte is read.
                clientStream.Read(bytes, 0, client.ReceiveBufferSize);
                var str = Encoding.UTF8.GetString(bytes);
                sb.Append(str);
                Console.WriteLine("received: {0}", str);
                if (str.Contains("ENDTRANS") || sb.ToString().Contains("ENDTRANS"))
                    continueToRead = false;
            }

            CreatePlayer(client, sb.Replace(" ENDTRANS","").ToString());

        }

        /// <summary>
        /// Create a mobile player if the message is valid
        /// </summary>
        /// <param name="client">The connected mobile client</param>
        /// <param name="message">The messesage the client sent</param>
        private static void CreatePlayer(TcpClient client, string message)
        {
            var clientDetails = message.Split(',');
            if(clientDetails.Length != 2)
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

            LogManager.Log("Adding player : " + clientDetails[0].Substring(1) + " " + clientDetails[1]);
            SendMessage(MessageType.Info+" OK " + Convert.ToBase64String(hashBytes), clientStream);
            var mobPlay = new NetworkMobilePlayer(client, clientDetails[0].Substring(1),Convert.ToBase64String(hashBytes));
            if (MobilePlayers.Count == 0)
                mobPlay.IsVip = true;
            if (MobilePlayers.Count == 1)
                mobPlay.IsNext = true;
            MobilePlayers.Add(mobPlay);
            if (ServerGameWindowMaster.CurrentScreen.GetType() == typeof(LobbyScreen))
                ((LobbyScreen)ServerGameWindowMaster.CurrentScreen).AddPlayer(mobPlay);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="stream">Stream inwhich to send the message on</param>
        public static void SendMessage(string message,  NetworkStream stream)
        {
            if (!stream.CanWrite) return;
            var bytesToWrite = Encoding.UTF8.GetBytes(message + " ENDTRANS");
            stream.Write(bytesToWrite, 0, bytesToWrite.Length);
            Console.WriteLine("sent");
            LogManager.Log("Sent message: " + message);
        }
    }
}
