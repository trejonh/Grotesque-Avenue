using Horse.Engine.Core;
using Horse.Engine.Utils;
using Horse.Server.Core;
using Horse.Server.Screens;
using Horse.Server.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFML.Window;
using System.Collections.Generic;

namespace Horse.Server.Test.ScreenTests
{
	[TestClass]
	public class LobbyScreenTest
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
            ServerSocketManagerMaster.CloseAllConnections();
            LogManager.CloseLog();
        }

        [TestMethod]
        public void GamewindowConstructorTest()
        {
            var lobbyScreen = new LobbyScreen(ref GameWindow.GameRenderWindow);
            var items = TestUtilityHelper.GetField(typeof(Screen), "ScreenItems", lobbyScreen);
            var color = TestUtilityHelper.GetField(typeof(Screen), "BgColor", lobbyScreen);
            var listening = (bool)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_listen", null);
            Assert.IsNotNull(items);
            Assert.IsNotNull(color);
            Assert.IsTrue(listening);
            Assert.IsNotNull(ServerGameFlowMaster.ServerSocket);
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
        public void EscapeKeyPress()
        {
            var lobby = new LobbyScreen(ref GameWindow.GameRenderWindow);
            var keypress = new KeyEventArgs(new KeyEvent());
            keypress.Code = Keyboard.Key.Escape;
            Assert.AreEqual(0, TestUtilityHelper.CallPrivateMethod(typeof(Screen), "OnKeyPress", lobby, new[] { null, keypress }));
            Assert.IsTrue(ServerGameWindowMaster.CurrentScreen.GetType() == typeof(MainMenu));
            var listening = (bool)TestUtilityHelper.GetField(typeof(ServerSocketManagerMaster), "_listen", null);
            Assert.IsFalse(listening);

        }
    }
}
