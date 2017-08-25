using System.Net.Sockets;

namespace Horse.Server.Core
{
    public class NetworkMobilePlayer
    {
        public string Name { get; private set; }
        public string DeviceName { get; private set; }
        public string DeviceID { get; private set; }
        public TcpClient Client { get; private set; }
        public NetworkMobilePlayer(TcpClient client, string name, string devName, string devID)
        {
            Client = client;
            Name = name;
            DeviceName = devName;
            DeviceID = devID;
        }
    }
}
