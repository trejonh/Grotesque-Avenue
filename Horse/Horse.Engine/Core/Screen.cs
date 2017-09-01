using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Horse.Engine.Core
{
    /// <summary>
    /// A container of drawables to render onto a window
    /// </summary>
    public abstract class Screen : Drawable
    {
        /// <summary>
        /// Items to draw on a window
        /// </summary>
        protected List<ScreenItem> ScreenItems;
        
        /// <summary>
        /// The background color of the screen
        /// </summary>
        protected Color BgColor;

        /// <summary>
        /// Creates a blank screen
        /// </summary>
        protected Screen()
        {
            ScreenItems = new List<ScreenItem>();
        }

        /// <summary>
        /// Creates a blank screen with a window to draw on
        /// </summary>
        /// <param name="window">Window to draw on</param>
        protected Screen(ref RenderWindow window) : base(ref window)
        {
            ScreenItems = new List<ScreenItem>();
            WinInstance.KeyPressed += OnKeyPress;
            WinInstance.KeyReleased += OnKeyRelease;
        }

        /// <summary>
        /// Remove key event handlers from the window
        /// </summary>
        public void RemoveWindowKeyEventHandler()
        {
            if (WinInstance == null)
                return;
            WinInstance.KeyReleased -= OnKeyRelease;
            WinInstance.KeyPressed -= OnKeyPress;
        }

        /// <summary>
        /// Add a screen item
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddScreenItem(ScreenItem item)
        {
            if(ScreenItems == null)
            {
                ScreenItems = new List<ScreenItem> {item};
                return;
            }
            ScreenItems.Add(item);
        }

        /// <summary>
        /// Key press handler for the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyEventArgs"></param>
        public abstract void OnKeyPress(object sender, KeyEventArgs keyEventArgs);

        /// <summary>
        /// Key released handler for the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyEventArgs"></param>
        public abstract void OnKeyRelease(object sender, KeyEventArgs keyEventArgs);

        /// <summary>
        /// Draw all screen items
        /// </summary>
        public override void Draw()
        {
            if (ScreenItems == null || ScreenItems.Count == 0)
                return;
            foreach (var item in ScreenItems)
                item.Draw();
        }

        /// <summary>
        /// Add key event handlers to the window
        /// </summary>
        public void AddWindowKeyEventHandler()
        {
            WinInstance.KeyPressed += OnKeyPress;
            WinInstance.KeyReleased += OnKeyRelease;
        }
    }
}
