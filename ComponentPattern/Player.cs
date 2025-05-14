using DesignPatterns.ComponentPattern;
using Kaiju.ComponentPattern.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;

namespace Kaiju.ComponentPattern
{
    public class Player : Component
    {
        private float speed;
        private bool grounded = false;
        private Vector2 yVelocity;
        private Vector2 currentVelocity = Vector2.Zero;

        protected SpriteRenderer sr;
        private Animator animator;
        public Character chr;
        public bool facingRight;


        public Player(GameObject gameObject) : base(gameObject)
        {

        }

        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            animator = gameObject.GetComponent<Animator>() as Animator;
            
            if (gameObject == GameWorld.Instance.player1Go)
            {
                gameObject.Transform.Position = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 3) * 1, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
            }
            else if (gameObject == GameWorld.Instance.player2Go)
            {
                gameObject.Transform.Position = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 3) * 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
            }
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

            if (currentVelocity == Vector2.Zero && grounded)
            {
                animator.PlayAnimation("Idle");
            }

            currentVelocity = Vector2.Zero;
        }

        public void Move(Vector2 velocity)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }

            currentVelocity = velocity;

            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }

            velocity *= speed;
            gameObject.Transform.Translate(velocity * GameWorld.Instance.DeltaTime);

            if (velocity.X < 0 && grounded)
            {
                chr.Flip(true);
                facingRight = true;
            }
            if (velocity.X > 0 && grounded)
            {
                chr.Flip(false);
                facingRight = false;
            }




            animator.PlayAnimation("Walk");
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
