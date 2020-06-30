using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Mapping.Enums;
using PestControlEngine.Objects.ECS;
using PestControlEngine.Resource;
using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PestControlEngine.Objects
{
    public class GameObject
    {
        // Variables
        private Vector2 _Position = new Vector2();

        public string Name = "";

        private List<GameObject> _children = new List<GameObject>();

        private GameObject _parent = null;

        public Rectangle BoundingBox { get; set; } = new Rectangle();

        public List<IEntityComponent> GameComponents { get; set; } = new List<IEntityComponent>();

        public Animation CurrentAnimation { get; set; } = new Animation();

        public float Layer { get; set; } = 0f;

        public bool Visible { get; set; } = true;

        public List<HelperRectangle> HelperRectangles = new List<HelperRectangle>();

        public List<ComponentInfo> Components { get; set; } = new List<ComponentInfo>();

        // These properties are purely for exporting a PCOI file for mapping. Make sure the key in the dictionary is the name of the variable here. 
        // They're pretty much just outlets to expose variables to the map editor.
        public Dictionary<string, GameObjectProperty> Properties = new Dictionary<string, GameObjectProperty>();

        public GameObject()
        {
            /// Properties for use in the map editor

            // Object Name
            var nameProperty = new GameObjectProperty("Name", PropertyType.STRING);
            Properties.Add("Name", nameProperty);
        }

        /// <summary>
        /// Overridable, allows us to control how to draw this object directly.
        /// Not recommended if you can achieve your goal with more higher level methods.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            // Draw children
            foreach (GameObject d in _children)
            {
                d.Draw(device, spriteBatch, info);
            }
        }

        public virtual void Update(GameTime gameTime, GameInfo gameInfo)
        {
            // Update children
            foreach (GameObject d in _children)
            {
                d.Update(gameTime, gameInfo);
            }
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

        public virtual Rectangle GetBoundingBox()
        {
            if (CurrentAnimation == null || CurrentAnimation.GetCurrentSprite() == null)
            {
                return BoundingBox;
            }
            else
            {
                int lowestX = 0;
                int lowestY = 0;
                int highestX = 0;
                int highestY = 0;

                foreach (KeyValuePair<string, Sprite> pair in FromJsonSprites(CurrentAnimation.GetCurrentSprite()))
                {
                    if (pair.Value.GetPosition().X < lowestX)
                        lowestX = (int)pair.Value.GetPosition().X;

                    if (pair.Value.GetPosition().Y < lowestY)
                        lowestY = (int)pair.Value.GetPosition().Y;

                    if (pair.Value.GetRectangle().Right > highestX)
                        highestX = pair.Value.GetRectangle().Right;

                    if (pair.Value.GetRectangle().Bottom > highestY)
                        highestY = pair.Value.GetRectangle().Bottom;
                }

                return new Rectangle(lowestX + (int)GetPosition().X, lowestY + (int)GetPosition().Y, highestX - lowestX, highestY - lowestY);
            }

        }

        public virtual Vector2 GetPosition()
        {
            if (_parent != null)
            {
                return new Vector2(_Position.X + GetParent().GetPosition().X, _Position.Y + GetParent().GetPosition().Y);
            }
            else
            {
                return _Position;
            }

        }

        public virtual Vector2 GetRawPosition()
        {
            return _Position;
        }

        public virtual void SetPosition(Vector2 position)
        {
            _Position = position;
        }

        public virtual void AddChild(GameObject drawable)
        {
            if (drawable == null)
                return;

            drawable._parent = this;
            _children.Add(drawable);
        }

        public virtual void RemoveChild(GameObject drawable)
        {
            if (drawable == null)
                return;

            if (drawable._parent == this)
            {
                _children.Remove(drawable);
                drawable._parent = null;
            }

        }

        public virtual List<GameObject> GetChildren()
        {
            return _children;
        }

        /// <summary>
        /// Sets the list of children in this drawable. NOTE: if you wish to add or remove children, it's recommended
        /// to use RemoveChild and AddChild.
        /// </summary>
        /// <param name="list"></param>
        public virtual void SetChildren(List<GameObject> list)
        {
            _children = list;
        }

        public virtual GameObject GetParent()
        {
            return _parent;
        }

        public virtual void SetParent(GameObject parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Finds component in ECS with the given type passed as generic "T" and returns if found.
        /// </summary>
        /// <typeparam name="T">Component type to find in component list.</typeparam>
        /// <returns>IEntityComponent</returns>
        public virtual IEntityComponent GetComponent<T>()
        {
            foreach (IEntityComponent component in GameComponents)
            {
                if (component is T)
                {
                    return component;
                }
            }

            return default;
        }

        public void MouseMoved()
        {
        }

        public void MouseClicked()
        {

        }
    }
}
