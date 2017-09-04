using Horse.Engine.Core;
using Horse.Server.Screens;
using System.Timers;
using System;
using Horse.Engine.Utils;

namespace Horse.Server.Core
{
    /// <summary>
    /// Controls the flow of the game
    /// </summary>
    public sealed class ServerGameFlowMaster
    {
        /// <summary>
        /// Timer for simple tasks
        /// </summary>
        private Timer _flowTimer;
        
        /// <summary>
        /// Reference to the splash screen
        /// </summary>
        private Screen _splashScreen;

        /// <summary>
        /// Referencce to the main menu
        /// </summary>
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
            _flowTimer.Start();
        }

        /// <summary>
        /// Begin the game flow
        /// </summary>
        public void BeginFlow()
        {
            if (ServerGameWindowMaster.GameWindow.IsOpen == false)
                return;
            try
            {
                while (ServerGameWindowMaster.GameWindow.IsOpen)
                {
                    ServerGameWindowMaster.GameWindow.DispatchEvents();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError("Major exception occurred while dispatching events <br/>" + ex.Message);
            }
            finally
            {
                QuitGame();
            }
        }

        /// <summary>
        /// Close any open sockets, stop drawing, and close the window
        /// </summary>
        internal static void QuitGame()
        {
            ServerSocketManagerMaster.CloseAllConnections();
            ServerGameWindowMaster.StopDrawing();
            ServerGameWindowMaster.GameWindow?.Close();
        }
    }
}
