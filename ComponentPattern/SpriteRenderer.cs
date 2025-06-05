using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// The SpriteRenderer is in charge of rendering sprites for all components 
    /// </summary>
    public class SpriteRenderer : Component
    {
        public Vector2 Origin { get; set; }
        public Texture2D Sprite { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        private Rectangle source;
        public Rectangle Source
        {
            get => source;
            set
            {
                source = value;
                Origin = new Vector2(source.Width / 2f, source.Height / 2f);
            }
        }

        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
            SpriteEffect = SpriteEffects.None;
        }

        /// <summary>
        /// Sets a sprite to a component when called
        /// </summary>
        /// <param name="spriteName">The name of the sprite</param>
        public void SetSprite(string spriteName)
        {
            Sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
            if (Source == Rectangle.Empty)
            {
                Source = new Rectangle(0, 0, Sprite.Width, Sprite.Height);
            }
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        public override void Start()
        {
        }

        /// <summary>
        /// The main draw loop of the component
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, gameObject.Transform.Position, Source, Color.White, gameObject.Transform.Rotation, Origin, gameObject.Transform.Scale, SpriteEffect, 0);
        }

        /// <summary>
        /// Flips sprites using the spriteeffects based on a bool
        /// Used since a lot of sprites are flipped differently from each other
        /// so this generalises the orientation of the sprites
        /// </summary>
        /// <param name="flip">Bool that decides if it should flip</param>
        public void SetFlipHorizontal(bool flip)
        {
            if (flip)
                SpriteEffect = SpriteEffects.FlipHorizontally;
            else
                SpriteEffect = SpriteEffects.None;
        }

        public bool IsFlipped => SpriteEffect.HasFlag(SpriteEffects.FlipHorizontally);
    }
}
