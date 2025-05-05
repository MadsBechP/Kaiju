using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.ComponentPattern
{
    public class SpriteRenderer : Component
    {
        public Vector2 Origin { get; set; }
        public Texture2D Sprite { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

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
            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, gameObject.Transform.Position, null, Color.White, gameObject.Transform.Rotation, Origin, gameObject.Transform.Scale, SpriteEffect, 0);

        }
    }
}
