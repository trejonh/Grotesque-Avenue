using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Net;
using System.Net.Sockets;

namespace Horse.Server.Screens
{
    public class LobbyScreen : Screen
    {
        private string _ipAdress;
        public LobbyScreen(ref RenderWindow window) : base(ref window)
        {
            _ipAdress = "";
            try
            {
                _ipAdress = GetLocalIPAdress();
            }
            catch (Exception)
            {
                LogManager.LogError("Local IP not found");
            }
            ServerSocketManagerMaster.Listen(_ipAdress);
            var serverName = new Text() { CharacterSize = 120, Color = Color.Black, DisplayedString = AssetManager.GetMessage("ServerName")+_ipAdress, Font = AssetManager.LoadFont("Shogun") };
            AddScreenItem(new ScreenItem(ref window, serverName, ScreenItem.ScreenPositions.BottomLeft,null));
            BgColor = AssetManager.LoadColor("FunkyPink");
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
    }
}
