using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// A countdown timer component.
    /// Displays remaining time in a MM:SS format and goes to the ending screen (VictoryState) when time runs out.
    /// Made by Emilie
    /// </summary>
    public class Timer : Component
    {
        private SpriteFont hudFont;
        private string text;
        private Vector2 position = new Vector2(GameWorld.Instance.GraphicsDevice.Viewport.Width * 0.5f, GameWorld.Instance.GraphicsDevice.Viewport.Height * 0.05f);
        private float timeLeft = 120f;
        public bool TimeRanOut { get; private set; } = false;

        /// <summary>
        /// Initialize a new instance of the timer.
        /// </summary>
        /// <param name="gameObject">The GameObject to attach the timer to</param>
        public Timer(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Called once when the timer is initialized.
        /// Loads the font used to draw the timer text.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            hudFont = GameWorld.Instance.Content.Load<SpriteFont>("HUDFont");
        }

        /// <summary>
        /// Updates the timer by reducing the remaining time.
        /// When time is up, it will set TimeRanOut to true.
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (timeLeft > 0f)
            {
                timeLeft -= GameWorld.Instance.DeltaTime;
                if (timeLeft <= 0f)
                {
                    TimeRanOut = true;
                }
                TimeSpan time = TimeSpan.FromSeconds(timeLeft);
                text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }

        /// <summary>
        /// Draws the remaining timer in MM:SS format
        /// </summary>
        /// <param name="spriteBatch">Used to draw the timer</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(hudFont, text, position, Color.Crimson);
            }
        }
    }
}
