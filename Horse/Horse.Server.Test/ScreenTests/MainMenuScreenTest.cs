using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Screens;
using Horse.Server.Test.Utils;
using System.Collections.Generic;
using Horse.Server.Core;

namespace Horse.Server.Test.ScreenTests
{
    [TestClass]
    public class MainMenuScreenTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
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
        public void GamewindowConstructorTest()
        {
            var mainMenu = new MainMenu(ref GameWindow.GameRenderWindow);
            var items = TestUtilityHelper.GetField(typeof(Screen), "ScreenItems", mainMenu);
            var color = TestUtilityHelper.GetField(typeof(Screen), "BgColor", mainMenu);
            Assert.IsNotNull(items);
            Assert.IsNotNull(color);
            Assert.IsNotNull(TestUtilityHelper.GetField(typeof(MainMenu), "_pointer", mainMenu));
            Assert.IsNotNull(TestUtilityHelper.GetField(typeof(MainMenu), "_pointerPositions", mainMenu));
            Assert.IsTrue(((List<ScreenItem>)items).Count > 0);
        }

        [TestMethod]
        public void PresentLobbyTest()
        {
            var mainMenu = new MainMenu(ref GameWindow.GameRenderWindow);
            Assert.AreEqual(0, TestUtilityHelper.CallPrivateMethod(typeof(MainMenu), "PresentLobbyAndStartServer", mainMenu, null));
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(LobbyScreen));

        }

        [TestMethod]
        public void QuitGameTest()
        {
            var mainMenu = new MainMenu(ref GameWindow.GameRenderWindow);
            Assert.AreEqual(0,TestUtilityHelper.CallPrivateMethod(typeof(MainMenu), "QuitGame",mainMenu, null));
            Assert.IsFalse(GameWindow.GameRenderWindow.IsOpen);

        }
    }
}
