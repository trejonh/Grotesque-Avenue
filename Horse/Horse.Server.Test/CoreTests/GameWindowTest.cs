using System;
using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using Horse.Server.Screens;
using Horse.Server.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Horse.Server.Test.CoreTests
{
    [TestClass]
    public class GameWindowTest
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
        public void InitGameWindowTest()
        {
            Assert.IsNull(ServerGameWindowMaster.GameWindow);
            Assert.IsNull(ServerGameWindowMaster.CurrentScreen);
            ServerGameWindowMaster.InitGameWindow();
            Assert.IsNotNull(ServerGameWindowMaster.GameWindow);
            Assert.IsNull(ServerGameWindowMaster.CurrentScreen);
            Assert.IsTrue(ServerGameWindowMaster.GameWindow.IsOpen);
            Assert.IsTrue(Math.Abs(ServerGameWindowMaster.FrameDelta.AsSeconds()) < 0.0001f);
        }

        [TestMethod]
        public void ChangeScreenTest()
        {
            Assert.IsTrue(Math.Abs(ServerGameWindowMaster.FrameDelta.AsSeconds()) > 0.0f);
            ServerGameWindowMaster.ChangeScreen(new TestingScreen());
            Assert.IsNull(TestUtilityHelper.GetField(typeof(ServerGameWindowMaster),"_previousScreen",typeof(ServerGameWindowMaster)));
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(TestingScreen));
            Assert.IsTrue(ServerGameWindowMaster.GameWindow.IsOpen);
        }

        [TestMethod]
        public void GotoPreviousScreenNullTest()
        {
            ServerGameWindowMaster.GotoPreviousScreen();
            Assert.IsNotNull(ServerGameWindowMaster.CurrentScreen);
            Assert.IsTrue(ServerGameWindowMaster.GameWindow.IsOpen);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GotoPreviousScreenNotNullTest()
        {
            TestUtilityHelper.SetField(typeof(ServerGameWindowMaster), "_previousScreen", typeof(ServerGameWindowMaster), new []{new SplashScreen(), });
            ServerGameWindowMaster.GotoPreviousScreen();
            Assert.IsNotNull(ServerGameWindowMaster.CurrentScreen);
            Assert.IsTrue(TestUtilityHelper.GetField(typeof(ServerGameWindowMaster), "_previousScreen", typeof(ServerGameWindowMaster)).GetType() == typeof(LobbyScreen));
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(TestingScreen));
            Assert.IsTrue(ServerGameWindowMaster.GameWindow.IsOpen);
        }

        [TestMethod]
        public void StopDrawingTest()
        {
            ServerGameWindowMaster.StopDrawing();
            Assert.IsFalse(ServerGameWindowMaster.GameWindow.IsOpen);
        }
    }
}
