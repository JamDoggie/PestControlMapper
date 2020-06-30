using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Objects.ECS
{
    public interface IEntityComponent
    {
        GameObject Parent { get; set; }

        string Name { get; set; }

        Animation DefaultAnimation { get; set; }

        List<HelperRectangle> HelperRectangles { get; set; }

        void Update(GameTime gameTime, GameInfo info);
        void Draw(GraphicsDevice device, GameTime gameTime, SpriteBatch spriteBatch, GameInfo info);


    }
}
