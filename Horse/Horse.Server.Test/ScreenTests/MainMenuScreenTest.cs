using Microsoft.VisualStudio.TestTools.UnitTesting;
using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Screens;
using Horse.Server.Test.Utils;
using System.Collections.Generic;
using Horse.Server.Core;
using SFML.Window;

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
            TestUtilityHelper.CallPrivateMethod(typeof(ServerGameWindowMaster), "SetGameWindow", typeof(ServerGameWindowMaster), new object[]{GameWindow.GameRenderWindow});
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
            Assert.IsTrue(((List<ScreenItem>)items).Count > 0);
        }

        [TestMethod]
        public void PresentLobbyTest()
        {
            var mainMenu = new MainMenu(ref GameWindow.GameRenderWindow);
           TestUtilityHelper.CallPrivateMethod(typeof(MainMenu), "PresentLobbyAndStartServer", mainMenu, null);
            var lobbyScreen = ServerGameWindowMaster.CurrentScreen;
            var items = TestUtilityHelper.GetField(typeof(Screen), "ScreenItems", lobbyScreen);
            var color = TestUtilityHelper.GetField(typeof(Screen), "BgColor", lobbyScreen);
            var listening = (bool)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_listen", null);
            Assert.IsNotNull(items);
            Assert.IsNotNull(color);
            Assert.IsTrue(listening);
            Assert.IsNotNull(ServerGameFlowMaster.ServerSocket);
            Assert.IsTrue(((List<ScreenItem>)items).Count > 0);
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(LobbyScreen));
            Assert.AreEqual(0, TestUtilityHelper.CallPrivateMethod(typeof(MainMenu), "QuitGame", mainMenu, null));
            Assert.IsFalse(GameWindow.GameRenderWindow.IsOpen);

        }
        [TestMethod]
        public void EscapeKeyPress()
        {
            var mainMenu = new MainMenu(ref GameWindow.GameRenderWindow);
            TestUtilityHelper.CallPrivateMethod(typeof(ServerGameWindowMaster), "SetCurrentScreen", typeof(ServerGameWindowMaster), new []{mainMenu});
            TestUtilityHelper.CallPrivateMethod(typeof(MainMenu), "PresentLobbyAndStartServer", mainMenu, null);
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(LobbyScreen));
            var lobby = ServerGameWindowMaster.CurrentScreen;
            var keypress = new KeyEventArgs(new KeyEvent())
            {
                Code = Keyboard.Key.Escape
            };
            TestUtilityHelper.CallPrivateMethod(typeof(Screen), "OnKeyPress", lobby, new object[] { null, keypress });
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(MainMenu));
            var listening = (bool)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_listen", null);
            Assert.IsFalse(listening);
            Assert.AreEqual(0, TestUtilityHelper.CallPrivateMethod(typeof(MainMenu), "QuitGame", mainMenu, null));
            Assert.IsFalse(GameWindow.GameRenderWindow.IsOpen);

        }
    }
}
