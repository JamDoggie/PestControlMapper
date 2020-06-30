using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GameManagers;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Objects;
using PestControlEngine.Objects.ECS;
using PestControlEngine.Objects.ECS.Components;
using PestControlEngine.Resource;
using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Objects
{
    public class MapGameObject : GameObjectAnimated
    {
        public GameObjectInfo Info { get; set; }


        public MapGameObject(GameObjectInfo info)
        {
            Info = info;
        }

        public override void Update(GameTime gameTime, GameInfo gameInfo)
        {
            if (Properties != null)
            {
                if (Info.AnimationProperty != Util.GetEngineNull() && CurrentAnimationKey != Properties[Info.AnimationProperty].GetAsString())
                {
                    CurrentAnimationKey = Properties[Info.AnimationProperty].GetAsString();
                    CurrentAnimation = ContentLoader.GetAnimation(CurrentAnimationKey);

                    if (CurrentAnimation != null)
                        CurrentAnimation.Play();
                    else
                        CurrentAnimation = Info.DefaultAnimation;
                }
            }

            foreach (ComponentInfo component in Components)
            {
                component.Update(gameTime, gameInfo);
            }

            base.Update(gameTime, gameInfo);
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            base.Draw(device, spriteBatch, info);

            foreach(HelperRectangle rectangle in HelperRectangles)
            {
                GameObjectProperty posX = null;
                GameObjectProperty posY = null;
                GameObjectProperty width = null;
                GameObjectProperty height = null;

                if (info.objectManager.SelectionObject.Selection.Contains(this) && Properties.TryGetValue(rectangle.PosXProperty, out posX) && Properties.TryGetValue(rectangle.PosYProperty, out posY) && 
                    Properties.TryGetValue(rectangle.WidthProperty, out width) && Properties.TryGetValue(rectangle.HeightProperty, out height))
                {
                    ObjectManager.DrawRectangle(spriteBatch, device, new Rectangle((int)posX.GetAsInt32() + (int)GetPosition().X, (int)posY.GetAsInt32() + (int)GetPosition().Y, (int)width.GetAsInt32(), (int)height.GetAsInt32()), 1, new Color(242, 228, 121, 120));
                }
            }

            foreach(ComponentInfo component in Components)
            {
                component.Draw(device, info.gameTime, spriteBatch, info, this);
            }
        }
    }
}
