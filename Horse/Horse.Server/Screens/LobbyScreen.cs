using Horse.Engine.Core;
using Horse.Engine.Extensions;
using Horse.Engine.Utils;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Net;
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
            var serverName = new Text() { CharacterSize = 120, Color = Color.Black, DisplayedString = AssetManager.GetMessage("ServerName")+ipAdress, Font = AssetManager.LoadFont("Shogun") };
            AddScreenItem(new ScreenItem(ref window, serverName, ScreenItem.ScreenPositions.BottomLeft,null));
            BgColor = AssetManager.LoadColor("FunkyPink");
        }

        private void ServerSocketOnPlayerDisconnected(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
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
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            throw new Exception("Local IP Address Not Found!");
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
            AddScreenItem(new ScreenItem(ref renderWindow, roundedRect, ScreenItem.ScreenPositions.TopLeft, null));
            AddScreenItem(new ScreenItem(ref renderWindow, text, ScreenItem.ScreenPositions.TopLeft, null));
        }
    }
}
