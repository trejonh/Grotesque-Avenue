using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System.Profile;
using Horse.Mobile.Core;
using Horse.Mobile.UWP.Core;

[assembly: Xamarin.Forms.Dependency(typeof(WindowsPhone))]
namespace Horse.Mobile.UWP.Core
{
    public class WindowsPhone : IDevice
    {
        public WindowsPhone() { }
        public string GetIdentifier()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var buffer = hardwareId.ToArray();
            return Convert.ToBase64String(buffer);
        }
    }
}
