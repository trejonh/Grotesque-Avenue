using Horse.Engine.Core;
using Horse.Engine.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Threading;

namespace Horse.Server.Core
{
    public static class ServerGameWindowMaster
    {
        /// <summary>
        /// The main game window
        /// </summary>
        public static RenderWindow GameWindow { get; private set; }

        /// <summary>
        /// The current screen that is being drawn
        /// </summary>
        public static Screen CurrentScreen { get; private set; }

        /// <summary>
        /// The previous screen
        /// </summary>
        private static Screen _previousScreen;

        /// <summary>
        /// The thread that draws screens and other items
        /// </summary>
        private static Thread _mainDrawingThread;

        /// <summary>
        /// The frame rate clock
        /// </summary>
        public static Time FrameDelta { get; private set; }

        /// <summary>
        /// The main game clock
        /// </summary>
        private static Clock _serverClock;

        /// <summary>
        /// Initialize the game window and start the drawing thread
        /// </summary>
        public static void InitGameWindow()
        {
            var resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var width = (uint)resolution.Width;
            var height = (uint)resolution.Height;
            var title = AssetManager.GetMessage("GameTitle");
            new GameWindow(width,height,title, true);
            GameWindow = Engine.Core.GameWindow.GameRenderWindow;
            GameWindow.SetVerticalSyncEnabled(true);
            _serverClock = new Clock();
            if (GameWindow == null)
            {
                LogManager.LogError("The game window is null, cannot continue");
                return;
            }
            GameWindow.SetActive(false);
            GameWindow.Closed += WindowClosed;
            var ts = new ThreadStart(Draw);
            _mainDrawingThread = new Thread(ts) { IsBackground = false, Priority = ThreadPriority.Highest };
            _mainDrawingThread.Start();
        }

        /// <summary>
        /// Draw the current screen and reset the frame clock
        /// </summary>
        private static void Draw()
        {
            try
            {
                GameWindow.SetMouseCursorVisible(false);
                while (GameWindow.IsOpen)
                {
                    if(_serverClock != null)
                        FrameDelta = _serverClock.Restart();
                    CurrentScreen?.Draw();
                    GameWindow.Display();
                }
            }catch(Exception ex)
            {
                LogManager.LogError("Exception thrown in servermaster draw: "+ex.Message+"<br/>"+ex.StackTrace);
            }
            finally
            {
                GameWindow.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screen"></param>
        public static void ChangeScreen(Screen screen)
        {
            _previousScreen = CurrentScreen;
            CurrentScreen = screen;
        }

        public static void GotoPreviousScreen()
        {
            if (_previousScreen == null) return;
            CurrentScreen = _previousScreen;
            CurrentScreen.AddWindowKeyEventHandler();
        }

        public static void StopDrawing()
        {
            _mainDrawingThread?.Abort();
        }
        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WindowClosed(object sender, System.EventArgs e)
        {
            _mainDrawingThread?.Abort();
            GameWindow.Close();
        }
    }
}
