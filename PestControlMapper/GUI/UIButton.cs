using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GameManagers;
using PestControlEngine.GUI.Enum;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PestControlEngine.GUI
{
    public class UIButton : UIElement
    {
        public UITextElement ButtonTextBlock = new UITextElement();

        public UIRectangle ButtonRectangle = new UIRectangle();

        public UIRectangle ButtonInnerRectangle = new UIRectangle();


        // Outline colors
        public Color OutlineColor { get; set; } = Color.White;

        public Color OutlineHoverColor { get; set; } = new Color(170, 170, 170);

        public Color OutlinePressColor { get; set; } = new Color(110, 110, 110);


        // Inner rectangle colors
        public Color InnerColor { get; set; } = new Color(170, 170, 170, 160);

        public Color InnerHoverColor { get; set; } = new Color(150, 150, 150, 160);

        public Color InnerPressColor { get; set; } = new Color(90, 90, 90, 160);

        public Color TextColor { get; set; } = Color.White;


        public int LineThickness { get; set; } = 2;

        public Color _CurrentOutlineColor = Color.White;
        private Color _CurrentInnerColor = new Color(235, 235, 235);

        public bool IsHovered { get; set; } = false;

        public bool IsPressed { get; set; } = false;

        public bool ScaleToText { get; set; } = true;

        public UIButton(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;

            // Rectangle element
            AddChild(ButtonRectangle);

            ButtonRectangle.Filled = false;
            ButtonRectangle.StrokeSize = LineThickness;
            ButtonRectangle.FillParent = true;

            // Inner Rectangle Element
            AddChild(ButtonInnerRectangle);

            ButtonInnerRectangle.Filled = true;

            // Text element
            AddChild(ButtonTextBlock);

            ButtonTextBlock.HorizontalAlignment = EnumHorizontalAlignment.CENTER;
            ButtonTextBlock.VerticalAlignment = EnumVerticalAlignment.CENTER;

            // Default outline color set
            _CurrentOutlineColor = OutlineColor;

            // Event stuff
            MouseMovedEvent += UIButton_MouseMovedEvent;
            MouseEnterEvent += Button_MouseEnterEvent;
            MouseLeaveEvent += Button_MouseLeaveEvent;
            MouseClickedEvent += Button_MouseClickedEvent;
            MouseReleasedEvent += Button_MouseReleasedEvent;

            DynamicallyScale = true;
        }

        

        public override void Update(GameTime gameTime, GameInfo info)
        {
            // Resize to fit with text.
            if (ScaleToText)
            {
                Vector2 textBoxSize = ContentLoader.GetFont("engine_font").MeasureString(ButtonTextBlock.Text);

                Width = (int)textBoxSize.X;
                Height = (int)textBoxSize.Y;
            }

            if (IsHovered || IsPressed)
            {
                if (IsHovered)
                {
                    _CurrentOutlineColor = OutlineHoverColor;
                    _CurrentInnerColor = InnerHoverColor;
                }
                    

                if (IsPressed)
                {
                    _CurrentOutlineColor = OutlinePressColor;
                    _CurrentInnerColor = InnerPressColor;
                }
            }
            else
            {
                _CurrentOutlineColor = OutlineColor;
                _CurrentInnerColor = InnerColor;
            }

            ButtonRectangle.RectangleColor = _CurrentOutlineColor;
            ButtonInnerRectangle.RectangleColor = _CurrentInnerColor;

            base.Update(gameTime, info);

            // Inner rectangle
            ButtonInnerRectangle.Position = new Vector2(ButtonRectangle.StrokeSize, ButtonRectangle.StrokeSize);
            ButtonInnerRectangle.Width = Width - (ButtonRectangle.StrokeSize * 2);
            ButtonInnerRectangle.Height = Height - (ButtonRectangle.StrokeSize * 2);
        }

        public Color GetCurrentOutlineColor()
        {
            return _CurrentOutlineColor;
        }

        public void SetCurrentOutlineColor(Color color)
        {
            _CurrentOutlineColor = color;
        }

        public override void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            base.Draw(gameTime, device, spriteBatch, info);
        }

        private void Button_MouseEnterEvent(MouseEventArgs e)
        {
            IsHovered = true;
        }

        private void Button_MouseLeaveEvent(MouseEventArgs e)
        {
            IsHovered = false;
            IsPressed = false;
        }

        private void Button_MouseClickedEvent(MouseEventArgs e)
        {
            IsPressed = true;
        }

        private void Button_MouseReleasedEvent(MouseEventArgs e)
        {
            IsPressed = false;
        }

        private void UIButton_MouseMovedEvent(MouseEventArgs e)
        {
            
        }
    }
}
