using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Horse.Server.Screens
{
    public class GameSelectionScreen : Screen
    {
        private bool _isInitialSelect;
        public GameSelectionScreen(ref RenderWindow window, bool isInit = true) : base(ref window)
        {
            _isInitialSelect = isInit;
            var bubble = AssetManager.LoadFont("Bubble");
            var text = new Text(){DisplayedString = AssetManager.GetMessage("GameSelectionScreen"), CharacterSize = 120, Color = Color.Black, Font = bubble};
            AddScreenItem(new ScreenItem(ref window, text, ScreenItem.ScreenPositions.Top, null));
            BgColor = AssetManager.LoadColor("FunkyTeal");
            var arrowLeft = new ScreenItem(ref window, AssetManager.LoadSprite("ArrowLeft"), new Vector2f(32.0f, window.Size.Y / 2.0f - 32), null);
            var pink = AssetManager.LoadColor("FunkyPink");
            arrowLeft.SetShape(new CircleShape(10.0f){ FillColor = pink, OutlineColor = Color.Transparent, Position = new Vector2f(32.0f, window.Size.Y / 2.0f - 32) });
            var arrowRight = new ScreenItem(ref window, AssetManager.LoadSprite("ArrowRight"), new Vector2f(window.Size.X-3*32.0f, window.Size.Y / 2.0f - 32), null);
            arrowRight.SetShape(new CircleShape(10.0f) { FillColor = pink, OutlineColor = Color.Transparent, Position = new Vector2f(window.Size.X - 3 * 32.0f, window.Size.Y / 2.0f - 32) });
            AddScreenItem(arrowRight);
            AddScreenItem(arrowLeft);
            var vip = new Text() { DisplayedString = ServerSocketManagerMaster.Players[0].Name + " " + AssetManager.GetMessage("IsSelect"), CharacterSize = 60, Color = Color.Black, Font = bubble };
            var next = new Text() { DisplayedString = ServerSocketManagerMaster.Players[1].Name + " " + AssetManager.GetMessage("IsNext"), CharacterSize = 60, Color = Color.Black, Font = bubble };
            AddScreenItem(new ScreenItem(ref window, vip, ScreenItem.ScreenPositions.BottomLeft, null));
            AddScreenItem(new ScreenItem(ref window, next, ScreenItem.ScreenPositions.BottomRight, null));
            foreach (var player in ServerSocketManagerMaster.Players)
            {
                ServerSocketManagerMaster.SendMessage(MessageType.Cmd+" selectagame", player.Client.GetStream());
            }
        }

        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            base.Draw();
        }
        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
        }
    }
}
