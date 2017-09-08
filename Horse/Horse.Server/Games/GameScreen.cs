using Horse.Engine.Core;
using Horse.Server.Core;
using SFML.Graphics;
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
            StartGame,
            Pause,
            Other
        }

        public struct GameMessage
        {
            public ProcessedMessageType Type { get; }
            public string Data { get; }
            public GameMessage(ProcessedMessageType type, string data)
            {
                Type = type;
                Data = data;
            }
        }
        protected Thread GameThread;
        protected Thread LoadingThread;
        protected Sprite BgImage;
        public bool GameStarted { get; protected set; }
        public bool IsPaused { get; protected set; }
        protected GameScreen(ref RenderWindow window):base(ref window)
        {
            GameThread = new Thread(GameFlow){Priority = ThreadPriority.AboveNormal, Name = "Thread for individual game glow", IsBackground = true};
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

        protected abstract void GameFlow();

        protected abstract GameMessage ProcessMessage(TcpClient client, string message);
    }
}
