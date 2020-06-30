using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GameManagers;
using PestControlEngine.Libs.Helpers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class UIRectangle : UIElement
    {
        public Color RectangleColor { get; set; } = Color.White;

        public int StrokeSize { get; set; } = 2;

        public bool Filled { get; set; } = true;

        public override void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (Filled)
            {
                ObjectManager.DrawFilledRectangle(spriteBatch, device, GetBoundingBox(), RectangleColor);
            }
            else
            {
                ObjectManager.DrawRectangle(spriteBatch, device, GetBoundingBox(), StrokeSize, RectangleColor);
            }

            base.Draw(gameTime, device, spriteBatch, info);
        }
    }
}
