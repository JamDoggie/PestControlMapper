using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Objects;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Objects
{
    public class PreviewObject : GameObjectAnimated
    {
        public PreviewObject()
        {

        }

        public bool Visible { get; set; } = true;

        public GameObjectInfo Info { get; set; }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (CurrentAnimation != null && CurrentAnimation.GetCurrentSprite() != null && Visible)
            {
                var orderedSprites = GetSpriteBoxes().OrderBy(f => f.Value.GetLayer()).ToList();

                foreach (KeyValuePair<string, Sprite> pair in orderedSprites)
                {
                    
                    Sprite spriteBox = pair.Value;

                    if (spriteBatch != null && ContentLoader.GetTexture(spriteBox.GetTextureKey(), device) != null && spriteBox.Visible())
                    {
                        spriteBatch.Draw(ContentLoader.GetTexture(spriteBox.GetTextureKey(), device), new Rectangle((int)(spriteBox.GetPosition().X + GetPosition().X), (int)(spriteBox.GetPosition().Y + GetPosition().Y), spriteBox.GetWidth(), spriteBox.GetHeight()), spriteBox.GetSourceRectangle(), new Color(255, 255, 255, 80));
                    }
                }
            }

            // Draw children
            foreach (GameObject d in GetChildren())
            {
                d.Draw(device, spriteBatch, info);
            }
        }
    }
}
