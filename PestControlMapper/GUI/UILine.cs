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
    public class UILine : UIElement
    {
        public Vector2 PointOneOffset { get; set; } = new Vector2(0, 0);

        public Vector2 PointTwoOffset { get; set; } = new Vector2(0, 0);

        public Color LineColor { get; set; } = Color.White;

        public float LineThickness { get; set; } = 1;

        public override void Update(GameTime gameTime, GameInfo info)
        {


            base.Update(gameTime, info);
        }

        public override void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            ExtendedSpriteBatch.DrawLine(spriteBatch, ObjectManager.GetWhitePixel(info.graphicsDevice), RenderPosition + PointOneOffset, RenderPosition + PointTwoOffset, LineColor, LineThickness);

            base.Draw(gameTime, device, spriteBatch, info);
        }
    }
}
