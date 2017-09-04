using System.Threading;
using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using Horse.Server.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Horse.Server.Test.CoreTests
{
    [TestClass]
    public class ServerSocketTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            new GameWindow(100, 100, "title", true);
            TestUtilityHelper.CallPrivateMethod(typeof(ServerGameWindowMaster), "SetGameWindow",
                typeof(ServerGameWindowMaster), new[] { GameWindow.GameRenderWindow});
        }

        [ClassCleanup]
        public static void Teardown()
        {
            GameWindow.GameRenderWindow?.Close();
            LogManager.CloseLog();
            ServerSocketManagerMaster.CloseAllConnections();
        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var ssm = new ServerSocketManagerMaster();
            Assert.IsNotNull(TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_salt", ssm));
            var thread =
                (Thread)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_checkConnectionThread", ssm);
            Assert.IsNotNull(thread);
            Assert.IsTrue(thread.IsAlive);
        }

        [TestMethod]
        public void ListenTest()
        {
            ServerSocketManagerMaster.Listen();
            Assert.IsTrue((bool)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_listen", typeof(ServerSocketManagerMaster)));
        }

        [TestMethod]
        public void StopListenTest()
        {
            ServerSocketManagerMaster.StopListening();
            Assert.IsFalse((bool)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_listen", typeof(ServerSocketManagerMaster)));
        }
    }
}
