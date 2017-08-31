using Horse.Engine.Core;
using Horse.Engine.Utils;
using SFML.Graphics;
using SFML.Window;

namespace Horse.Server.Screens
{
    /// <summary>
    /// The main splash screen for HORSE
    /// </summary>
    public sealed class SplashScreen : Screen
    {
        /// <summary>
        /// Creates  a splash screen with null properties
        /// </summary>
        public SplashScreen():base()
        {
        }

        /// <summary>
        /// Creates a splash screen that can be drawn
        /// </summary>
        /// <param name="window">The window to draw to</param>
        public SplashScreen(ref RenderWindow window) : base(ref window)
        {
            var text = new Text() { DisplayedString = AssetManager.GetMessage("GameTitle"), CharacterSize = 120, Color = AssetManager.LoadColor("FunkyPink"), Font = AssetManager.LoadFont("DefaultFont")};
            AddScreenItem(new ScreenItem(ref window, AssetManager.LoadSprite("HouseLogo"), ScreenItem.ScreenPositions.TopRight, null));
            AddScreenItem(new ScreenItem(ref window, AssetManager.LoadSprite("SFMLLogo"), ScreenItem.ScreenPositions.BottomLeft, null));
            AddScreenItem(new ScreenItem(ref window, text, ScreenItem.ScreenPositions.Center, null));
            BgColor = AssetManager.LoadColor("FunkyDarkBlue");
        }

        /// <summary>
        /// Draw the splash screen
        /// </summary>
        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            base.Draw();
        }

        /// <summary>
        /// Handle key press for splash screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyEventArgs"></param>
        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            //splash screens does nothing
        }

        /// <summary>
        /// Key release handler for splash screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyEventArgs"></param>
        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
            //splash screens does nothing
        }
    }
}
