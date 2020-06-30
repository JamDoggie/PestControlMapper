using Microsoft.Xna.Framework;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.GUI
{
    public class UIFPSCounter : UIElement
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public UITextElement FPSText = new UITextElement();

        public UIFPSCounter()
        {
            AddChild(FPSText);
        }

        public override void Update(GameTime gameTime, GameInfo info)
        {
            CurrentFramesPerSecond = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

            FPSText.Text = ((int)AverageFramesPerSecond).ToString() + " FPS";

            Width = (int)ContentLoader.GetFont("engine_font").MeasureString(FPSText.Text).Width;
            Height = (int)ContentLoader.GetFont("engine_font").MeasureString(FPSText.Text).Height;

            base.Update(gameTime, info);
        }
    }
}
