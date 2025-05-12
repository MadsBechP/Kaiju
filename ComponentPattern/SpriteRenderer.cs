using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.ComponentPattern
{
    public class SpriteRenderer : Component
    {
        public Vector2 Origin { get; set; }
        public Texture2D Sprite { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public Rectangle Source { get; set; }

        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
            SpriteEffect = SpriteEffects.None;
        }
        public void SetSprite(string spriteName)
        {
            Sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
        }
        public override void Start()
        {
            Origin = new Vector2(Source.Center.X, Source.Center.Y);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, gameObject.Transform.Position, Source, Color.White, gameObject.Transform.Rotation, Origin, gameObject.Transform.Scale, SpriteEffect, 0);

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
