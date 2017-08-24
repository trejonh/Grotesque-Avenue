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
        public static RenderWindow GameWindow { get; private set; }
        public static Screen CurrentScreen { get; private set; }
        private static Thread _mainDrawingThread;
        public static Time FrameDelta { get; private set; }
        private static Clock _serverClock;
        public static void InitGameWindow()
        {
            var resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var width = (uint)resolution.Width;
            var height = (uint)resolution.Height;
            var title = AssetManager.GetMessage("GameTitle");
            new GameWindow(width,height,title, true);
            GameWindow = Engine.Core.GameWindow.GameRenderWindow;
            _serverClock = new Clock();
            if (GameWindow == null)
            {
                LogManager.LogError("The game window is null, cannot continue");
                return;
            }
            GameWindow.SetActive(false);
            var ts = new ThreadStart(Draw);
            _mainDrawingThread = new Thread(ts) { IsBackground = false, Priority = ThreadPriority.Highest };
            _mainDrawingThread.Start();
        }

        private static void Draw()
        {
            try
            {
                GameWindow.SetMouseCursorVisible(false);
                while (GameWindow.IsOpen)
                {
                    if(_serverClock != null)
                        FrameDelta = _serverClock.Restart();
                    if(CurrentScreen != null)
                    {
                        CurrentScreen.Draw();
                    }
                    GameWindow.Display();
                }
            }catch(Exception ex)
            {
                LogManager.LogError("Exception thrown in servermaster draw: "+ex.Message+"<br/>"+ex.StackTrace);
            }
            finally
            {
                LogManager.CloseLog();
                GameWindow.Close();
            }
        }
        public static void ChangeScreen(Screen screen)
        {
            CurrentScreen = screen;
        }
        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WindowClosed(object sender, System.EventArgs e)
        {
            _mainDrawingThread?.Abort();
            LogManager.CloseLog();
            GameWindow.Close();
        }
    }
}
