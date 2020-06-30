using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Resource;
using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Objects.ECS.Components
{
    public class AnimationComponent : IEntityComponent
    {

        public GameObject Parent { get; set; }

        private Animation _CurrentAnimation { get; set; }

        /// <summary>
        /// Default animation to show in the map editor.
        /// </summary>
        public Animation DefaultAnimation { get; set; }

        [MapProperty("Animation", "The animation this component should use.")]
        [DefaultAnimationProperty]
        public string CurrentAnimationKey { get; set; } = Util.GetEngineNull();

        [MapProperty("PlayOnSpawn", "Whether or not the animation should automatically play on world spawn.", true)]
        public bool PlayOnSpawn { get; set; }

        /// <summary>
        /// For object information used in the map editor. If you want helper rectangles to show in the map editor, add them to this in a parameterless constructor.
        /// </summary>
        public List<HelperRectangle> HelperRectangles { get; set; } = new List<HelperRectangle>();
        public string Name { get; set; }

        private string SetAnimKey = "";


        public void Draw(GraphicsDevice device, GameTime gameTime, SpriteBatch spriteBatch, GameInfo info)
        {
            if (_CurrentAnimation != null && _CurrentAnimation.GetCurrentSprite() != null)
            {
                var orderedSprites = GetSpriteBoxes().OrderBy(f => f.Value.GetLayer()).ToList();

                foreach (KeyValuePair<string, Sprite> pair in orderedSprites)
                {
                    Sprite spriteBox = pair.Value;

                    if (spriteBatch != null && ContentLoader.GetTexture(spriteBox.GetTextureKey(), device) != null && spriteBox.Visible())
                    {
                        spriteBatch.Draw(ContentLoader.GetTexture(spriteBox.GetTextureKey(), device), new Rectangle((int)(spriteBox.GetPosition().X + Parent.GetPosition().X), (int)(spriteBox.GetPosition().Y + Parent.GetPosition().Y), spriteBox.GetWidth(), spriteBox.GetHeight()), spriteBox.GetSourceRectangle(), Color.White);
                    }
                }
            }
        }

        public void Update(GameTime gameTime, GameInfo info)
        {
            if (CurrentAnimationKey != SetAnimKey)
            {
                _CurrentAnimation = ContentLoader.GetAnimation(CurrentAnimationKey);
                SetAnimKey = CurrentAnimationKey;
            }

            if (_CurrentAnimation != null && !_CurrentAnimation.IsPlaying())
            {
                _CurrentAnimation.Play();
            }

            // Update animation
            if (_CurrentAnimation != null)
                _CurrentAnimation.Update(gameTime);
        }

        /// <summary>
        /// Converts a Dictionary of SpriteJsons to a Dictionary of Sprites.
        /// </summary>
        /// <param name="spriteDictionary">The Dictionary of SpriteJsons to convert</param>
        /// <returns></returns>
        public static Dictionary<string, Sprite> FromJsonSprites(Dictionary<string, SpriteJson> spriteDictionary)
        {
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

            foreach (KeyValuePair<string, SpriteJson> pair in spriteDictionary)
            {
                sprites.Add(pair.Key, Sprite.FromJsonElement(pair.Value));
            }

            return sprites;
        }

        public Dictionary<string, Sprite> GetSpriteBoxes()
        {
            if (_CurrentAnimation == null || _CurrentAnimation.GetCurrentSprite() == null)
                return new Dictionary<string, Sprite>();

            return FromJsonSprites(_CurrentAnimation.GetCurrentSprite());
        }
    }
}
