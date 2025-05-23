﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Kaiju.ComponentPattern
{
    internal class Timer : Component
    {
        private SpriteFont hudFont;
        private string text;
        private Vector2 position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2, 20);
        private float timeLeft = 120f;
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
                    timeLeft = 0f;
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
