using Horse.Engine.Core;
using SFML.Graphics;
using System;
using SFML.System;
namespace Horse.Server.Games.ColorMePretty
{
    public class CMPScreenItem : ScreenItem
    {
        public CMPScreenItem(ref RenderWindow window, Text item, ScreenPositions position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public CMPScreenItem(ref RenderWindow window, Sprite item, ScreenPositions position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public CMPScreenItem(ref RenderWindow window, Shape item, ScreenPositions position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public CMPScreenItem(ref RenderWindow window, Text item, Vector2f position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public CMPScreenItem(ref RenderWindow window, Sprite item, Vector2f position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public CMPScreenItem(ref RenderWindow window, Shape item, Vector2f position, Func<int> func) : base(ref window, item, position, func)
        {
        }

        public CMPScreenItem(ref RenderWindow window) : base(ref window)
        {
        }
    }
}
