using SFML.Graphics;
using SFML.Window;

namespace Horse.Engine.Core
{
    public sealed class GameWindow
    {
        public static RenderWindow GameRenderWindow;

        public GameWindow(uint width, uint height, string title,bool debug)
        {
            GameRenderWindow = debug == false ? new RenderWindow(new VideoMode(width,height),title,Styles.Fullscreen)
                    : new RenderWindow(new VideoMode(width, height), title, Styles.Close);
        }
    }
}
