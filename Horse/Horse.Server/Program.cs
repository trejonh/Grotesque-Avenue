using System;
using Horse.Engine.Utils;
using Horse.Server.Core;

namespace Horse.Server
{
    internal class Program
    {
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UncaughtExceptionHandler;
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            new ServerGameFlowMaster().BeginFlow();
          //  LogManager.EmailLogs();
            LogManager.CloseLog();
            Environment.Exit(0);
        }

        private static void UncaughtExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            LogManager.LogError("Experienced fatal system error and will be closing down the application");
            LogManager.LogError(ex.Message);
            LogManager.LogError(ex.StackTrace);
            LogManager.CloseLog();
            Environment.Exit(0);
        }
    }
}
