using System;
using System.Collections.Generic;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.Window;
using Horse.Engine.Utils;
using Horse.Engine.Core;
using System.Threading;
using SFML.System;
using System.Linq;
namespace Horse.Server.Games.ColorMePretty
{
    public class CMPGameScreen : GameScreen
    {
        public Font CMPFont;
        public CMPScreenItem CurrentPaintBlob;
        private bool _isLoaded;

        public CMPGameScreen(ref RenderWindow window, ref List<NetworkMobilePlayer> players) : base(ref window, ref players)
        {
            BgColor = Color.White;
            CMPFont = AssetManager.LoadFont("KissMeOrNot");
            var loading = new Text() { DisplayedString = AssetManager.GetMessage("Loading"), Font = CMPFont, CharacterSize = 120, Color = Color.Black };
            AddScreenItem(new ScreenItem(ref window, loading, ScreenItem.ScreenPositions.Center, null));
            var title = new Text() { DisplayedString = AssetManager.GetMessage("ColorMePretty"), Font = CMPFont, CharacterSize = 120, Color = Color.Black };
            AddScreenItem(new ScreenItem(ref window, title, ScreenItem.ScreenPositions.Top, null));
            LoadingThread = new Thread(Load) { Priority = ThreadPriority.Normal, IsBackground = true, Name = "CMPLoadingThread"};
            LoadingThread.Start();
        }

        private void Load()
        {
            try
            {
                DisplayPlayerQueue();
                LoadPaint();
                ScreenItem toRmv = null;
                foreach(ScreenItem scrnItem in ScreenItems)
                {
                    if (scrnItem.GetText() == null)
                        continue;
                    if (scrnItem.GetText().DisplayedString.Equals(AssetManager.GetMessage("Loading")))
                    {
                        toRmv = scrnItem;
                        break;
                    }
                }
                if(toRmv != null)
                    ScreenItems.Remove(toRmv);
                _isLoaded = true;
            }
            catch (ThreadAbortException)
            {
                LogManager.LogWarning("Color me pretty loading aborted early.");
            }
            finally
            {
                LogManager.Log("Color me pretty finished loading");
            }
        }

        private void DisplayPlayerQueue()
        {
            var currPlayer = Players.Single(player => player.IsCurrentlyPlaying);
            var nextPlayer = Players.Single(player => player.IsNext);
        }

        private void LoadPaint()
        {
            var win = WinInstance;
            CurrentPaintBlob = new CMPScreenItem(ref win);
            var paintBlob = AssetManager.LoadSprite("PaintBlob");
            var blobSize = paintBlob.Texture.Size;
            var outterBox = new RectangleShape(new Vector2f(blobSize.X, blobSize.Y)) { OutlineColor = Color.Transparent};
            outterBox.Position = new Vector2f(WinInstance.Size.X - blobSize.X/2, WinInstance.Size.Y - blobSize.Y/2);
            paintBlob.Position = outterBox.Position;
            var text = new Text() { DisplayedString = "", Font = CMPFont, CharacterSize = 60, Position = new Vector2f(paintBlob.Position.X+blobSize.X/4, paintBlob.Position.Y + blobSize.Y/4)};
            CurrentPaintBlob.SetShape(outterBox);
            CurrentPaintBlob.SetText(text);
            CurrentPaintBlob.SetSprite(paintBlob);
        }

        public override void Draw()
        {
            WinInstance.Clear();
            if (_isLoaded)
            {
                CurrentPaintBlob.Draw();
            }
            base.Draw();
        }

        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            throw new NotImplementedException();
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
