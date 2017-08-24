﻿using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Horse.Engine.Core
{
    public abstract class Screen : Drawable
    {
        protected List<ScreenItem> ScreenItems;
        protected Color BgColor;
        protected Screen()
        {
            ScreenItems = new List<ScreenItem>();
        }

        protected Screen(ref RenderWindow window) : base(ref window)
        {
            ScreenItems = new List<ScreenItem>();
            WinInstance.KeyPressed += OnKeyPress;
            WinInstance.KeyReleased += OnKeyRelease;
        }

        public void RemoveWindowKeyEventHandler()
        {
            if (WinInstance == null)
                return;
            WinInstance.KeyReleased -= OnKeyRelease;
            WinInstance.KeyPressed -= OnKeyPress;
        }

        public void AddScreenItem(ScreenItem item)
        {
            if(ScreenItems == null)
            {
                ScreenItems = new List<ScreenItem>();
                ScreenItems.Add(item);
            }
            ScreenItems.Add(item);
        }

        public abstract void OnKeyPress(object sender, KeyEventArgs keyEventArgs);

        public abstract void OnKeyRelease(object sender, KeyEventArgs keyEventArgs);

        public override void Draw()
        {
            if (ScreenItems == null || ScreenItems.Count == 0)
                return;
            foreach (var item in ScreenItems)
                item.Draw();
        }

        public void AddWindowKeyEventHandler()
        {
            WinInstance.KeyPressed += OnKeyPress;
        }
    }
}