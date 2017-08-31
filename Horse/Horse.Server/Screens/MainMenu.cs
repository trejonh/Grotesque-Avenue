using System;
using Horse.Engine.Core;
using SFML.Window;
using SFML.Graphics;
using Horse.Engine.Extensions;
using SFML.System;
using Horse.Engine.Utils;
using System.Collections.Generic;
using Horse.Server.Core;

namespace Horse.Server.Screens
{
    /// <summary>
    /// The main menu of the game
    /// </summary>
    public class MainMenu : Screen
    {
        /// <summary>
        /// The pointer for the main menu
        /// </summary>
        private readonly Sprite _pointer;
        /// <summary>
        /// The pointer positions
        /// </summary>
        private readonly IEnumerator<Vector2f> _pointerPositions;
        /// <summary>
        /// The previous position
        /// </summary>
        private Vector2f _previous;

        /// <summary>
        /// Create the main menu of the game
        /// </summary>
        /// <param name="window">The window to draw to</param>
        public MainMenu(ref RenderWindow window):base(ref window)
        {
            var roundRect = new RoundedRectangle(new Vector2f(384, 108), 5f, 4) { FillColor = AssetManager.LoadColor("FunkyTeal"), OutlineColor = AssetManager.LoadColor("FunkyYellow") };
            var roundRect2 = new RoundedRectangle(new Vector2f(256, 96), 5f, 4) { FillColor = AssetManager.LoadColor("FunkyTeal"), OutlineColor = AssetManager.LoadColor("FunkyYellow") };
            var font = AssetManager.LoadFont("DefaultFont");
            var startLobby =  new Text() { CharacterSize = 72, DisplayedString = AssetManager.GetMessage("StartLobby"), Font = font, Color = AssetManager.LoadColor("FunkyPink")};
            var exitGame = new Text() { CharacterSize = 72, DisplayedString = AssetManager.GetMessage("Exit"), Font = font, Color = AssetManager.LoadColor("FunkyRed") };
            var title = new Text() { DisplayedString = AssetManager.GetMessage("GameTitle"), CharacterSize = 120, Color = AssetManager.LoadColor("FunkyPink"), Font = font };
            AddScreenItem(new ScreenItem(ref window, title, ScreenItem.ScreenPositions.Top, null));
            var pos = new Vector2f(WinInstance.Size.X/2,WinInstance.Size.Y - WinInstance.Size.Y/3);
            AddScreenItem(new ScreenItem(ref window, roundRect, new Vector2f(pos.X - (startLobby.CharacterSize * 3.0f), pos.Y - 8), PresentLobbyAndStartServer));
            AddScreenItem(new ScreenItem(ref window, roundRect2, new Vector2f(WinInstance.Size.X/2.0f - roundRect.GetSize().X/2.45f, WinInstance.Size.Y - roundRect.GetSize().Y - 32),QuitGame));
            AddScreenItem(new ScreenItem(ref window, startLobby,  new Vector2f(pos.X - (startLobby.CharacterSize * 2.5f), pos.Y), null));
            AddScreenItem(new ScreenItem(ref window, exitGame, new Vector2f(WinInstance.Size.X/2.0f - (exitGame.CharacterSize+40), WinInstance.Size.Y - roundRect.GetSize().Y - 32), null));
            BgColor = AssetManager.LoadColor("FunkyDarkBlue");
            _pointer = AssetManager.LoadSprite("Pointer");
            if (_pointer == null){
                LogManager.LogError("MainMenu Screen cannot find pointer");
                return;
            }
            _pointer.Scale = new Vector2f(0.5f, 0.4f);
            var pp = new List<Vector2f>();
            foreach (var item in ScreenItems)
            {
                if (item.IsShape())
                    pp.Add(new Vector2f(item.Position.X - (_pointer.Texture.Size.X * 0.75f), item.Position.Y - 16));
            }
            _pointerPositions = pp.GetEnumerator();
            _pointerPositions.Reset();
            _pointerPositions.MoveNext();
            _pointer.Position = _pointerPositions.Current;
        }

        /// <summary>
        /// Draws the main menu
        /// </summary>
        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            WinInstance.Draw(_pointer);
            base.Draw();
        }


        /// <summary>
        /// Handles key presses for the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyEventArgs"></param>
        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.Up:
                    if (Math.Abs(_previous.X) < 0.1f && Math.Abs(_previous.Y) < 0.1f)
                    {
                        while (_pointerPositions.MoveNext())
                            _previous = _pointerPositions.Current;
                        _pointer.Position = _previous;
                    }
                    else
                    {
                        _previous = _pointerPositions.Current;
                        _pointerPositions.Reset();
                        _pointerPositions.MoveNext();
                        _pointer.Position = _pointerPositions.Current;
                    }
                    break;
                case Keyboard.Key.Down:
                    _previous = _pointerPositions.Current;
                    if (_pointerPositions.MoveNext())
                        _pointer.Position = _pointerPositions.Current;
                    else
                    {
                        _pointerPositions.Reset();
                        _pointerPositions.MoveNext();
                        _pointer.Position = _pointerPositions.Current;
                    }
                    break;
                case Keyboard.Key.Return:
                    foreach(var item in ScreenItems)
                    {
                        if (Math.Abs(item.Position.Y - (_pointerPositions.Current.Y + 16)) < 1.0f)
                            item.DoAction();
                    }
                    break;
                case Keyboard.Key.Escape:
                    ServerGameFlowMaster.QuitGame();
                    break;
            }
        }

        /// <summary>
        /// Handle key release on the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyEventArgs"></param>
        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
           
        }

        /// <summary>
        /// Remove the key handlers and switch to the lobby screen
        /// </summary>
        /// <returns></returns>
        private int PresentLobbyAndStartServer()
        {
            RemoveWindowKeyEventHandler();
            var renderWindow = WinInstance;
            ServerGameWindowMaster.ChangeScreen(new LobbyScreen(ref renderWindow));
            return 0;
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        /// <returns></returns>
        private int QuitGame()
        {
            ServerGameFlowMaster.QuitGame();
            return 0;
        }
    }
}
