using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;

namespace Kaiju.ComponentPattern
{
    public class Player : Component
    {
        private float speed;
        private SpriteRenderer sr;
        private Vector2 yVelocity;
        private bool grounded = false;
        byte animationIndex;
        public Player(GameObject gameObject) : base(gameObject)
        {

        }
        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.Source = new Rectangle(27 + (72*animationIndex), 0, 72, 63);
            sr.SetSprite("Goji");
            gameObject.Transform.Scale = new Vector2(3f, 3f);
            gameObject.Transform.Position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
            speed = 600;
        }
        public override void Update()
        {
            
            gameObject.Transform.Translate(yVelocity * GameWorld.Instance.DeltaTime);
            if (gameObject.Transform.Position.Y < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (sr.Origin.Y * gameObject.Transform.Scale.Y))
            {
                yVelocity += new Vector2(0, 90f);
                grounded = false;
            }
            else
            {
                gameObject.Transform.Position = new Vector2(gameObject.Transform.Position.X, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (sr.Origin.Y * gameObject.Transform.Scale.Y));
                yVelocity = Vector2.Zero;
                grounded = true;
            }
        }
        public void Move(Vector2 velocity)
        {
            sr.Source = new Rectangle(27 + (72 * animationIndex), 0, 72, 63);
            animationIndex++;
            if (animationIndex == 8)
            {
                animationIndex = 0;
            }
            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }

            velocity *= speed;
            gameObject.Transform.Translate(velocity * GameWorld.Instance.DeltaTime);
            if (velocity.X < 0 && grounded)
            {
                sr.SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            if (velocity.X > 0 && grounded)
            {
                sr.SpriteEffect = SpriteEffects.None;
            }
        }
        public void Jump()
        {
            if (grounded)
            {
                yVelocity = new Vector2(0, -2000f);
                gameObject.Transform.Translate(yVelocity * GameWorld.Instance.DeltaTime);
            }
        }
    }
}
