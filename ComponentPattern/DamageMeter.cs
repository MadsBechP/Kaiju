using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// Displays a HUD element showing a player's damage percentage and name.
    /// Uses the Observer to update the display when subject changes.
    /// Made by Emilie
    /// </summary>
    public class DamageMeter : Component, IObserver
    {
        private SpriteFont damageFont;
        private SpriteFont playerNameFont;
        private Texture2D playerHUD;
        private Texture2D profileTexture;
        private float scale = 1.4f;

        private string damageText;
        private string playerName;
        private int damageTaken;

        private Vector2 damageFontPos;
        private Vector2 namePos;
        private Vector2 hudPos;
        private Vector2 profilePos;

        private ISubject subject;

        /// <summary>
        /// Constructer
        /// </summary>
        /// <param name="gameObject">The GameObject the component is attached to</param>
        public DamageMeter(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Sets up the layout and content of the damage meter HUD.
        /// </summary>
        /// <param name="playerName">The name of the player</param>
        /// <param name="profileTexture">The texture used for the player's profile picture</param>
        /// <param name="damageFontPos">The position of the damage text</param>
        /// <param name="namePos">The position of player's name</param>
        /// <param name="hudPos">The position of the HUD background</param>
        /// <param name="profilePos">The position of the profile picture</param>
        public void Setup(string playerName, Texture2D profileTexture, Vector2 damageFontPos, Vector2 namePos, Vector2 hudPos, Vector2 profilePos)
        {
            this.playerName = playerName;
            this.profileTexture = profileTexture;
            this.damageFontPos = damageFontPos;
            this.namePos = namePos;
            this.hudPos = hudPos;
            this.profilePos = profilePos;
        }

        /// <summary>
        /// Attaches the damage meter to a subject to be observed
        /// </summary>
        /// <param name="subject">The subject to be observed</param>
        public void SetSubject(ISubject subject)
        {
            this.subject = subject;
            subject.Attach(this);
        }

        /// <summary>
        /// Loads necessary content when the component starts. 
        /// </summary>
        public override void Start()
        {
            damageFont = GameWorld.Instance.Content.Load<SpriteFont>("DamageFont");
            playerNameFont = GameWorld.Instance.Content.Load<SpriteFont>("playerNameFont");
            playerHUD = GameWorld.Instance.Content.Load<Texture2D>("playerHUD");

            UpdateDamage();
        }

        /// <summary>
        /// Draws the damage meter HUD, including damage percentage, player name, and profile picture
        /// </summary>
        /// <param name="spriteBatch">Used to draw with</param>
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

        /// <summary>
        /// Calculate the latest damage value from the subject and updates the text.
        /// </summary>
        private void UpdateDamage()
        {
            if (subject is Player player)
            {
                damageTaken = player.Damage;
            }
            else if (subject is AI ai)
            {
                damageTaken = ai.Damage;
            }

            damageText = $"{damageTaken:D2}%";
            Debug.WriteLine($"DamageMeter Updated: {damageText}");
        }

        /// <summary>
        /// Called by the subject when its state changes.
        /// Updates the damage meter display when taking damage.
        /// </summary>
        public void Updated()
        {
            UpdateDamage();
        }
    }
}
