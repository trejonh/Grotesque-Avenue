using Horse.Engine.Core;
using Horse.Engine.Utils;
using SFML.Graphics;
using SFML.Window;

namespace Horse.Server.Screens
{
    public sealed class SplashScreen : Screen
    {
        public SplashScreen():base()
        {
        }

        public SplashScreen(ref RenderWindow window) : base(ref window)
        {
            var text = new Text() { DisplayedString = AssetManager.GetMessage("GameTitle"), CharacterSize = 120, Color = AssetManager.LoadColor("FunkyPink"), Font = AssetManager.LoadFont("DefaultFont")};
            AddScreenItem(new ScreenItem(ref window, AssetManager.LoadSprite("HouseLogo"), ScreenItem.ScreenPositions.TopRight, null));
            AddScreenItem(new ScreenItem(ref window, AssetManager.LoadSprite("SFMLLogo"), ScreenItem.ScreenPositions.BottomLeft, null));
            AddScreenItem(new ScreenItem(ref window, text, ScreenItem.ScreenPositions.Center, null));
            BgColor = AssetManager.LoadColor("FunkyDarkBlue");
        }

        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            base.Draw();
        }
        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            //splash screens does nothing
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
            //splash screens does nothing
        }
    }
}
