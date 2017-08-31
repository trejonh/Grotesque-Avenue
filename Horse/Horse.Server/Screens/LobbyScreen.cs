using Horse.Engine.Core;
using Horse.Engine.Extensions;
using Horse.Engine.Utils;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Horse.Server.Screens
{
    public class LobbyScreen : Screen
    {
        public LobbyScreen(ref RenderWindow window) : base(ref window)
        {
            var ipAdress = "";
            try
            {
                ipAdress = GetLocalIPAdress();
            }
            catch (Exception)
            {
                LogManager.LogError("Local IP not found");
            }
            if(ServerGameFlowMaster.ServerSocket == null)
                ServerGameFlowMaster.ServerSocket = new ServerSocketManagerMaster();
            ServerGameFlowMaster.ServerSocket.PlayerDisconnected += ServerSocketOnPlayerDisconnected;
            ServerSocketManagerMaster.Listen();
            var serverName = new Text() { CharacterSize = 120, Color = Color.Black, DisplayedString = AssetManager.GetMessage("ServerName") + ipAdress, Font = AssetManager.LoadFont("Shogun") };
            AddScreenItem(new ScreenItem(ref window, serverName, ScreenItem.ScreenPositions.BottomLeft, null));
            var lobbyText = new Text() { CharacterSize = 120, Color = Color.Black, DisplayedString = AssetManager.GetMessage("Lobby"), Font = AssetManager.LoadFont("KissMeOrNot") };
            AddScreenItem(new ScreenItem(ref window, lobbyText, ScreenItem.ScreenPositions.Top, null));
            BgColor = AssetManager.LoadColor("FunkyPink");
        }

        private void ServerSocketOnPlayerDisconnected(object sender, EventArgs eventArgs)
        {
            var tmp = ScreenItems.ToArray();
            foreach (var item in tmp)
            {
                if (item.GetType() == typeof(LobbyScreenItem))
                    ScreenItems.Remove(item);
            }
            foreach (var player in ServerSocketManagerMaster.MobilePlayers)
            {
                AddPlayer(player);
            }
        }

        public override void Draw()
        {
            WinInstance.Clear(BgColor);
            base.Draw();
        }

        public override void OnKeyPress(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.Escape:
                    RemoveWindowKeyEventHandler();
                    ServerGameFlowMaster.ServerSocket.PlayerDisconnected -= ServerSocketOnPlayerDisconnected;
                    ServerSocketManagerMaster.CloseAllConnections();
                    ServerGameWindowMaster.GotoPreviousScreen();
                    break;
            }
        }

        public override void OnKeyRelease(object sender, KeyEventArgs keyEventArgs)
        {
            
        }

        private string GetLocalIPAdress()
        {
            var tmp = Dns.GetHostAddresses(Dns.GetHostName());
            return tmp.Length > 0 ? tmp[tmp.Length - 1].ToString() : "";
        }

        internal void AddPlayer(NetworkMobilePlayer mobPlay)
        {
            var roundedRect = new RoundedRectangle(new Vector2f(128.0f, 64.0f), 10f, 4) { FillColor = Color.Transparent, OutlineColor = AssetManager.LoadColor("FunkyPink")};
            var text = new Text()
            {
                Color = Color.Black,
                CharacterSize =  60,
                Font =  AssetManager.LoadFont("Hunt"),
                DisplayedString = mobPlay.Name
            };
            var renderWindow = WinInstance;
            var mobCount = ServerSocketManagerMaster.MobilePlayers.Count;
            if (mobCount % 2 == 1)
            {
                var pos = new Vector2f(32.0f,128.0f*mobCount);
                AddScreenItem(new LobbyScreenItem(ref renderWindow, roundedRect, pos, null));
                AddScreenItem(new LobbyScreenItem(ref renderWindow, text, pos, null));
            }
            else
            {
                var pos = new Vector2f(WinInstance.Size.X-256.0f,128.0f*mobCount);
                AddScreenItem(new LobbyScreenItem(ref renderWindow, roundedRect, pos, null));
                AddScreenItem(new LobbyScreenItem(ref renderWindow, text, pos, null));
            }
        }
    }
}
