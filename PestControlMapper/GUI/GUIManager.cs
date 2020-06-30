using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using PestControlEngine.GameManagers;
using PestControlEngine.GUI.Enum;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Objects;
using PestControlEngine.Resource;
using PestControlMapper.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class GUIManager
    {
        private Dictionary<string, Screen> _LoadedScreens = new Dictionary<string, Screen>();

        public string CurrentScreen = Util.GetEngineNull();

        public string ScreenToLoad = Util.GetEngineNull();

        public int VirtualViewWidth { get; set; } = 256;

        public int VirtualViewHeight { get; set; } = 224;

        public StretchType Stretch { get; set; } = StretchType.FIT_PARENT_ASPECT_RATIO;

        public bool UseVirtualSize { get; set; } = false;

        public bool UIHovered { get; set; } = false;

        private RenderTarget2D _RenderTarget = null;

        public GUIManager()
        {
            

        }

        

        public void LoadScreen(string key, Screen screen)
        {
            _LoadedScreens.Add(key, screen);
        }

        public void SetScreen(string screen)
        {
            ScreenToLoad = screen;
        }

        public void Update(GameTime gameTime, GameInfo info)
        {
            UIHovered = false;

            if (UseVirtualSize)
            {
                switch (Stretch)
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

            if (_RenderTarget != null)
                _RenderTarget.Dispose();

            if (UseVirtualSize)
                _RenderTarget = new RenderTarget2D(info.graphicsDevice, VirtualViewWidth, VirtualViewHeight, false, info.graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            if (ScreenToLoad != Util.GetEngineNull())
            {
                CurrentScreen = ScreenToLoad;
                ScreenToLoad = Util.GetEngineNull();
            }

            Screen currentScreen = GetScreen(CurrentScreen);

            if (currentScreen != null)
                currentScreen.Update(gameTime, info);

        }

        public void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            if (info == null)
                return;

            Screen currentScreen = GetScreen(CurrentScreen);

            if (_RenderTarget != null && UseVirtualSize)
                info.graphicsDevice.SetRenderTarget(_RenderTarget);

            // Only do this on the GUI layer so that it doesn't overwrite the object layer.
            if (_RenderTarget != null)
                info.graphicsDevice.Clear(Color.Transparent);

            StartDefaultSpriteBatch(spriteBatch);

            if (currentScreen != null)
                currentScreen.Draw(gameTime, device, spriteBatch, info);

            // Draw name above currently selected objects
            if (info.objectManager.SelectionObject != null && info.objectManager.SelectionObject.Selection != null)
            {
                foreach(GameObject gameObject in info.objectManager.SelectionObject.Selection)
                {
                    if (gameObject.Properties.ContainsKey("Name"))
                    {
                        MapGameObject mapObject = gameObject as MapGameObject;

                        float textScale = 0.5f;
                        float transformedTextScale = textScale / (float)info.objectManager.CurrentCamera.LerpZoom;

                        if (mapObject != null)
                        {
                            string shownText = $"{mapObject.Properties["Name"].CurrentValue} ({mapObject.Info.DisplayName})";

                            Size2 stringHeight = ContentLoader.GetFont("engine_font").MeasureString(shownText);

                            spriteBatch.DrawString(ContentLoader.GetFont("engine_font"), shownText, Vector2.Transform(new Vector2((int)mapObject.GetPosition().X, (int)mapObject.GetPosition().Y - (stringHeight.Height * transformedTextScale) - 2), (Matrix)info.objectManager.CurrentMatrix), Color.White, 0.0f, new Vector2(0, 0), textScale, SpriteEffects.None, 0f);
                        }

                    }
                }
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

        public Screen GetScreen(string screen)
        {
            Screen retScreen;

            _LoadedScreens.TryGetValue(screen, out retScreen);

            return retScreen;
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

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
        }
    }
}
