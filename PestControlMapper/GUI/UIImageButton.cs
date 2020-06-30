using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GUI;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.GUI
{
    public class UIImageButton : UIButton
    {
        public string Image { get; set; }

        public bool IsOption { get; set; } = false;

        public bool IsChosenOption { get; set; } = false;

        public UIImageButton(Vector2 position, int width, int height) : base(position, width, height)
        {
            InnerColor = Color.Transparent;
            InnerPressColor = Color.Transparent;
            InnerHoverColor = Color.Transparent;

            ScaleToText = false;
            OutlineColor = new Color(150, 150, 150, 100);
            OutlineHoverColor = new Color(100, 100, 100, 100);
        }

        public override void Update(GameTime gameTime, GameInfo info)
        {
            base.Update(gameTime, info);

            if (IsOption)
            {
                if (!IsChosenOption)
                {
                    ButtonRectangle.RectangleColor = Color.Transparent;
                }
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            Texture2D texture = ContentLoader.GetTexture(Image, device);

            if (texture != null)
            {
                spriteBatch.Draw(texture, new Rectangle((int)Position.X  + (Width / 2 - texture.Width / 2), (int)Position.Y + (Height / 2 - texture.Height / 2), texture.Width, texture.Height), texture.Bounds, Color.White);
            }
                

            base.Draw(gameTime, device, spriteBatch, info);
        }
    }
}
