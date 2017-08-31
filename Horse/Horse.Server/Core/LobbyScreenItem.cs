using System;
using Horse.Engine.Core;
using SFML.Graphics;
using SFML.System;

namespace Horse.Server.Core
{
    /// <summary>
    /// Simple wrapper for screen items, to distingush 
    /// which ones to remove
    /// </summary>
    public class LobbyScreenItem : ScreenItem
    {
        public LobbyScreenItem(ref RenderWindow window, Text item, ScreenPositions position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public LobbyScreenItem(ref RenderWindow window, Sprite item, ScreenPositions position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public LobbyScreenItem(ref RenderWindow window, Shape item, ScreenPositions position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public LobbyScreenItem(ref RenderWindow window, Text item, Vector2f position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public LobbyScreenItem(ref RenderWindow window, Sprite item, Vector2f position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public LobbyScreenItem(ref RenderWindow window, Shape item, Vector2f position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public LobbyScreenItem(ref RenderWindow window) : base(ref window)
        {
        }
    }
}
