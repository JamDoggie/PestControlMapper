using Microsoft.Xna.Framework;
using PestControlEngine.GUI.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class UIConsole : UIElement
    {
        public UIRectangle BorderRectangle { get; set; }

        public UITextElement BorderText { get; set; }

        public UIRectangle BodyRectangle { get; set; }

        public UIConsole()
        {
            Position = new Vector2(100, 100);

            BorderRectangle = new UIRectangle()
            {
                Filled = true,
                RectangleColor = new Color(100, 100, 100),
                Width = 200,
                Height = 20
            };

            BodyRectangle = new UIRectangle()
            {
                Filled = true,
                RectangleColor = new Color(70, 70, 70),
                Width = 200,
                Height = 200,
                Position = new Vector2(0, 20)
            };

            BorderText = new UITextElement()
            {
                Text = "Debug Console",
                HorizontalAlignment = EnumHorizontalAlignment.LEFT,
                VerticalAlignment = EnumVerticalAlignment.TOP,
                DynamicallyScale = true

            };

            AddChild(BorderRectangle);
            AddChild(BodyRectangle);
            BorderRectangle.AddChild(BorderText);
        }
    }
}
