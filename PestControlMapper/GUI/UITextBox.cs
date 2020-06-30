// Class heavily based on https://github.com/craftworkgames/MonoGame.Extended/blob/develop/Source/MonoGame.Extended.Gui/Controls/TextBox.cs
// Credit where credit is due.

using Microsoft.Xna.Framework;
using MonoGame.Extended;
using PestControlEngine.GUI.Enum;
using PestControlEngine.Libs.Helpers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class UITextBox : UIElement
    {
        private string _Text;

        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChange();
                }
            }
        }

        public int CurrentSelectedChar { get; set; }

        public char? PasswordChar { get; set; }

        public UITextElement ButtonTextBlock = new UITextElement();

        public UIRectangle ButtonRectangle = new UIRectangle();

        public UIRectangle ButtonInnerRectangle = new UIRectangle();

        public Color InnerRectangleColor = new Color(170, 170, 170, 160);

        public Color OuterRectangleColor = Color.Gray;

        public Color TextColor = Color.Black;

        public int LineThickness = 2;

        public int ScrubberHeight = 20;

        public UILine TextScrubber;

        public UITextBox()
        {
            TextChangedEvent += TextChange;

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


            TextScrubber = new UILine
            {
                LineColor = Color.Red
            };

            AddChild(TextScrubber);


        }


        private void OnTextChange()
        {
            if (!string.IsNullOrEmpty(Text) && CurrentSelectedChar > Text.Length)
                CurrentSelectedChar = Text.Length;

            TextChangedEvent.Invoke();

            ButtonTextBlock.Text = Text;
        }

        public Size GetTextSize()
        {
            var font = ButtonTextBlock.Font;
            var stringSize = (Size)font.MeasureString(Text ?? string.Empty);

            return new Size(stringSize.Width,
                stringSize.Height < font.LineHeight ? font.LineHeight : stringSize.Height);
        }

        private int FindNearestGlyphIndex(Point position, GameInfo info)
        {
            float scaleX = 1;
            float scaleY = 1;

            // Change scale of recieved mouse input if GUI is using virtual scaling.
            if (info.guiManager.UseVirtualSize)
            {
                scaleX = (float)info.graphicsDevice.PresentationParameters.BackBufferWidth / (float)info.guiManager.VirtualViewWidth;
                scaleY = (float)info.graphicsDevice.PresentationParameters.BackBufferHeight / (float)info.guiManager.VirtualViewHeight;

                position.X = (int)((float)position.X / scaleX);
                position.Y = (int)((float)position.Y / scaleY);
            }

            var font = ButtonTextBlock.Font;
            var i = 0;

            foreach (var glyph in font.GetGlyphs(Text, new Point2(GetBoundingBox().X, GetBoundingBox().Y)))
            {
                var fontRegionWidth = glyph.FontRegion?.Width ?? 0;
                var glyphMiddle = (int)(glyph.Position.X + (((float)fontRegionWidth) / 2f));

                if (position.X >= glyphMiddle)
                {
                    i++;
                    continue;
                }

                return i;
            }

            return i;
        }

        public int GetScrubberPos(GameInfo info)
        {
            var font = ButtonTextBlock.Font;

            int pos = 0;
            int i = 0;

            foreach (var glyph in font.GetGlyphs(Text, new Point2(GetBoundingBox().X, GetBoundingBox().Y)))
            {
                pos += glyph.FontRegion.XAdvance;

                if (i == CurrentSelectedChar)
                {
                    return pos;
                }

                i++;
            }

            return 0;
        }

        public override void Update(GameTime gameTime, GameInfo info)
        {
            // Text scrubber offset
            TextScrubber.PointTwoOffset = new Vector2(0, ScrubberHeight);

            TextScrubber.Position = new Vector2(GetScrubberPos(info), TextScrubber.Position.Y);


            // Set rectangle colors
            ButtonInnerRectangle.RectangleColor = InnerRectangleColor;
            ButtonRectangle.RectangleColor = OuterRectangleColor;

            // Resize and fit inner rectangle
            ButtonInnerRectangle.Position = new Vector2(ButtonRectangle.StrokeSize, ButtonRectangle.StrokeSize);
            ButtonInnerRectangle.Width = Width - (ButtonRectangle.StrokeSize * 2);
            ButtonInnerRectangle.Height = Height - (ButtonRectangle.StrokeSize * 2);

            ButtonTextBlock.TextColor = TextColor;

            base.Update(gameTime, info);
        }

        // Events
        public delegate void TextChanged();
        public event TextChanged TextChangedEvent;


        // Event overrides
        private void TextChange()
        {

        }
    }
}
