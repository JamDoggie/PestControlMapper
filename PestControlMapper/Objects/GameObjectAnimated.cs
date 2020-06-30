using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Mapping.Enums;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Objects
{
    public class GameObjectAnimated : GameObject
    {
        public GameObjectAnimated()
        {
            /// Properties for use in the map editor

            // Object Name
            var animProperty = new GameObjectProperty("Animation", PropertyType.STRING);
            Properties.Add("Animation", animProperty);
        }

        public string CurrentAnimationKey { get; set; } = "";

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
                        spriteBatch.Draw(ContentLoader.GetTexture(spriteBox.GetTextureKey(), device), new Rectangle((int)(spriteBox.GetPosition().X + GetPosition().X), (int)(spriteBox.GetPosition().Y + GetPosition().Y), spriteBox.GetWidth(), spriteBox.GetHeight()), spriteBox.GetSourceRectangle(), Color.White);
                    }
                }
            }

            base.Draw(device, spriteBatch, info);
        }

        public override void Update(GameTime gameTime, GameInfo gameInfo)
        {
            // Update animation
            if (CurrentAnimation != null)
                CurrentAnimation.Update(gameTime);

            base.Update(gameTime, gameInfo);
        }

        public virtual Dictionary<string, Sprite> GetSpriteBoxes()
        {
            if (CurrentAnimation == null || CurrentAnimation.GetCurrentSprite() == null)
                return new Dictionary<string, Sprite>();

            return FromJsonSprites(CurrentAnimation.GetCurrentSprite());
        }
    }
}
