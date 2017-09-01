using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Screens;
using Horse.Server.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Horse.Server.Test.ScreenTests
{
    [TestClass]
    public class SplashScreenTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            new GameWindow(100, 100, "title", true);
        }

        [ClassCleanup]
        public static void Teardown()
        {
            GameWindow.GameRenderWindow?.Close();
            LogManager.CloseLog();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void DefaultConstructorTest()
        {
            var splashScreen = new SplashScreen();
            var items = TestUtilityHelper.GetField(typeof(Screen), "ScreenItems", splashScreen);
            Assert.IsNotNull(items);
            Assert.AreEqual(0, ((List<ScreenItem>)items).Count);
            splashScreen.Draw();
        }

        [TestMethod]
        public void GamewindowConstructorTest()
        {
            var splashScreen = new SplashScreen(ref GameWindow.GameRenderWindow);
            var items = TestUtilityHelper.GetField(typeof(Screen), "ScreenItems", splashScreen);
            var color = TestUtilityHelper.GetField(typeof(Screen), "BgColor", splashScreen);
            Assert.IsNotNull(items);
            Assert.IsNotNull(color);
            Assert.IsTrue(((List<ScreenItem>)items).Count > 0);
            splashScreen.Draw();
        }
    }
}
