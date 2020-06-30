using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PestControlEngine.Libs.Helpers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Objects
{
    public class GameCamera : GameObject
    {
        public float Zoom;
        public Rectangle Bounds { get; protected set; }
        public Rectangle VisibleArea { get; protected set; }
        public Matrix Transform { get; set; }

        public float MinZoom { get; set; } = 0.35f;

        public float MaxZoom { get; set; } = 10f;

        public float DefaultZoom { get; set; } = 1f;

        public bool IsEnabled { get; set; } = false;

        public double LerpZoom = 1;

        public GameCamera()
        {
            Zoom = DefaultZoom;
            LerpZoom = Zoom;
            SetPosition(new Vector2(0, 0));
        }


        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            Transform = Matrix.CreateTranslation(new Vector3(GetPosition().X, GetPosition().Y, 0)) *
                Matrix.CreateScale((float)LerpZoom);

            UpdateVisibleArea();
        }

        public void MoveCamera(Vector2 movePosition)
        {
            Vector2 newPosition = GetPosition() + movePosition;
            SetPosition(newPosition);
        }

        public void AdjustZoom(float zoomAmount)
        {
            Zoom += zoomAmount;
            if (Zoom < MinZoom)
            {
                Zoom = MinZoom;
            }
            if (Zoom > MaxZoom)
            {
                Zoom = MaxZoom;
            }
        }

        public override void Update(GameTime gameTime, GameInfo gameInfo)
        {
            if (Zoom < MinZoom)
                Zoom = MinZoom;

            if (LerpZoom < MinZoom)
                LerpZoom = MinZoom;

            Bounds = gameInfo.graphicsDevice.Viewport.Bounds;

            double oldLerp = LerpZoom;

            if (LerpZoom < Zoom)
                LerpZoom = MathHelper.Lerp((float)LerpZoom, Zoom + 0.15f, 0.05f * (float)gameTime.ElapsedGameTime.TotalSeconds * 100);

            if (LerpZoom > Zoom)
                LerpZoom = MathHelper.Lerp((float)LerpZoom, Zoom - 0.15f, 0.05f * (float)gameTime.ElapsedGameTime.TotalSeconds * 100);

            if (LerpZoom > oldLerp && LerpZoom >= Zoom)
            {
                LerpZoom = Zoom;
            }

            if (LerpZoom < oldLerp && LerpZoom <= Zoom)
            {
                LerpZoom = Zoom;
            }

            UpdateMatrix();

            // If the camera is enabled, pass the matrix to the object manager. This displays the game using the camera's matrix essentially "enabling" this camera.
            if (IsEnabled)
            {
                gameInfo.objectManager.SetMatrix(Transform);
                gameInfo.objectManager.CurrentCamera = this;
            }

            

            base.Update(gameTime, gameInfo);
        }
    }
}
