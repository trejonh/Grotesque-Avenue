using System;
using System.Timers;
using Horse.Mobile.Screens;
using Xamarin.Forms;

namespace Horse.Mobile
{
	public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
		    MainLogo.Source = "crazyhorse.png";
		    Device.StartTimer(TimeSpan.FromMilliseconds(2500), () =>
		    {
		        Navigation.PushAsync(new WelcomeScreen());
                Navigation.RemovePage(this);
		        return false;
		    });
		}
	}
}
