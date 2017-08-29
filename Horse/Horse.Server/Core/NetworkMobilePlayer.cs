using System.Net.Sockets;

namespace Horse.Server.Core
{
    public class NetworkMobilePlayer
    {
        public string Name { get; private set; }
        public string DeviceId { get; private set; }
        public TcpClient Client { get; private set; }
        public NetworkMobilePlayer(TcpClient client, string name, string devId)
        {
            Client = client;
            Name = name;
            DeviceId = devId;
        }
    }
}
