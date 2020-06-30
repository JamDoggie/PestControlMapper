using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GUI.Enum;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Libs.Helpers.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class Screen
    {
        private List<UIElement> Controls { get; set; } = new List<UIElement>();

        /// <summary>
        /// False = fade in
        /// True = fade out
        /// </summary>
        public bool FadeIn { get; set; } = true;

        public float FadeTime { get; set; } = 500;

        private UIRectangle fadeRectangle = null;

        private string _nextScreen = Util.GetEngineNull();

        public void Update(GameTime gameTime, GameInfo info)
        {
            // Update child controls
            foreach (UIElement control in Controls)
            {
                control.Update(gameTime, info);
            }
        }

        public void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, GameInfo info)
        {
            // Draw child controls
            foreach (UIElement control in Controls)
            {
                control.Draw(gameTime, device, spriteBatch, info);
            }
        }

        public void SetNextScreen(string next)
        {
            _nextScreen = next;
        }

        public string GetNextScreen()
        {
            return _nextScreen;
        }

        public void AddControl(UIElement control)
        {
            control.ParentScreen = this;
            Controls.Add(control);
        }

        public void RemoveControl(UIElement control)
        {
            Controls.Remove(control);
        }

        public List<UIElement> GetControls()
        {
            return Controls;
        }
    }
}
