using Horse.Engine.Core;
using Horse.Engine.Utils;
using SFML.Graphics;
using SFML.Window;

namespace Horse.Server.Games.FryThatEgg
{
    public class FryThatEggMainScreen : Screen
    {
        public FryThatEggMainScreen(ref RenderWindow window) : base(ref window)
        {
            BgColor = AssetManager.LoadColor("FunkYellow");
        }

        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            base.Draw();
        }

        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            throw new System.NotImplementedException();
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
            throw new System.NotImplementedException();
        }
    }
}
