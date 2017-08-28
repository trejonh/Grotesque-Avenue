using Android.Provider;
using Horse.Mobile.Core;
using Horse.Mobile.Droid.Core;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPhone))]
namespace Horse.Mobile.Droid.Core
{
    public class AndroidPhone : IDevice
    {
        public AndroidPhone()
        {
            
        }
        public string GetIdentifier()
        {
            return Settings.Secure.GetString(Forms.Context.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}