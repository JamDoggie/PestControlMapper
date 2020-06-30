using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PestControlEngine.GUI.Enum;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Objects;
using PestControlEngine.Resource;
using PestControlMapper.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GameManagers
{
    public class ObjectManager
    {
        private List<GameObject> Objects = new List<GameObject>();

        public Matrix? CurrentMatrix { get; set; } = null;

        public GameCamera CurrentCamera = null;

        public int VirtualViewWidth { get; set; } = 256;

        public int VirtualViewHeight { get; set; } = 224;

        public StretchType Stretch { get; set; } = StretchType.FIT_PARENT_ASPECT_RATIO;

        public bool UseVirtualSize { get; set; } = false;

        private RenderTarget2D _RenderTarget = null;

        public PreviewObject PreviewObject = new PreviewObject();

        public SelectionObject SelectionObject = new SelectionObject();

        public ObjectManager()
        {
            GameCamera camera = new GameCamera();
            Objects.Add(camera);
            camera.IsEnabled = true;
            CurrentMatrix = camera.Transform;

            Grid grid = new Grid();
            Objects.Add(grid);

            Objects.Add(PreviewObject);

            Objects.Add(SelectionObject);
        }

        /// <summary>
        /// Calls draw methods in all GameObjects. This then repeats with all the GameObject's children down the line.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="spriteBatch"></param>
        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (info == null)
                return;

            if (_RenderTarget != null && UseVirtualSize)
                info.graphicsDevice.SetRenderTarget(_RenderTarget);

            StartDefaultSpriteBatch(spriteBatch);

            var layeredObjects = Objects.OrderBy(f => f.Layer);

            if (spriteBatch != null)
            {
                foreach (GameObject drawable in layeredObjects)
                {
                    drawable.Draw(device, spriteBatch, info);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(spriteBatch));
            }

            SwitchToDefaultSpriteBatch(spriteBatch);

            if (UseVirtualSize)
                info.graphicsDevice.SetRenderTarget(null);

            // Draw render target to screen so we can "stretch" the viewport if needed.
            Rectangle bounds = new Rectangle(0, 0, info.graphicsDevice.PresentationParameters.BackBufferWidth, info.graphicsDevice.PresentationParameters.BackBufferHeight);

            if (UseVirtualSize)
                spriteBatch.Draw(_RenderTarget, bounds, Color.White);

            spriteBatch.End();

        }

        public List<GameObject> GetObjects()
        {
            return Objects;
        }

        public void Update(GameTime gameTime, GameInfo info)
        {
            if (UseVirtualSize)
            {
                switch(Stretch)
                {
                    case StretchType.FIT_PARENT_ASPECT_RATIO:
                        // Make aspect ratio of viewport the same as the parent window.
                        float aspectRatio = ((float)info.graphicsDevice.PresentationParameters.BackBufferWidth / (float)info.graphicsDevice.PresentationParameters.BackBufferHeight);
                        float newWidth = (VirtualViewHeight * aspectRatio);


                        //Console.WriteLine($"{info.graphicsDevice.PresentationParameters.BackBufferWidth}____{info.graphicsDevice.PresentationParameters.BackBufferHeight}");
                        VirtualViewWidth = (int)newWidth;
                        break;
                }
            }

            if (_RenderTarget == null && UseVirtualSize)
            {
                _RenderTarget = new RenderTarget2D(info.graphicsDevice, VirtualViewWidth, VirtualViewHeight, false, info.graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            }

            if (_RenderTarget != null && (_RenderTarget.Width != VirtualViewWidth || _RenderTarget.Height != VirtualViewHeight))
            {
                _RenderTarget = new RenderTarget2D(info.graphicsDevice, VirtualViewWidth, VirtualViewHeight, false, info.graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            }


            foreach (GameObject drawable in Objects)
            {
                drawable.Update(gameTime, info);

                if (drawable is GameCamera camera && camera.IsEnabled)
                {
                    CurrentCamera = camera;
                }
            }
        }

        public void SetMatrix(Matrix? matrix)
        {
            CurrentMatrix = matrix;
        }

        // Returns 1x1 texture that is RGB 255,255,255
        public static Texture2D GetWhitePixel(GraphicsDevice graphicsDevice)
        {
            if (ContentLoader.GetTexture("enginereserved_onepx", graphicsDevice) != null)
            {
                return ContentLoader.GetTexture("enginereserved_onepx", graphicsDevice);
            }

            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            Color[] colors = new Color[1];
            colors[0] = new Color(255, 255, 255);
            texture.SetData(colors);

            // Load texture into memory so we don't have to do the expensive operation of generating it every time.
            ContentLoader.LoadTexture("enginereserved_onepx", texture);

            return texture;
        }

        /// <summary>
        /// Helper method for drawing rectangle with specified thickness and color.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="rectangle"></param>
        /// <param name="strokeSize"></param>
        /// <param name="color"></param>
        public static void DrawRectangle(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle rectangle, int strokeSize, Color color)
        {
            Texture2D texture = GetWhitePixel(graphicsDevice);

            if (spriteBatch == null || graphicsDevice == null || rectangle == null || color == null)
            {
                return;
            }

            // Top part of rectangle
            spriteBatch.Draw(texture, new Rectangle(rectangle.X + strokeSize, rectangle.Y, rectangle.Width - strokeSize, strokeSize), new Rectangle(0, 0, 1, 1), color);
            // Left side of rectangle
            spriteBatch.Draw(texture, new Rectangle(rectangle.X, rectangle.Y, strokeSize, rectangle.Height), new Rectangle(0, 0, 1, 1), color);
            // Right side of rectangle
            spriteBatch.Draw(texture, new Rectangle(rectangle.X + rectangle.Width - strokeSize, rectangle.Y, strokeSize, rectangle.Height), new Rectangle(0, 0, 1, 1), color);
            // Bottom part of rectangle
            spriteBatch.Draw(texture, new Rectangle(rectangle.X + strokeSize, rectangle.Y + rectangle.Height - strokeSize, rectangle.Width - strokeSize, strokeSize), new Rectangle(0, 0, 1, 1), color);
        }

        /// <summary>
        /// Helper method for drawing rectangles with a specified color. This method only draws filled rectangles, for drawing unfilled rectangles with custom stroke sizes use DrawRectangle.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        public static void DrawFilledRectangle(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle rectangle, Color color)
        {
            Texture2D texture = GetWhitePixel(graphicsDevice);

            if (spriteBatch == null || graphicsDevice == null || rectangle == null || color == null)
            {
                return;
            }

            spriteBatch.Draw(texture, rectangle, new Rectangle(0, 0, 1, 1), color);
        }

        /// <summary>
        /// Ends given sprite batch and begins it with the default parameters.
        /// WARNING: sprite batch must already be started. Otherwise it will throw an exception.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void SwitchToDefaultSpriteBatch(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null)
                return;

            spriteBatch.End();
            StartDefaultSpriteBatch(spriteBatch);
        }


        /// <summary>
        /// Starts given spritebatch and beings it with the default parameters.
        /// WARNING: sprite batch must not already be started. This starts the spritebatch assuming it wasn't running.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void StartDefaultSpriteBatch(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, CurrentMatrix);

        }
    }

    public static class ExtendedSpriteBatch
    {
        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, texture, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}
