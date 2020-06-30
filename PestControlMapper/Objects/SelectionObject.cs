using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GameManagers;
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
    public class SelectionObject : GameObject
    {
        public List<GameObject> Selection { get; set; } = new List<GameObject>();

        public SelectionObject()
        {
            Layer = 100000;
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (Selection != null)
            {
                foreach(GameObject obj in Selection)
                {
                    ObjectManager.DrawRectangle(spriteBatch, device, new Rectangle((int)obj.GetPosition().X, (int)obj.GetPosition().Y, obj.GetBoundingBox().Width, obj.GetBoundingBox().Height), 1, new Color(255, 255, 255, 70));
                }
            }

            base.Draw(device, spriteBatch, info);
        }
    }
}
