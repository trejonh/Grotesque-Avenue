using System;
using Horse.Engine.Utils;
using Horse.Server.Core;

namespace Horse.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UncaughtExceptionHandler);
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            new ServerGameFlowMaster().BeginFlow();
            LogManager.CloseLog();
        }

        private static void UncaughtExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            LogManager.LogError("Experienced fatal system error and will be closing down the application");
            LogManager.LogError(ex.Message);
            LogManager.LogError(ex.StackTrace);
            LogManager.CloseLog();
        }
    }
}
