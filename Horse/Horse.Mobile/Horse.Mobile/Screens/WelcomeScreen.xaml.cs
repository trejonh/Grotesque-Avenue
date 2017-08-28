using System;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Horse.Mobile.Screens
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WelcomeScreen : ContentPage
	{
	    public WelcomeScreen()
	    {
	        InitializeComponent();
	        NavigationPage.SetHasNavigationBar(this, false);
	        JoinGameLobby.Clicked += JoinGameLobby_Clicked;
	    }

	    private void JoinGameLobby_Clicked(object sender, System.EventArgs e)
	    {
	        IPAddress ip;
	        if (IsOkToConnectToServer(out ip))
	        {
	            Navigation.PushAsync(new LobbyScreen(ip, PlayerName.Text));
	        }
	    }

	    private bool IsOkToConnectToServer(out IPAddress ip)
	    {
	        if (string.IsNullOrEmpty(ServerAddress.Text) || string.IsNullOrEmpty(PlayerName.Text))
	        {
	            DisplayAlert("Error", "Please fill in all fields", "OK");
	            ip = null;
	            return false;
	        }
	        if (PlayerName.Text.Length < 3)
	        {
	            DisplayAlert("Error", "Display name must be 3 or more characters.", "OK");
	            ip = null;
	            return false;
	        }
	        try
	        {
	            ip = IPAddress.Parse(ServerAddress.Text);
	        }
	        catch (FormatException)
	        {
	            DisplayAlert("Server Address Error", "The provided server address is not valid", "OK");
	            ip = null;
	            return false;
	        }
	        return true;
	    }
    }
}