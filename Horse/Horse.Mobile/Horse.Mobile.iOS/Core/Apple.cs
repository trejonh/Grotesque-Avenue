using System;
using System.Runtime.InteropServices;
using Foundation;
using Horse.Mobile.Core;
using Horse.Mobile.iOS.Core;
[assembly: Xamarin.Forms.Dependency(typeof(Apple))]
namespace Horse.Mobile.iOS.Core
{
    public class Apple : IDevice
    {
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static extern uint IOServiceGetMatchingService(uint masterPort, IntPtr matching);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static extern IntPtr IOServiceMatching(string s);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static extern IntPtr IORegistryEntryCreateCFProperty(uint entry, IntPtr key, IntPtr allocator, uint options);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static extern int IOObjectRelease(uint o);

        public Apple()
        {
            
        }

        public string GetIdentifier()
        {
            var serial = string.Empty;
            var platformExpert = IOServiceGetMatchingService(0, IOServiceMatching("IOPlatformExpertDevice"));
            if (platformExpert == 0) return serial;
            var key = (NSString)"IOPlatformSerialNumber";
            var serialNumber = IORegistryEntryCreateCFProperty(platformExpert, key.Handle, IntPtr.Zero, 0);
            if (serialNumber != IntPtr.Zero)
            {
                serial = NSString.FromHandle(serialNumber);
            }

            IOObjectRelease(platformExpert);

            return serial;
        }
    }
}