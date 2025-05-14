using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    public class DamageMeter : Component, IObserver
    {
        private SpriteFont damageFont;
        private SpriteFont playerNameFont;
        private Texture2D playerHUD;
        private Texture2D profileTexture;
        private float scale = 1.4f;

        private string damageText = "";
        private string playerName;
        private int damageTaken;

        private Vector2 damageFontPos;
        private Vector2 namePos;
        private Vector2 hudPos;
        private Vector2 profilePos;
        
        
        public DamageMeter(GameObject gameObject) : base(gameObject)
        {
            GameWorld.Instance.Attach(this); 
        }
        
        public void Setup(string playerName, Texture2D profileTexture, Vector2 damageFontPos, Vector2 namePos, Vector2 hudPos, Vector2 profilePos)
        {
            this.playerName = playerName;
            this.profileTexture = profileTexture;
            this.damageFontPos = damageFontPos;
            this.namePos = namePos;
            this.hudPos = hudPos;
            this.profilePos = profilePos;
        }

        public override void Start()
        {  
            damageFont = GameWorld.Instance.Content.Load<SpriteFont>("DamageFont");
            playerNameFont = GameWorld.Instance.Content.Load<SpriteFont>("playerNameFont");
            playerHUD = GameWorld.Instance.Content.Load<Texture2D>("playerHUD");
        }
        //public override void Update()
        //{
        //    base.Update();
            
        //}
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerHUD, hudPos, Color.White);

            spriteBatch.Draw(profileTexture, profilePos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1);

            if (!string.IsNullOrEmpty(damageText))
            {
                spriteBatch.DrawString(damageFont, damageText, damageFontPos, Color.White);
            }
            if (!string.IsNullOrEmpty(playerName))
            {
                spriteBatch.DrawString(playerNameFont, playerName, namePos, Color.White);
            }

        }

        public void Updated()
        {
            damageTaken++;
            damageText = $"{damageTaken:D2}%";
        }
    }
}
