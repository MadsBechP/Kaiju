using Kaiju.ComponentPattern.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;
using System.Threading;

namespace Kaiju.ComponentPattern
{
    public class Player : Component
    {
        protected float speed;
        private bool grounded = false;

        protected SpriteRenderer sr;
        private Animator animator;
        public Character chr;
        public bool facingRight;
        private bool lastPunchRight;
        private float atkCooldown;

        private bool hit = false;
        private float hitTimer;


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
                facingRight = true;
            }
            else if (gameObject == GameWorld.Instance.player2Go)
            {
                gameObject.Transform.Position = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 3) * 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
                facingRight = false;
            }
            speed = 600;
        }

        public override void Update()
        {
            // Moves player according to its current velocity
            gameObject.Transform.Translate();

            // Timer for hitstun
            if (hitTimer > 0)
            {
                hitTimer -= GameWorld.Instance.DeltaTime;
            }
            else if (hit)
            {
                hit = false;
            }

            // Timer for attack
            atkCooldown += GameWorld.Instance.DeltaTime;

            // Adds gravity
            if (gameObject.Transform.Position.Y < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (sr.Origin.Y * gameObject.Transform.Scale.Y))
            {
                gameObject.Transform.AddVelocity(new Vector2(0, 2f));
                grounded = false;
            }
            else
            {
                gameObject.Transform.Position = new Vector2(gameObject.Transform.Position.X, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (sr.Origin.Y * gameObject.Transform.Scale.Y));
                gameObject.Transform.CurrentVelocity = new Vector2(gameObject.Transform.CurrentVelocity.X, 0);
                grounded = true;
            }

            // Velocity resistance depending on players state
            if (grounded)
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.5f), gameObject.Transform.CurrentVelocity.Y);
                hit = false;
            }
            else if (!hit)
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.3f), gameObject.Transform.CurrentVelocity.Y);
            }
            else
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.1f), gameObject.Transform.CurrentVelocity.Y);
            }

            // Changes sprites facing direction
            if (facingRight)
            {
                chr.FaceRight(true);
            }
            else
            {
                chr.FaceRight(false);
            }

            
            

        }

        public void Move(Vector2 velocity)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }

            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }

            velocity *= speed;
            if (Math.Abs(gameObject.Transform.CurrentVelocity.X) < 8 && !hit)
            {
                gameObject.Transform.AddVelocity(velocity * GameWorld.Instance.DeltaTime);
            }

            if (velocity.X < 0 && grounded)
            {
                facingRight = false;
            }
            if (velocity.X > 0 && grounded)
            {
                facingRight = true;
            }

            animator.PlayAnimation("Walk");
        }

        public void Jump()
        {
            if (grounded)
            {
                gameObject.Transform.AddVelocity(new Vector2(0, -2000f) * GameWorld.Instance.DeltaTime);
            }
        }
        public void Attack(int atkNumber)
        {
            if (atkCooldown < 1)
            {
                return;
            }
            atkCooldown = 0;

            GameObject attackGo = new();
            Rectangle position = new Rectangle();
            if (facingRight)
            {
                position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X + 100), (int)Math.Round(gameObject.Transform.Position.Y - 100), 100, 100);
            }
            else
            {
                position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 200), (int)Math.Round(gameObject.Transform.Position.Y - 100), 100, 100);
            }
            attackGo.AddComponent<Collider>(0.2f, position, this, 1);
            GameWorld.Instance.Instantiate(attackGo);

            switch (atkNumber)
            {
                case 1:
                    {
                        if (lastPunchRight)
                        {
                            animator.PlayAnimation("LPunch");
                            lastPunchRight = false;
                        }
                        else
                        {
                            animator.PlayAnimation("RPunch");
                            lastPunchRight = true;
                        }
                        return;
                    }
                case 2:
                    {
                        animator.PlayAnimation("TailSwipe");
                        return;
                    }
            }
            
        }
        public void Block()
        {
            animator.PlayAnimation("Block");
        }

        public override void OnCollisionEnter(Collider collider)
        {
            if (collider.isAttack)
            {
                GameWorld.Instance.Destroy(collider.gameObject);

                hit = true;
                hitTimer = 0.5f;
                Vector2 knockback = gameObject.Transform.Position - collider.Owner.gameObject.Transform.Position;
                knockback.Normalize();

                gameObject.Transform.CurrentVelocity = knockback * GameWorld.Instance.DeltaTime * 3000;
            }
        }
    }
}
