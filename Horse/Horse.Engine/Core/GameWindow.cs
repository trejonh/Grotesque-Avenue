using SFML.Graphics;
using SFML.Window;

namespace Horse.Engine.Core
{
    public sealed class GameWindow
    {
        public static RenderWindow GameRenderWindow;

        public GameWindow(uint width, uint height, string title)
        {
            GameRenderWindow = new RenderWindow(new VideoMode(width,height),title,Styles.Fullscreen);
        }
    }
}
