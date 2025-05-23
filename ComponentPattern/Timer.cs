using Kaiju.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Kaiju.ComponentPattern
{
    public class Timer : Component
    {
        private SpriteFont hudFont;
        private string text;
        private Vector2 position = new Vector2(GameWorld.Instance.GraphicsDevice.Viewport.Width / 2, 20);
        private float timeLeft = 5f; //remember to change to 120f

        public bool TimeRanOut { get; private set; } = false;

        public Timer(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            base.Awake();
            hudFont = GameWorld.Instance.Content.Load<SpriteFont>("HUDFont");
        }
        public override void Update()
        {
            base.Update();
            if (timeLeft > 0f)
            {
                timeLeft -= GameWorld.Instance.DeltaTime;
                if (timeLeft < 0f)
                {
                    TimeRanOut = true;
                }
                TimeSpan time = TimeSpan.FromSeconds(timeLeft);
                text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(hudFont, text, position, Color.Crimson);
            }
        }
    }
}
