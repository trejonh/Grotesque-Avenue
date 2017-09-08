using System;
using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Horse.Server.Games.ColorMePretty;

namespace Horse.Server.Screens
{
    public class GameSelectionScreen : Screen
    {
        private bool _isInitialSelect;
        private static GameSelectionScreen _gss;
        private static object locker = new object();
        private bool _initialized;
        private GameSelectionScreen(ref RenderWindow window, bool isInit = true) : base(ref window)
        {
            _isInitialSelect = isInit;
            var bubble = AssetManager.LoadFont("Bubble");
            var text = new Text(){DisplayedString = AssetManager.GetMessage("GameSelectionScreen"), CharacterSize = 120, Color = Color.Black, Font = bubble};
            AddScreenItem(new ScreenItem(ref window, text, ScreenItem.ScreenPositions.Top, null));
            BgColor = AssetManager.LoadColor("FunkyTeal");
            var vip = new Text() { DisplayedString = ServerSocketManagerMaster.Players[0].Name + " " + AssetManager.GetMessage("IsSelect"), CharacterSize = 60, Color = Color.Black, Font = bubble };
            var next = new Text() { DisplayedString = ServerSocketManagerMaster.Players[1].Name + " " + AssetManager.GetMessage("IsNext"), CharacterSize = 60, Color = Color.Black, Font = bubble };
            AddScreenItem(new ScreenItem(ref window, vip, new Vector2f(32.0f, WinInstance.Size.Y - (vip.CharacterSize*2.5f)), null));
            AddScreenItem(new ScreenItem(ref window, next, new Vector2f(32.0f, WinInstance.Size.Y-(next.CharacterSize*1.5f)), null));
            ServerSocketManagerMaster.SendAll(MessageType.Cmd, "selectagame");
            _initialized = true;
        }

        public static GameSelectionScreen GetInstance()
        {
            if(_gss == null)
            {
                var win = ServerGameWindowMaster.GameWindow;
                lock (locker) {
                    _gss = new GameSelectionScreen(ref win);
                }
            }
            return _gss;
        }
        public override void Draw()
        {
            if (_initialized == false)
                return;
            WinInstance.Clear(BgColor);
            base.Draw();
        }
        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
        }

        internal static void PlayGame(string gameTitle)
        {
            switch (gameTitle)
            {
                case "cmp":
                    GetInstance().RemoveWindowKeyEventHandler();
                    var win = ServerGameWindowMaster.GameWindow;
                    ServerGameWindowMaster.ChangeScreen(new CmpGameScreen(ref win));
                    break;
            }
        }
    }
}
