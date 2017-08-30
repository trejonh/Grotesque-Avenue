using Horse.Engine.Core;
using Horse.Server.Screens;
using System.Timers;
using System;
using Horse.Engine.Utils;

namespace Horse.Server.Core
{
    public sealed class ServerGameFlowMaster
    {
        private Timer _flowTimer;
        private Screen _splashScreen;
        private Screen _mainMenu;
        public static ServerSocketManagerMaster ServerSocket;
        public ServerGameFlowMaster()
        {
            ServerGameWindowMaster.InitGameWindow();
            var win = ServerGameWindowMaster.GameWindow;
            _splashScreen = new SplashScreen(ref win);
            ServerGameWindowMaster.ChangeScreen(_splashScreen);
            _flowTimer = new Timer(1000) { AutoReset = false, Enabled = true};
            _flowTimer.Elapsed +=  (sender, e) => {
                _splashScreen.RemoveWindowKeyEventHandler();
                _mainMenu = new MainMenu(ref win);
                ServerGameWindowMaster.ChangeScreen(_mainMenu);
            };
            _flowTimer.Start();;
        }

        public void BeginFlow()
        {
            if (ServerGameWindowMaster.GameWindow.IsOpen == false)
                return;
            while (ServerGameWindowMaster.GameWindow.IsOpen)
            {
                ServerGameWindowMaster.GameWindow.DispatchEvents();
            }
        }

        internal static void QuitGame()
        {
            ServerGameWindowMaster.GameWindow.Close();
        }
    }
}
