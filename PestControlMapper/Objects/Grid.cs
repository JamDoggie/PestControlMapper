using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GameManagers;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Objects
{
    public class Grid : GameObject
    {
        public Grid()
        {
            Layer = -100000;
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (spriteBatch == null)
                return;

            if (!MainWindowViewModel.viewModel.GridVisible)
                return;

            GameCamera camera = info.objectManager.CurrentCamera;
            int gridsize = MainWindowViewModel.viewModel.GridSize;

            Texture2D onepx = ObjectManager.GetWhitePixel(device);

            int cameraGridY = camera.VisibleArea.Top - (camera.VisibleArea.Top % gridsize);

            int cameraGridX = camera.VisibleArea.Left - (camera.VisibleArea.Left % gridsize);

            int numHorizontal = camera.VisibleArea.Height / gridsize + 3;
            int numVertical = camera.VisibleArea.Width / gridsize + 3;

            //get line for x and y origin
            Rectangle originxline = new Rectangle(camera.VisibleArea.Left, 0, camera.VisibleArea.Right - camera.VisibleArea.Left, 1);
            Rectangle originyline = new Rectangle(0, camera.VisibleArea.Top, 1, camera.VisibleArea.Bottom - camera.VisibleArea.Top);

            Color gridColor = new Color(28, 28, 28, 190);

            //draw horizontal grid lines
            for (int i = 0; i < numHorizontal; i++)
            {
                Rectangle lineRect = new Rectangle(camera.VisibleArea.Left - 32, (i * gridsize) + cameraGridY, camera.VisibleArea.Width + 32, 1);
                spriteBatch.Draw(onepx, lineRect, gridColor);
            }
            //draw vertical grid lines
            for (int i = 0; i < numVertical; i++)
            {
                Rectangle lineRect = new Rectangle((i * gridsize) + cameraGridX, camera.VisibleArea.Top - 32, 1, camera.VisibleArea.Height + 32);
                spriteBatch.Draw(onepx, lineRect, gridColor);
            }

            // Origin lines
            Color originColor = new Color(158, 20, 177, 125);

            spriteBatch.Draw(onepx, originxline, originColor);
            spriteBatch.Draw(onepx, originyline, originColor);
        }
    }
}
