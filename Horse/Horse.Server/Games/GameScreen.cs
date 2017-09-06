using Horse.Engine.Core;
using Horse.Server.Core;
using SFML.Graphics;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Horse.Server.Games
{
    public abstract class GameScreen : Screen
    {
        public enum ProcessedMessageType
        {
            Error,
            Ok,
            Command,
            Data,
            Other
        }
        protected Thread GameThread;
        protected Thread LoadingThread;
        protected Sprite BgImage;
        protected GameScreen(ref RenderWindow window):base(ref window)
        {
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

        protected abstract ProcessedMessageType ProcessMessage(TcpClient client, string message);
    }
}
