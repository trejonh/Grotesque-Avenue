using Horse.Engine.Core;
using Horse.Server.Core;
using SFML.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace Horse.Server.Games
{
    public abstract class GameScreen : Screen
    {
        protected List<NetworkMobilePlayer> Players;
        protected Thread GameThread;
        protected Thread LoadingThread;
        protected GameScreen(ref RenderWindow window, ref List<NetworkMobilePlayer> players):base(ref window)
        {
            Players = players;
        }

        public void StopGameThread()
        {
            ServerSocketManagerMaster.IsGameThreadControllingInput = false;
            GameThread?.Abort();
        }

        public void StopLoading()
        {
            LoadingThread?.Abort();
        }
    }
}
