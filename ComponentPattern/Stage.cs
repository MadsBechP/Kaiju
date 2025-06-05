using Microsoft.Xna.Framework;

namespace Kaiju.ComponentPattern
{
    public class Stage : Component
    {
        protected SpriteRenderer sr;

        public Stage(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Gets stage sprite and sets scale/position
        /// </summary>
        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Stage\\Stage");
            gameObject.Transform.Scale = new Vector2(6f, 6f);

            gameObject.Transform.Position = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2), GameWorld.Instance.GraphicsDevice.Viewport.Height + sr.Sprite.Height);

        }
    }
}
