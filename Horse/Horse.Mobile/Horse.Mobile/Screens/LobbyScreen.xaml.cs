using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Horse.Mobile.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Horse.Mobile.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LobbyScreen : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        public TcpClient ServerConnection { get; private set; }
        private string _playerName;
        public LobbyScreen()
        {
            InitializeComponent();

            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };
            _playerName = "";
            BindingContext = this;
        }

        public LobbyScreen(IPAddress ip, string playerNameText) : this()
        {
            InitializeComponent();
            _playerName = playerNameText;
            ServerConnection = new TcpClient();
            ServerConnection.BeginConnect(ip, 54000, Connected, ServerConnection);
            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            BindingContext = this;
        }

        private void Connected(IAsyncResult ar)
        {
            try
            {
               /* ServerConnection = (TcpClient)ar.AsyncState;
                ServerConnection.EndConnect(ar);*/
                SendInitialMessageToServer();
            }
            catch (Exception ex)
            {
                //log error
                Console.WriteLine(ex.ToString());
            }
        }

        private void SendInitialMessageToServer()
        {
            var deviceId = DependencyService.Get<IDevice>().GetIdentifier();
            DisplayAlert("ID", deviceId, "ok","cancel");
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            if (((ListView)sender).SelectedItem == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}