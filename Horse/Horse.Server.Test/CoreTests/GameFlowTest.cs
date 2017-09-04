using System;
using System.Timers;
using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using Horse.Server.Screens;
using Horse.Server.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Horse.Server.Test.CoreTests
{
    [TestClass]
    public class GameFlowTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
        }

        [ClassCleanup]
        public static void Teardown()
        {
            GameWindow.GameRenderWindow?.Close();
            LogManager.CloseLog();
        }

        [TestMethod]
        public void CreateFlowTest()
        {
            new ServerGameFlowMaster();
            var type = ServerGameWindowMaster.CurrentScreen.GetType();
            Assert.AreEqual(type,typeof(SplashScreen));
            Assert.IsTrue(ServerGameWindowMaster.GameWindow.IsOpen);
        }

        [TestMethod]
        public void QuitGameTest()
        {
            new ServerGameFlowMaster();
            var timer = new Timer(1000) { AutoReset = false, Enabled = true };
            timer.Elapsed += (sender, e) => {
                Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(MainMenu));
                Assert.IsTrue(ServerGameWindowMaster.GameWindow.IsOpen);
                TestUtilityHelper.CallPrivateMethod(typeof(ServerGameFlowMaster), "QuitGame",
                    typeof(ServerGameWindowMaster), null);
            };
            timer.Start();
        }
    }
}
