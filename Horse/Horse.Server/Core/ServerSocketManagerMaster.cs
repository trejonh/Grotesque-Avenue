using Horse.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Horse.Server.Core
{
    public static class ServerSocketManagerMaster
    {

        private static Thread _listenerThread;
        private static bool _listen;
        private static List<TcpClient> _mobileClients;
        public static void Listen(string ip)
        {
            _listen = true;
            _mobileClients = new List<TcpClient>();
            _listenerThread = new Thread(
                new ThreadStart(()=> {
                    var listener = new TcpListener(IPAddress.Parse(ip), 54000);
                    listener.Start();
                    while (_listen)
                    {
                        if (_mobileClients.Count == 8)
                            continue;
                        var client = listener.AcceptTcpClient();
                        Console.WriteLine("accepted connection.");
                        _mobileClients.Add(client);
                        ThreadPool.QueueUserWorkItem(ProcessNewClient, client);
                    }
                }
            )) { Priority = ThreadPriority.BelowNormal, IsBackground = true };
            _listenerThread.Start();
        }

        public static void StopListening()
        {
            _listen = false;
        }

        private static void ProcessNewClient(object obj)
        {
            var client = (TcpClient)obj;
            var clientStream = client.GetStream();
            var sb = new StringBuilder();
            if (clientStream.DataAvailable)
            {
                var bytes = new byte[client.ReceiveBufferSize];

                // Read can return anything from 0 to numBytesToRead. 
                // This method blocks until at least one byte is read.
                clientStream.Read(bytes, 0, client.ReceiveBufferSize);
                var str = Encoding.UTF8.GetString(bytes);
                Console.WriteLine("Data: {0}", str);
            }
        }
       
    }
}
