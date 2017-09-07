using System;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.Window;
using Horse.Engine.Utils;
using Horse.Engine.Core;
using System.Threading;
using SFML.System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Horse.Server.Games.ColorMePretty
{
    public class CmpGameScreen : GameScreen
    {
        public readonly Font CmpFont;
        public CmpScreenItem CurrentPaintBlob;
        private bool _isLoaded;
        private readonly Color _nearGray;
        private List<Color> _availableColors;
        private List<string> _availableColorTexts;
        private CmpScreenItem _turnCountDown;
        private CmpScreenItem _roundCountDown;
        private int _turnCD = 3;
        private int _roundCD = 15;
        private bool _turnCDStarted;
        private bool _roundCDStarted;
        public CmpGameScreen(ref RenderWindow window) : base(ref window)
        {
            _nearGray = new Color(181,181,181,183);
            BgColor = Color.Transparent;
            BgImage = AssetManager.LoadSprite("CMPBackground");
            var bgText = BgImage.Texture;
            var scaleX = WinInstance.Size.X / (bgText.Size.X*1.0f);
            var scaleY = WinInstance.Size.Y / (bgText.Size.Y*1.0f);
            BgImage.Scale = new Vector2f(scaleX,scaleY);
            CmpFont = AssetManager.LoadFont("KissMeOrNot");
            var loading = new Text() { DisplayedString = AssetManager.GetMessage("Loading"), Font = CmpFont, CharacterSize = 120, Color = Color.Black };
            var loadingSrcnItem = new ScreenItem(ref window, loading, ScreenItem.ScreenPositions.Center, null);
            var lBox = new RectangleShape(new Vector2f(120.0f, 60.0f)) { Position = loadingSrcnItem.Position, OutlineColor = Color.Transparent, FillColor = _nearGray};
            loadingSrcnItem.SetShape(lBox);
            AddScreenItem(loadingSrcnItem);
            var title = new Text() { DisplayedString = AssetManager.GetMessage("ColorMePretty"), Font = CmpFont, CharacterSize = 120, Color = Color.Black };
            var titleScrnItem = new ScreenItem(ref window, title, ScreenItem.ScreenPositions.Top, null);
            var tBox = new RectangleShape(new Vector2f(120.0f, 60.0f)) { Position = titleScrnItem.Position, OutlineColor = Color.Transparent, FillColor = _nearGray };
            titleScrnItem.SetShape(tBox);
            AddScreenItem(titleScrnItem);
            LoadingThread = new Thread(Load) { Priority = ThreadPriority.Normal, IsBackground = true, Name = "CMPLoadingThread"};
            LoadingThread.Start();
        }

        private void Load()
        {
            try
            {
                var playerQueue = DisplayPlayerQueue();
                LoadPaint();
                WaitForPlayersToReadInstructionsAysnc();
                PrepareTimers();
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
                AddScreenItem(playerQueue);
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

        private void PrepareTimers()
        {
        }

        private async void WaitForPlayersToReadInstructionsAysnc()
        {
            var numRead = 0;
            while(numRead < ServerSocketManagerMaster.Players.Count)
            {
                foreach(var player in ServerSocketManagerMaster.Players)
                {
                    if (player.Client == null)
                        continue;
                    if (player.Client.Connected == false)
                        continue;
                    var clientStream = player.Client.GetStream();
                    if (clientStream.DataAvailable == false)
                        continue;
                    var sb = new StringBuilder();
                    while (clientStream.DataAvailable)
                    {
                        var bytes = new byte[player.Client.ReceiveBufferSize];

                        // Read can return anything from 0 to numBytesToRead. 
                        // This method blocks until at least one byte is read.
                        await clientStream.ReadAsync(bytes, 0, player.Client.ReceiveBufferSize);
                        var str = Encoding.UTF8.GetString(bytes);
                        sb.Append(str);
                        if (sb.ToString().Contains("ENDTRANS"))
                            break;
                    }
                    LogManager.Log(player.Name + " " + player.DeviceId + " sent:" + sb);
                    if (ProcessMessage(player.Client, sb.Replace(" ENDTRANS", "").ToString()).Type == ProcessedMessageType.StartGame)
                        numRead++;
                    sb.Clear();
                }
            }
        }

        private ScreenItem DisplayPlayerQueue()
        {
            var currPlayer = ServerSocketManagerMaster.Players.Single(player => player.IsCurrentlyPlaying);
            var nextPlayer = ServerSocketManagerMaster.Players.Single(player => player.IsNext);
            var sb = new StringBuilder();
            sb.Append(currPlayer.Name).Append(AssetManager.GetMessage("IsCurrentlyPlaying")).AppendLine();
            sb.Append(nextPlayer.Name).Append(" ").Append(AssetManager.GetMessage("IsNext")).AppendLine();
            var box = new RectangleShape(new Vector2f(240.0f, 120.0f)) { OutlineColor = Color.Transparent, FillColor = _nearGray};
            var text = new Text() { DisplayedString = sb.ToString(), CharacterSize = 120, Color = Color.Black, Font = CmpFont };
            var position = new Vector2f(32.0f, WinInstance.Size.Y - (box.Size.Y * 1.25f));
            var win = WinInstance;
            var item = new ScreenItem(ref win, box, position, null);
            item.SetText(text);
            return item;
        }

        private void LoadPaint()
        {
            var win = WinInstance;
            CurrentPaintBlob = new CmpScreenItem(ref win);
            var paintBlob = AssetManager.LoadSprite("PaintBlob");
            var blobSize = paintBlob.Texture.Size;
            var outterBox =
                new RectangleShape(new Vector2f(blobSize.X, blobSize.Y))
                {
                    OutlineColor = Color.Transparent,
                    FillColor = Color.White,
                    Position = new Vector2f(WinInstance.Size.X - blobSize.X / 2, WinInstance.Size.Y - blobSize.Y / 2)
                };
            paintBlob.Position = outterBox.Position;
            var text = new Text()
            {
                DisplayedString = "",
                Font = CmpFont,
                CharacterSize = 60,
                Position = new Vector2f(paintBlob.Position.X + blobSize.X / 4, paintBlob.Position.Y + blobSize.Y / 4)
            };
            CurrentPaintBlob.SetShape(outterBox);
            CurrentPaintBlob.SetText(text);
            CurrentPaintBlob.SetSprite(paintBlob);
            _availableColors = new List<Color>() {
                new Color(139,69,19), // Brown
                new Color(128,0,128), // Purple
                new Color(255,140,0), // Orange
                Color.Black,
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Yellow
            };

            _availableColorTexts = new List<string>() {
                AssetManager.GetMessage("Black"),
                AssetManager.GetMessage("Blue"),
                AssetManager.GetMessage("Green"),
                AssetManager.GetMessage("Orange"),
                AssetManager.GetMessage("Purple"),
                AssetManager.GetMessage("Red"),
                AssetManager.GetMessage("Yellow"),
                AssetManager.GetMessage("Brown"),
            };
        }

        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            WinInstance.Draw(BgImage);
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

        protected override GameMessage ProcessMessage(TcpClient client, string message)
        {
            var sb = new StringBuilder(message);
            sb.Replace("$", "").Replace("\0", "");
            message = sb.ToString();
            if (message.Contains(MessageType.Cmd))
            {
                var cmd = message.Substring(message.IndexOf(MessageType.Cmd, StringComparison.Ordinal) + 4).Trim().ToLower();
                switch (cmd)
                {
                    default:
                        LogManager.LogWarning("Command: " + cmd + " not found");
                        break;
                }
            }
            else if (message.Contains(MessageType.Data))
            {

            }
            else if (message.Contains(MessageType.Info))
            {
                var info = message.Substring(message.IndexOf(MessageType.Cmd, StringComparison.Ordinal) + 4).Trim().ToLower();
                switch (info)
                {
                    case "readinstructions":
                        return new GameMessage(ProcessedMessageType.StartGame,null);
                    default:
                        var player = ServerSocketManagerMaster.Players.Single(pl => pl.Client == client);
                        LogManager.LogWarning("Info: " + info + " <br/> FROM: "+player?.Name);
                        break;
                }
            }
            else
            {
                LogManager.Log("Message from client: " + message.Replace(MessageType.Info, ""));
            }
            return new GameMessage(0,"");
        }
    }
}
