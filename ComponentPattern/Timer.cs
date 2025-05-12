using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    internal class Timer : Component
    {
        private SpriteFont hudFont;
        private string text = "02:00";
        private Vector2 position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2, 20);
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
