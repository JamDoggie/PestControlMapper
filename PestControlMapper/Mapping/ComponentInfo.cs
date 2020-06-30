using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Objects;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PestControlMapper.Mapping
{
    public class ComponentInfo
    {
        public string RealName { get; set; }

        public string ClassName { get; set; }

        public Animation DefaultAnimation { get; set; }

        public string AnimationProperty { get; set; }

        public List<HelperRectangle> HelperRectangles { get; set; }

        public Dictionary<string, ComponentProperty> Properties { get; set; }


        
        private string SetAnimKey;

        [JsonIgnore]
        private Animation _CurrentAnimation { get; set; }

        public ComponentInfo(string name, string classname, Animation defaultAnimation, string animationproperty, List<HelperRectangle> helperRectangles, Dictionary<string, ComponentProperty> properties)
        {
            RealName = name;
            ClassName = classname;
            DefaultAnimation = defaultAnimation;
            AnimationProperty = animationproperty;

            Properties = properties;
            HelperRectangles = helperRectangles;

            _CurrentAnimation = new Animation();
            SetAnimKey = "";
        }

        public ComponentInfo Copy()
        {
            List<HelperRectangle> newRectangles = new List<HelperRectangle>();

            foreach(HelperRectangle rect in HelperRectangles)
            {
                newRectangles.Add(rect.Copy());
            }

            Dictionary<string, ComponentProperty> newProperties = new Dictionary<string, ComponentProperty>();

            foreach (KeyValuePair<string, ComponentProperty> pair in Properties)
            {
                newProperties.Add((string)pair.Key.Clone(), pair.Value.Copy());
            }

            var newComponent = new ComponentInfo((string)RealName.Clone(), (string)ClassName.Clone(), DefaultAnimation.Copy(), (string)AnimationProperty.Clone(), newRectangles, newProperties);
            newComponent.SetAnimKey = (string)SetAnimKey.Clone();
            newComponent._CurrentAnimation = _CurrentAnimation;

            return newComponent;
        }

        public void Draw(GraphicsDevice device, GameTime gameTime, SpriteBatch spriteBatch, GameInfo info, GameObject Parent)
        {
            if (_CurrentAnimation != null && _CurrentAnimation.GetCurrentSprite() != null && Parent.Visible)
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
            string CurrentAnimationKey = GetPropertyByRealName("Animation").GetAsString();

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

        public ComponentProperty GetPropertyByRealName(string name)
        {
            foreach(KeyValuePair<string, ComponentProperty> propertyPair in Properties)
            {
                if (propertyPair.Value.RealName == name)
                {
                    return propertyPair.Value;
                }
            }

            return null;
        }
    }
}
