using System;
using SFML.Graphics;
using SFML.System;

namespace Horse.Engine.Core
{
    public  class ScreenItem : Drawable
    {
        public enum ScreenPositions
        {
            TopLeft, 
            Top,
            TopRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            Bottom,
            BottomRight
        }

        private Text _textItem;
        private Sprite _spriteItem;
        private Shape _shapeItem;
        private Func<int> _action;

        public ScreenItem(ref RenderWindow window, Text item, ScreenPositions position, Func<int> func) : base(ref window)
        {
            _textItem = item;
            _action = func;
            SetPosition(position);
        }

        public ScreenItem(ref RenderWindow window, Sprite item, ScreenPositions position, Func<int> func) : base(ref window)
        {
            _spriteItem = item;
            _action = func;
            SetPosition(position);
        }

        public ScreenItem(ref RenderWindow window, Shape item, ScreenPositions position, Func<int> func) : base(ref window)
        {
            _shapeItem = item;
            _action = func;
            SetPosition(position);
        }

        public ScreenItem(ref RenderWindow window) : base(ref window)
        {
        }

        public void SetFont(Text text) => _textItem = text;
        public void SetSprite(Sprite sprite) => _spriteItem = sprite;
        public void SetShape(Shape shape) => _shapeItem = shape;

        public void SetPosition(ScreenPositions position)
        {
            var marginX = 32.0f;
            var marginY = 32.0f;
            var centerX = WinInstance.Size.X / 2.0f;
            var centerY = WinInstance.Size.Y / 2.0f;
            switch (position)
            {
                case ScreenPositions.TopLeft:
                    var topLeft = new Vector2f(marginX, marginY);
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = topLeft;
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = topLeft;
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = topLeft;
                    }
                    break;
                case ScreenPositions.Top:
                    var top = new Vector2f(centerX, marginY);
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = top;
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = top;
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = top;
                    }
                    break;
                case ScreenPositions.TopRight:
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X-_shapeItem.Texture.Size.X-marginX,marginY);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(WinInstance.Size.X-_spriteItem.Texture.Size.X-marginX,marginY);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(WinInstance.Size.X - _textItem.CharacterSize - marginX, marginY);
                    }
                    break;
                case ScreenPositions.CenterLeft:
                    var centerLeft = new Vector2f(marginX, centerY);
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = centerLeft;
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = centerLeft;
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = centerLeft;
                    }
                    break;
                case ScreenPositions.Center:
                    var center = new Vector2f(centerX, centerY);
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = center;
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = center;
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = center;
                    }
                    break;
                case ScreenPositions.CenterRight:
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X-marginX-_shapeItem.Texture.Size.X,centerY);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(WinInstance.Size.X - marginX - _spriteItem.Texture.Size.X, centerY);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(WinInstance.Size.X - marginX - _textItem.CharacterSize, centerY);
                    }
                    break;
                case ScreenPositions.BottomLeft:
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = new Vector2f(marginX, WinInstance.Size.Y-marginY-_shapeItem.Texture.Size.Y);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(marginX, WinInstance.Size.Y - marginY - _spriteItem.Texture.Size.Y);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(marginX, WinInstance.Size.Y - marginY - _textItem.CharacterSize);
                    }
                    break;
                case ScreenPositions.Bottom:
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = new Vector2f(centerX, WinInstance.Size.Y - marginY - _shapeItem.Texture.Size.Y);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(centerX, WinInstance.Size.Y - marginY - _spriteItem.Texture.Size.Y);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(centerX, WinInstance.Size.Y - marginY - _textItem.CharacterSize);
                    }
                    break;
                case ScreenPositions.BottomRight:
                    if (_shapeItem != null)
                    {
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X - marginX - _shapeItem.Texture.Size.X, WinInstance.Size.Y - marginY - _shapeItem.Texture.Size.Y);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(WinInstance.Size.X - marginX - _spriteItem.Texture.Size.X, WinInstance.Size.Y - marginY - _spriteItem.Texture.Size.Y);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(WinInstance.Size.X - marginX -_textItem.CharacterSize, WinInstance.Size.Y - marginY - _textItem.CharacterSize);
                    }
                    break;
            }
        }

        public int DoAction()
        {
            return _action == null ? -1 : _action();
        }

        public override void Draw()
        {
            if (_shapeItem != null)
            {
                WinInstance.Draw(_shapeItem);
            }
            if (_spriteItem != null)
            {
                WinInstance.Draw(_spriteItem);
            }
            if (_textItem != null)
            {
                WinInstance.Draw(_textItem);
            }
        }
    }
}
