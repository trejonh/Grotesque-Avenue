using Horse.Engine.Utils;
using Horse.Server.Screens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Horse.Server.Core
{
    public  class ServerSocketManagerMaster
    {
        private static bool _listen;
        private static List<TcpClient> _mobileClients;
        private static TcpListener _listener;
        public static List<NetworkMobilePlayer> MobilePlayers;
        public event EventHandler PlayerDisconnected;
        private static Thread _checkConnectionThread;

        public ServerSocketManagerMaster()
        {
            _checkConnectionThread = new Thread(() =>
                {
                    try
                    {
                        while (ServerGameWindowMaster.GameWindow.IsOpen)
                        {
                            if (MobilePlayers == null || MobilePlayers.Count == 0)
                                continue;
                            var tempArr = MobilePlayers.ToArray();
                            foreach (var player in tempArr)
                            {
                                if (player.Client.Connected)
                                    continue;
                                player.Client.Close();
                                MobilePlayers.Remove(player);
                                OnPlayerDisconnected(new EventArgs());
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        LogManager.LogError("Aborting connection checking thread");
                    }
                })
                { Priority = ThreadPriority.Lowest , IsBackground = true, Name = "Client Connection Checker"};
            _checkConnectionThread.Start();
        }
        public static void Listen()
        {
            if (MobilePlayers != null && MobilePlayers.Count > 0 || _mobileClients != null && _mobileClients.Count > 0)
                CloseExistingConnections();
            _listen = true;
            MobilePlayers = new List<NetworkMobilePlayer>();
            _mobileClients = new List<TcpClient>();
            _listener = new TcpListener(IPAddress.Any, 54000);
            _listener.Start();
            Console.WriteLine("Listening...");
            StartAccept();
        }

        protected virtual void OnPlayerDisconnected(EventArgs e)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(this, e);
        }

        private static void CloseExistingConnections()
        {
            foreach (var player in MobilePlayers)
            {
                if (player.Client != null && player.Client.Connected)
                    player.Client.Close();
            }
            foreach (var client in _mobileClients)
            {
                if (client != null && client.Connected)
                    client.Close();
            }
        }

        public static void StopListening()
        {
            _listen = false;
        }

        public static void CloseAllConnections()
        {
            StopListening();
            CloseExistingConnections();
            _checkConnectionThread?.Abort();
        }

        private static void StartAccept()
        {
            _listener.BeginAcceptTcpClient(HandleAsyncConnection, _listener);
        }
        private static void HandleAsyncConnection(IAsyncResult res)
        {
            if (_listen == false && _mobileClients.Count >= 8)
                return;
            StartAccept(); //listen for new connections again
            TcpClient client = _listener.EndAcceptTcpClient(res);
            _mobileClients.Add(client);
            //proceed
            var clientStream = client.GetStream();
            var sb = new StringBuilder();
            while (clientStream.DataAvailable)
            {
                var bytes = new byte[client.ReceiveBufferSize];

                // Read can return anything from 0 to numBytesToRead. 
                // This method blocks until at least one byte is read.
                clientStream.Read(bytes, 0, client.ReceiveBufferSize);
                sb.Append(Encoding.UTF8.GetString(bytes));
            }

            CreatePlayer(client, sb.ToString());

        }

        private static void CreatePlayer(TcpClient client, string message)
        {
            var clientDetails = message.Split(',');
            if(clientDetails.Length != 3)
            {
                LogManager.LogError("Invalid data sent from mobile client, closing connection");
                client.Close();
            }
            var mobPlay = new NetworkMobilePlayer(client, clientDetails[0], clientDetails[1], clientDetails[2]);
            MobilePlayers.Add(mobPlay);
            if (ServerGameWindowMaster.CurrentScreen.GetType() == typeof(LobbyScreen))
                ((LobbyScreen)ServerGameWindowMaster.CurrentScreen).AddPlayer(mobPlay);
        }
    }
}
