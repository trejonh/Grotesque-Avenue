using System;
using Horse.Server.Core;
using SFML.Graphics;
using Horse.Engine.Utils;
using Horse.Engine.Core;
using System.Threading;
using SFML.System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using Horse.Server.Helpers;
using KeyEventArgs = SFML.Window.KeyEventArgs;

namespace Horse.Server.Games.ColorMePretty
{
    public class CmpGameScreen : GameScreen
    {
        public readonly Font CmpFont;
        public CmpScreenItem CurrentPaintBlob;
        private CmpScreenItem _playerQueue;
        private bool _isLoaded;
        private readonly Color _nearGray;
        private List<Color> _availableColors;
        private List<string> _availableColorTexts;
        private CmpScreenItem _turnCountDown;
        private CmpScreenItem _roundCountDown;
        private System.Timers.Timer _turnTimer;
        private System.Timers.Timer _roundTimer;
        private int _turnCd;
        private int _roundCd;
        private bool _turnCdStarted;
        private bool _roundCdStarted;
        private bool _dispTurnCd;
        private bool _dispRoundCd;
        private float _turnDispTime;
        private List<PlayerGameRecord> _playerGameRecords;
        private const float MaxAllowedDispTime = 4.5f;
        private bool _timesUp;
        private int _numPlayersCompleted;
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
                PrepareGameRecords();
                PrepareTimers();
                var toRmv = ScreenItems.Where(scrnItem => scrnItem.GetText() != null)
                                       .FirstOrDefault(scrnItem => scrnItem.GetText().DisplayedString.Equals(AssetManager.GetMessage("Loading")));
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

        private void PrepareGameRecords()
        {
            _playerGameRecords = new List<PlayerGameRecord>();
            foreach (var player in ServerSocketManagerMaster.Players)
            {
                if (player.IsCurrentlyPlaying)
                {
                    _playerGameRecords.Add(new PlayerGameRecord(){Name = player.Name, DeviceId = player.DeviceId, PickedCmp = true, Score = 0});
                    continue;
                }
                _playerGameRecords.Add(new PlayerGameRecord() { Name = player.Name, DeviceId = player.DeviceId, PickedCmp = false, Score = 0 });
            }
        }

        private void PrepareTimers()
        {
            _turnCd = 3;
            _roundCd = 30;
            _turnTimer = new System.Timers.Timer(1000){AutoReset = true, Enabled = true};
            _roundTimer = new System.Timers.Timer(1000){ AutoReset = true, Enabled = true};
            _turnTimer.Elapsed += (sender, args) =>
            {
                if (_dispTurnCd == false)
                    return;
                if (_turnCd < 0)
                    _turnCd = 3;
                if (_turnCountDown == null)
                {
                    var circle = new CircleShape(30.0f){OutlineColor = Color.Transparent, FillColor = _nearGray};
                    var text = new Text(){DisplayedString = ""+_turnCd, CharacterSize = 30, Font = CmpFont, Color = Color.Black};
                    var renderWindow = WinInstance;
                    _turnCountDown = new CmpScreenItem(ref renderWindow, circle, ScreenItem.ScreenPositions.Center, null);
                    text.Position = _turnCountDown.Position;
                    _turnCountDown.SetText(text);
                }
                if (_turnCd == 0)
                {
                    _turnCountDown.GetText().DisplayedString = AssetManager.GetMessage("Go");
                }
                else
                {
                    _turnCountDown.GetText().DisplayedString = "" + _turnCd;
                }
                _turnCd--;
            };
            _roundTimer.Elapsed += (sender, args) =>
            {
                if (_dispRoundCd == false)
                    return;
                if (_roundCd < 0)
                    _turnCd = 30;
                if (_roundCountDown == null)
                {
                    var circle = new CircleShape(30.0f) { OutlineColor = Color.Transparent, FillColor = _nearGray };
                    var text = new Text() { DisplayedString = "" + _turnCd, CharacterSize = 30, Font = CmpFont, Color = Color.Black };
                    var renderWindow = WinInstance;
                    _roundCountDown = new CmpScreenItem(ref renderWindow, circle, ScreenItem.ScreenPositions.BottomRight, null);
                    text.Position = _roundCountDown.Position;
                    _roundCountDown.SetText(text);
                }
                if (_roundCd == 0)
                {
                    _roundCountDown.GetText().DisplayedString = AssetManager.GetMessage("Stop");
                }
                else
                {
                    _roundCountDown.GetText().DisplayedString = "" + _roundCd;
                }
                _roundCd--;
            };
        }

        private void StartTurnTimer()
        {
            if (_turnCdStarted) return;
            if (_turnTimer == null)
                PrepareTimers();
            else
            {
                _turnCd = 3;
                _turnTimer.Start();
            }
            _turnCdStarted = true;
        }

        private void StartRoundTimer()
        {
            if (_roundCdStarted) return;
            if (_roundTimer == null)
                PrepareTimers();
            else
            {
                _roundCd = 30;
                _roundTimer.Start();
            }
            _roundCdStarted = true;
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
            if (_playerQueue == null)
            {
                var box = new RectangleShape(new Vector2f(240.0f, 120.0f))
                {
                    OutlineColor = Color.Transparent,
                    FillColor = _nearGray
                };
                var text = new Text()
                {
                    DisplayedString = sb.ToString(),
                    CharacterSize = 120,
                    Color = Color.Black,
                    Font = CmpFont
                };
                var position = new Vector2f(32.0f, WinInstance.Size.Y - (box.Size.Y * 1.25f));
                var win = WinInstance;
                _playerQueue = new CmpScreenItem(ref win, box, position, null);
                _playerQueue.SetText(text);
            }
            else
            {
                _playerQueue.GetText().DisplayedString = sb.ToString();
            }
            return _playerQueue;
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
                Position = new Vector2f(paintBlob.Position.X + blobSize.X / 4.0f, paintBlob.Position.Y + blobSize.Y / 4.0f)
            };
            CurrentPaintBlob.SetShape(outterBox);
            CurrentPaintBlob.SetText(text);
            CurrentPaintBlob.SetSprite(paintBlob);
            _availableColors = new List<Color> {
                new Color(139,69,19), // Brown
                new Color(128,0,128), // Purple
                new Color(255,140,0), // Orange
                Color.Black,
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Yellow
            };

            _availableColorTexts = new List<string> {
                AssetManager.GetMessage("Black"),
                AssetManager.GetMessage("Blue"),
                AssetManager.GetMessage("Green"),
                AssetManager.GetMessage("Orange"),
                AssetManager.GetMessage("Purple"),
                AssetManager.GetMessage("Red"),
                AssetManager.GetMessage("Yellow"),
                AssetManager.GetMessage("Brown"),
            };
            ScrambleBlob();
        }

        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            WinInstance.Draw(BgImage);
            if (_isLoaded)
            {
                if (_dispTurnCd && _dispRoundCd == false)
                {
                    if (_turnCdStarted == false)
                        StartTurnTimer();
                    _turnCountDown.Draw();
                    _turnDispTime += ServerGameWindowMaster.FrameDelta.AsSeconds();
                    if (_turnDispTime >= MaxAllowedDispTime)
                    {
                        _dispTurnCd = false;
                        _dispRoundCd = true;
                        _turnCdStarted = false;
                        _turnTimer.Stop();
                        _turnDispTime = 0.0f;
                    }
                }
                if (_dispRoundCd && _dispTurnCd == false && _timesUp == false)
                {
                    if(_roundCdStarted == false)
                        StartRoundTimer();
                    _roundCountDown.Draw();
                    if (_roundCountDown.GetText().DisplayedString == AssetManager.GetMessage("Go") && _timesUp == false)
                        _timesUp = true;
                }
                CurrentPaintBlob.Draw();
            }
            base.Draw();
        }

        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            // do nothing for now
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
            // do nothing for now
        }

        protected override async void GameFlow()
        {
            try
            {
                while (true)
                {
                    if(GameStarted == false)continue;
                    if (_timesUp)
                    {
                        ServerSocketManagerMaster.MoveNextPlayerFlag();
                        DisplayPlayerQueue();
                        ServerSocketManagerMaster.SendAll(MessageType.Cmd, "getplayerlist");
                        WaitForReadySignalAsync();
                        _dispTurnCd = true;
                        _dispRoundCd = false;
                        _timesUp = false;
                        ScrambleBlob();
                        _numPlayersCompleted++;
                    }
                    if(_dispTurnCd) continue;
                    var currPlayer = ServerSocketManagerMaster.Players.Single(pl => pl.IsCurrentlyPlaying);
                    var playerRecord =
                        _playerGameRecords.Single(rec => rec.Name.Equals(currPlayer.Name) &&
                                                         rec.DeviceId.Equals(currPlayer.DeviceId));
                    if (currPlayer.Client == null || currPlayer.Client.Connected == false)
                    {
                        LogManager.LogError(currPlayer.Name + "("+currPlayer.DeviceId+") is not connected to the game anymore");
                        continue;
                    }
                    var clientStream = currPlayer.Client.GetStream();
                    if(clientStream.DataAvailable == false)
                        continue;
                    var sb = new StringBuilder();
                    while (clientStream.DataAvailable)
                    {
                        var bytes = new byte[currPlayer.Client.ReceiveBufferSize];

                        // Read can return anything from 0 to numBytesToRead. 
                        // This method blocks until at least one byte is read.
                        await clientStream.ReadAsync(bytes, 0, currPlayer.Client.ReceiveBufferSize);
                        var str = Encoding.UTF8.GetString(bytes);
                        sb.Append(str);
                        if (sb.ToString().Contains("ENDTRANS"))
                            break;
                    }
                    LogManager.Log(currPlayer.Name + " " + currPlayer.DeviceId + " sent:" + sb);
                    sb.Replace("$", "").Replace("\0", "").Replace("\u001d", "");
                    var messages = StringHelper.ReplaceAndToArray(sb.ToString(), "ENDTRANS");
                    sb.Clear();
                    foreach (var mess in messages)
                    {
                        var message = ProcessMessage(currPlayer.Client, mess);
                        switch (message.Type)
                        {
                            case ProcessedMessageType.Error:
                                LogManager.LogError("An error ha occured with the current client");
                                break;
                            case ProcessedMessageType.Ok:
                                break;
                            case ProcessedMessageType.Command:
                                break;
                            case ProcessedMessageType.Data:
                                if (CurrentPaintBlob.GetText().DisplayedString.ToLower().Equals(message.Data))
                                {
                                    //award points/keep track of total right
                                    //play happy sound
                                    //now mix up blob color/text
                                    playerRecord.Score++;
                                    ScrambleBlob();
                                }
                                else
                                {
                                    //play incorrect sound and keep same blob up
                                }
                                break;
                            case ProcessedMessageType.StartGame:
                                IsPaused = false;
                                break;
                            case ProcessedMessageType.Pause:
                                IsPaused = true;
                                break;
                            case ProcessedMessageType.Other:
                                break;
                        }

                    }
                }
            }
            catch (ThreadAbortException)
            {
                LogManager.LogWarning("Exiting CMP game flow thread early due to abortion");
            }
            finally
            {
                LogManager.Log("Safely exiting cmp game thread");
            }
        }

        private async void WaitForReadySignalAsync()
        {
            var currPlayer = ServerSocketManagerMaster.Players.Single(pl => pl.IsCurrentlyPlaying);
            if (currPlayer.Client == null || currPlayer.Client.Connected == false)
            {
                LogManager.LogError(currPlayer.Name + "(" + currPlayer.DeviceId + ") is not connected to the game anymore");
                return;
            }
            var clientStream = currPlayer.Client.GetStream();
            ServerSocketManagerMaster.SendMessage(MessageType.Cmd+" sendreadysignal", clientStream);
            var sb = new StringBuilder();
            while (true)
            {
                var bytes = new byte[currPlayer.Client.ReceiveBufferSize];

                // Read can return anything from 0 to numBytesToRead. 
                // This method blocks until at least one byte is read.
                await clientStream.ReadAsync(bytes, 0, currPlayer.Client.ReceiveBufferSize);
                var str = Encoding.UTF8.GetString(bytes);
                sb.Append(str);
                if (sb.ToString().Contains("ENDTRANS"))
                    break;
            }
            var messages = StringHelper.ReplaceAndToArray(sb.ToString(), "ENDTRANS");
            foreach (var message in messages)
            {
                var mess = ProcessMessage(currPlayer.Client, message);
                if (mess.Type == ProcessedMessageType.Command && mess.Data.Equals("ready"))
                    break;
            }
        }

        private void ScrambleBlob()
        {
            var rand = new Random(ServerGameWindowMaster.FrameDelta.AsMilliseconds());
            CurrentPaintBlob.GetShape().FillColor = _availableColors[rand.Next(0, _availableColors.Count)];
            CurrentPaintBlob.GetText().DisplayedString = _availableColorTexts[rand.Next(0, _availableColorTexts.Count)];
        }

        protected override GameMessage ProcessMessage(TcpClient client, string message)
        {
            var sb = new StringBuilder(message);
            sb.Replace("$", "").Replace("\0", "").Replace("\u001d", "");
            message = sb.ToString();
            if (message.Contains(MessageType.Cmd))
            {
                var cmd = message.Substring(message.IndexOf(MessageType.Cmd, StringComparison.Ordinal) + 4).Trim().ToLower();
                switch (cmd)
                {
                    case "ready":
                        return new GameMessage(ProcessedMessageType.Command, "ready");
                    default:
                        LogManager.LogWarning("Command: " + cmd + " not found");
                        break;
                }
            }
            else if (message.Contains(MessageType.Data))
            {
                var data = message.Substring(message.IndexOf(MessageType.Data, StringComparison.Ordinal) + 4).Trim().ToLower();
                return new GameMessage(ProcessedMessageType.Data, data);
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

        private struct PlayerGameRecord
        {
            public string Name { get; set; }
            public string DeviceId { get; set; }
            public int Score { get; set; }
            public bool PickedCmp { get; set; }
        }
    }
}
