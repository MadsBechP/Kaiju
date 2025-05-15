using Kaiju.ComponentPattern.Characters;
using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;

namespace Kaiju.ComponentPattern
{
    public class Player : Component, ISubject
    {
        private float speed;
        private bool grounded = false;
        private Vector2 yVelocity;
        private Vector2 currentVelocity = Vector2.Zero;

        protected SpriteRenderer sr;
        private Animator animator;
        public Character chr;
        public bool facingRight;
        private bool lastPunchRight;

        private List<IObserver> observers = new List<IObserver>();
        public int Damage { get; private set; }


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
            if (facingRight)
            {
                chr.FaceRight(true);
            }
            else
            {
                chr.FaceRight(false);
            }

            if (currentVelocity == Vector2.Zero && grounded)
            {
                //animator.PlayAnimation("Idle");
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
                yVelocity = new Vector2(0, -2000f);
                gameObject.Transform.Translate(yVelocity * GameWorld.Instance.DeltaTime);
            }
        }
        public void Attack(int atkNumber)
        {
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
            attackGo.AddComponent<Collider>(0.2f, position, this);
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

        public void TakeDamage(int amount)
        {
            Damage += amount;
            Notify();
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Updated();
            }
        }
    }
}
