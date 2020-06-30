using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class UITextElement : UIElement
    {
        public string Text { get; set; } = "";

        public Color TextColor { get; set; } = Color.White;

        public BitmapFont Font { get; set; }

        private RasterizerState _RasterizerState = new RasterizerState() { ScissorTestEnable = true };

        public UITextElement()
        {
            BitmapFont font = ContentLoader.GetFont("engine_font");
            Font = font;
        }

        public UITextElement(BitmapFont font)
        {
            Font = font;
        }

        public override void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (Parent != null)
            {
                // This draws the text so that monogame doesn't draw it outside it's parent element. 
                // This does introduce an extra draw call.

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _RasterizerState);

                // Save copy of scissor rectangle for later
                Rectangle currentRect = spriteBatch.GraphicsDevice.ScissorRectangle;

                // Set scissor rectangle to the bounding box of the parent object. This makes it so the text is only rendered within the parent element and will cut off outside of it.
                if (Parent != null)
                    spriteBatch.GraphicsDevice.ScissorRectangle = Parent.GetBoundingBox();

                // Dynamically scale text to the parent so it fits within the parent.
                if (DynamicallyScale)
                {
                    float newScale = (float)Parent.Height / Font.MeasureString(Text).Height;

                    spriteBatch.DrawString(Font, Text, RenderPosition, TextColor, 0, new Vector2(), new Vector2(newScale, newScale), SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.DrawString(Font, Text, RenderPosition, TextColor, 0, new Vector2(), new Vector2(1f, 1f), SpriteEffects.None, 0);
                }

                // Restore scissor rectangle
                spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;

                info.guiManager.SwitchToDefaultSpriteBatch(spriteBatch);
            }
            else
            {
                spriteBatch.DrawString(Font, Text, RenderPosition, TextColor, 0, new Vector2(), new Vector2(1f, 1f), SpriteEffects.None, 0);
            }

            base.Draw(gameTime, device, spriteBatch, info);
        }

        public override void Update(GameTime gameTime, GameInfo info)
        {
            Width = (int)Font.MeasureString(Text).Width;
            Height = (int)Font.MeasureString(Text).Height;

            base.Update(gameTime, info);
        }
    }
}
