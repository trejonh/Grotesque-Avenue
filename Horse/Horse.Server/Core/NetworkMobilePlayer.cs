using System.Net.Sockets;

namespace Horse.Server.Core
{
    /// <summary>
    /// A mobile player that is connected to 
    /// the game server
    /// </summary>
    public class NetworkMobilePlayer
    {
        /// <summary>
        /// Display name of the player
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The unique hashed device id
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// Determines who the lead player is
        /// </summary>
        public bool IsVip { get;  set; }

        /// <summary>
        /// Determines who'se turn is next
        /// </summary>
        public bool IsNext { get; set; }
        public bool IsCurrentlyPlaying { get; set; }

        /// <summary>
        /// The connected tcp client to communicate to the player
        /// </summary>
        public TcpClient Client { get; private set; }

        /// <summary>
        /// Create a networked mobile player
        /// </summary>
        /// <param name="client">The connected TCP Client used for communication</param>
        /// <param name="name"></param>
        /// <param name="devId"></param>
        public NetworkMobilePlayer(TcpClient client, string name, string devId)
        {
            Client = client;
            Name = name;
            DeviceId = devId;
        }
    }
}
