using SFML.Graphics;
using SFML.Window;

namespace Horse.Engine.Core
{
    /// <summary>
    /// A render window holder class
    /// </summary>
    public sealed class GameWindow
    {
        /// <summary>
        /// The sole game window
        /// </summary>
        public static RenderWindow GameRenderWindow;

        /// <summary>
        /// Initiate the game window
        /// </summary>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window</param>
        /// <param name="title">The window title</param>
        /// <param name="debug">Enable fullscreen if false</param>
        public GameWindow(uint width, uint height, string title,bool debug)
        {
            GameRenderWindow = debug == false ? new RenderWindow(new VideoMode(width,height),title,Styles.Fullscreen)
                    : new RenderWindow(new VideoMode(width, height), title, Styles.Close);
        }
    }
}
