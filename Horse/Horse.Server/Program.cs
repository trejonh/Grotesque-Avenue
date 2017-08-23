using Horse.Engine.Utils;
using Horse.Server.Core;

namespace Horse.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            new ServerGameFlowMaster().BeginFlow();
        }
    }
}
