using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class UIGrid : UIElement
    {
        public List<GridCell> Cells = new List<GridCell>();

        public void AddCell(UIElement element, double xPos, double yPos, double Width, double Height)
        {
            Cells.Add(new GridCell()
            {
                posX = xPos,
                posY = yPos,
                width = Width,
                height = Height,
                Child = element
            });

            AddChild(element);
        }

        public void RemoveCell(UIElement element)
        {
            GridCell toRemove = null;

            foreach(GridCell cell in Cells)
            {
                if (cell.Child == element)
                {
                    toRemove = cell;
                }
            }

            Cells.Remove(toRemove);

            RemoveChild(element);
        }

        public override void Update(GameTime gameTime, GameInfo info)
        {
            if (ParentScreen == null)
            {
                Console.WriteLine("no parent");
            }

            if (Parent == null)
            {
                Width = (int)info.Resolution.X + 1;
                Height = (int)info.Resolution.Y + 1;
            }
            else
            {
                Width = Parent.Width;
                Height = Parent.Height;
            }

            foreach (GridCell cell in Cells)
            {
                if (cell.Child != null)
                {
                    float parentWidth;
                    float parentHeight;

                    if (cell.Child.Parent != null)
                    {
                        parentWidth = cell.Child.Parent.Width;
                        parentHeight = cell.Child.Parent.Height;
                    }
                    else
                    {
                        parentWidth = info.Resolution.X + 1;
                        parentHeight = info.Resolution.Y + 1;
                    }

                    cell.Child.Position = new Vector2((int)(parentWidth * (float)cell.posX), (int)(parentHeight * (float)cell.posY));
                    cell.Child.Width = (int)(parentWidth * cell.width);
                    cell.Child.Height = (int)(parentHeight * cell.height);
                }
            }

            base.Update(gameTime, info);
        }

        public override void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            base.Draw(gameTime, device, spriteBatch, info);
        }
    }

    public class GridCell
    {
        // These numbers go from 0 to 1 because they are percents
        public double posX = 0;
        public double posY = 0;
        public double width = 0;
        public double height = 0;

        public UIElement Child = null;
    }
}
