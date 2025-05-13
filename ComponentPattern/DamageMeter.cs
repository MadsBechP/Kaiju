using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    public class DamageMeter : Component
    {
        private SpriteFont damageFont;
        private SpriteFont playerNameFont;
        private Texture2D playerHUD;
        private Texture2D playerProfile;

        private string text = "00%";
        private string playerName = "GZ";

        private Vector2 positionDamageFont = new Vector2(
            (GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2) - 305,
            GameWorld.Instance.Graphics.PreferredBackBufferHeight - 185);

        private Vector2 positionPlayerName = new Vector2(
            (GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2) - 310,
            GameWorld.Instance.Graphics.PreferredBackBufferHeight - 80);

        private Vector2 positionPlayerHud = new Vector2(
            (GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2) - 550,
            GameWorld.Instance.Graphics.PreferredBackBufferHeight - 250);

        private Vector2 positionPlayerProfile = new Vector2(
            (GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2) - 505,
            GameWorld.Instance.Graphics.PreferredBackBufferHeight - 200);

        private float scale = 1.4f;

        public DamageMeter(GameObject gameObject) : base(gameObject)
        {

        }
        public override void Start()
        {  
            damageFont = GameWorld.Instance.Content.Load<SpriteFont>("DamageFont");
            playerNameFont = GameWorld.Instance.Content.Load<SpriteFont>("playerNameFont");
            playerHUD = GameWorld.Instance.Content.Load<Texture2D>("playerHUD");
            playerProfile = GameWorld.Instance.Content.Load<Texture2D>("GZProfile");
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerHUD, positionPlayerHud, Color.White);

            spriteBatch.Draw(playerProfile, positionPlayerProfile, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1);

            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(damageFont, text, positionDamageFont, Color.White);
            }
            if (!string.IsNullOrEmpty(playerName))
            {
                spriteBatch.DrawString(playerNameFont, playerName, positionPlayerName, Color.White);
            }

        }
    }
}
