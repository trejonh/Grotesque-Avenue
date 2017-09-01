using System;
using SFML.Graphics;
using SFML.System;
using Horse.Engine.Extensions;

namespace Horse.Engine.Core
{
    /// <summary>
    /// An item to draw onto a screen
    /// </summary>
    public  class ScreenItem : Drawable
    {
        /// <summary>
        /// Possible screen position defaults
        /// </summary>
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

        /*Possible drawables on screen*/
        private Text _textItem;
        private Sprite _spriteItem;
        private Shape _shapeItem;

        /// <summary>
        /// Function to invoke if item is clicked
        /// </summary>
        private readonly Func<int> _action;

        /// <summary>
        /// The position of the screen item
        /// </summary>
        public Vector2f Position => GetPosition();

        /// <summary>
        /// Create a new screen item
        /// </summary>
        /// <param name="window">The window to draw to</param>
        /// <param name="item">The item to draw</param>
        /// <param name="position">The initial position on the screen</param>
        /// <param name="func">The function to invoke if the item is clicked</param>
        public ScreenItem(ref RenderWindow window, Text item, ScreenPositions position, Func<int> func) : base(ref window)
        {
            _textItem = item;
            _action = func;
            SetPosition(position);
        }


        /// <summary>
        /// Create a new screen item
        /// </summary>
        /// <param name="window">The window to draw to</param>
        /// <param name="item">The item to draw</param>
        /// <param name="position">The initial position on the screen</param>
        /// <param name="func">The function to invoke if the item is clicked</param>
        public ScreenItem(ref RenderWindow window, Sprite item, ScreenPositions position, Func<int> func) : base(ref window)
        {
            _spriteItem = item;
            _action = func;
            SetPosition(position);
        }


        /// <summary>
        /// Create a new screen item
        /// </summary>
        /// <param name="window">The window to draw to</param>
        /// <param name="item">The item to draw</param>
        /// <param name="position">The initial position on the screen</param>
        /// <param name="func">The function to invoke if the item is clicked</param>
        public ScreenItem(ref RenderWindow window, Shape item, ScreenPositions position, Func<int> func) : base(ref window)
        {
            _shapeItem = item;
            _action = func;
            SetPosition(position);
        }


        /// <summary>
        /// Create a new screen item
        /// </summary>
        /// <param name="window">The window to draw to</param>
        /// <param name="item">The item to draw</param>
        /// <param name="position">The initial position on the screen</param>
        /// <param name="func">The function to invoke if the item is clicked</param>
        public ScreenItem(ref RenderWindow window, Text item, Vector2f position, Func<int> func) : base(ref window)
        {
            _textItem = item;
            _action = func;
            SetPosition(position);
        }


        /// <summary>
        /// Create a new screen item
        /// </summary>
        /// <param name="window">The window to draw to</param>
        /// <param name="item">The item to draw</param>
        /// <param name="position">The initial position on the screen</param>
        /// <param name="func">The function to invoke if the item is clicked</param>
        public ScreenItem(ref RenderWindow window, Sprite item, Vector2f position, Func<int> func) : base(ref window)
        {
            _spriteItem = item;
            _action = func;
            SetPosition(position);
        }


        /// <summary>
        /// Create a new screen item
        /// </summary>
        /// <param name="window">The window to draw to</param>
        /// <param name="item">The item to draw</param>
        /// <param name="position">The initial position on the screen</param>
        /// <param name="func">The function to invoke if the item is clicked</param>
        public ScreenItem(ref RenderWindow window, Shape item, Vector2f position, Func<int> func) : base(ref window)
        {
            _shapeItem = item;
            _action = func;
            SetPosition(position);
        }

        /// <summary>
        /// Creates a blank screen item
        /// </summary>
        /// <param name="window">The window to draw on</param>
        public ScreenItem(ref RenderWindow window) : base(ref window)
        {
        }

        /// <summary>
        /// Sets the text of the screen item
        /// </summary>
        /// <param name="text">The new text</param>
        public void SetText(Text text) => _textItem = text;

        /// <summary>
        /// Sets the sprite of the screen item
        /// </summary>
        /// <param name="sprite">The new sprite</param>
        public void SetSprite(Sprite sprite) => _spriteItem = sprite;

        /// <summary>
        /// Set the screen item shape
        /// </summary>
        /// <param name="shape">The new shape</param>
        public void SetShape(Shape shape) => _shapeItem = shape;

        /// <summary>
        /// Set the position of the screen item
        /// </summary>
        /// <param name="position">The new position</param>
        public void SetPosition(Vector2f position)
        {
            if (_shapeItem != null)
            {
                _shapeItem.Position = position;
            }
            if (_spriteItem != null)
            {
                _spriteItem.Position = position;
            }
            if (_textItem != null)
            {
                _textItem.Position = position;
            }

        }

        /// <summary>
        /// Set the position of the screen item
        /// </summary>
        /// <param name="position">The new position</param>
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
                    if (_shapeItem != null)
                    {
                        var size = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                            size = ((CircleShape)_shapeItem).Radius * 2;
                        if (_shapeItem.GetType() == typeof(RectangleShape))
                            size = ((RectangleShape)_shapeItem).Size.X;
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                            size = ((RoundedRectangle)_shapeItem).GetSize().X;
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X - size - marginX, marginY);
                        _shapeItem.Position = new Vector2f(centerX -(size*2),marginY);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(centerX-(_spriteItem.Texture.Size.X*2.5f),marginY);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(centerX - (_textItem.CharacterSize * 2.5f), marginY);
                    }
                    break;
                case ScreenPositions.TopRight:
                    if (_shapeItem != null)
                    {
                        var size = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                            size = ((CircleShape)_shapeItem).Radius * 2;
                        if (_shapeItem.GetType() == typeof(RectangleShape))
                            size = ((RectangleShape)_shapeItem).Size.X;
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                            size = ((RoundedRectangle)_shapeItem).GetSize().X;
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X-size-marginX,marginY);
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
                    if (_shapeItem != null)
                    {
                        var size = 0.0f;
                        var sizeY = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                        {
                            size = ((CircleShape)_shapeItem).Radius * 2;
                            sizeY = size;
                        }
                        if (_shapeItem.GetType() == typeof(RectangleShape))
                        {
                            size = ((RectangleShape)_shapeItem).Size.X;
                            sizeY = ((RectangleShape)_shapeItem).Size.Y;
                        }
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                        {
                            size = ((RoundedRectangle)_shapeItem).GetSize().X;
                            sizeY = ((RoundedRectangle)_shapeItem).GetSize().Y;
                        }
                        _shapeItem.Position = new Vector2f(centerX - (size*2.0f), centerY - sizeY / 2.0f);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(centerX - (_spriteItem.Texture.Size.X * 2.5f), centerY - _spriteItem.Texture.Size.Y / 2.0f);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(centerX - (_textItem.CharacterSize *  2.5f) , centerY - _textItem.CharacterSize / 2.0f);
                    }
                    break;
                case ScreenPositions.CenterRight:
                    if (_shapeItem != null)
                    {
                        var size = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                            size = ((CircleShape)_shapeItem).Radius * 2;
                        if (_shapeItem.GetType() == typeof(RectangleShape))
                            size = ((RectangleShape)_shapeItem).Size.X;
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                            size = ((RoundedRectangle)_shapeItem).GetSize().X;
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X-marginX-size,centerY);
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

                        var size = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                            size = ((CircleShape)_shapeItem).Radius * 2;
                        if (_shapeItem.GetType() == typeof(RectangleShape))
                            size = ((RectangleShape)_shapeItem).Size.Y;
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                            size = ((RoundedRectangle)_shapeItem).GetSize().Y;
                        _shapeItem.Position = new Vector2f(marginX, WinInstance.Size.Y-marginY-size);
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
                        var size = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                            size = ((CircleShape)_shapeItem).Radius * 2;
                        if (_shapeItem.GetType() == typeof(RectangleShape))
                            size = ((RectangleShape)_shapeItem).Size.Y;
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                            size = ((RoundedRectangle)_shapeItem).GetSize().Y;
                        _shapeItem.Position = new Vector2f(centerX - (size*2.0f), WinInstance.Size.Y - marginY - size);
                    }
                    if (_spriteItem != null)
                    {
                        _spriteItem.Position = new Vector2f(centerX - (_spriteItem.Texture.Size.X * 2.5f), WinInstance.Size.Y - marginY - _spriteItem.Texture.Size.Y);
                    }
                    if (_textItem != null)
                    {
                        _textItem.Position = new Vector2f(centerX - (_textItem.CharacterSize * 2.5f), WinInstance.Size.Y - marginY - _textItem.CharacterSize);
                    }
                    break;
                case ScreenPositions.BottomRight:
                    if (_shapeItem != null)
                    {
                        var size = 0.0f;
                        var sizeY = 0.0f;
                        if (_shapeItem.GetType() == typeof(CircleShape))
                        {
                            size = ((CircleShape)_shapeItem).Radius * 2;
                            sizeY = size;
                        }
                        if (_shapeItem.GetType() == typeof(RectangleShape)) {
                            size = ((RectangleShape)_shapeItem).Size.X;
                            sizeY = ((RectangleShape)_shapeItem).Size.Y;
                        }
                        if (_shapeItem.GetType() == typeof(RoundedRectangle))
                        {
                            size = ((RoundedRectangle)_shapeItem).GetSize().X;
                            sizeY = ((RoundedRectangle)_shapeItem).GetSize().Y;
                        }
                        _shapeItem.Position = new Vector2f(WinInstance.Size.X - marginX - size, WinInstance.Size.Y - marginY - sizeY);
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

        /// <summary>
        /// Gets the position of the screen item
        /// </summary>
        /// <returns>Current position</returns>
        private Vector2f GetPosition()
        {

            if (_shapeItem != null)
            {
                return _shapeItem.Position;
            }
            if (_spriteItem != null)
            {
                return _spriteItem.Position;
            }
            if (_textItem != null)
            {
               return _textItem.Position;
            }
            return new Vector2f();
        }
        /// <summary>
        /// Invoke the action meant for the screen item
        /// </summary>
        /// <returns>-1 if the function is null otherwise the return value of the function</returns>
        public int DoAction()
        {
            return _action == null ? -1 : _action();
        }

        /// <summary>
        /// Draw the screen item
        /// </summary>
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

        /// <summary>
        /// Determine if the screen item is of type Shape
        /// </summary>
        /// <returns>True if the screen item is a shape</returns>
        public bool IsShape()
        {
            return _shapeItem != null;
        }
    }
}
