using Kaiju.ComponentPattern.Characters;
using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;

namespace Kaiju.ComponentPattern
{ 
    public class Player : Component, ISubject
    {
        protected float speed;
        private bool grounded = false;

        protected SpriteRenderer sr;
        private Animator animator;
        public Character chr;
        public bool facingRight;
        private bool lastPunchRight;
        private float atkCooldown;

        private bool specialActive;
        private float SpecialDuration = 3;
        private float specialTime;

        private bool hit = false;
        private float hitTimer;

        private bool blocking;
        private float maxblockhp = 30;
        private float blockhp = 30;
        private Texture2D shieldTexture;
        private float shieldMaxRadius = 100;

        private float secondTimer = 1;

        private List<IObserver> observers = new List<IObserver>();

        public InputType InputType { get; set; }
        public PlayerIndex GamePadIndex { get; set; }

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
                shieldTexture = CreateCircleTexture(GameWorld.Instance.GraphicsDevice, (int)shieldMaxRadius, Color.Red);
            }
            else if (gameObject == GameWorld.Instance.player2Go)
            {
                gameObject.Transform.Position = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 3) * 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
                facingRight = false;
                shieldTexture = CreateCircleTexture(GameWorld.Instance.GraphicsDevice, (int)shieldMaxRadius, Color.Blue);
            }
            speed = 600;
        }

        public override void Update()
        {
            if (!blocking)
            {
                if (secondTimer > 0)
                {
                    secondTimer -= GameWorld.Instance.DeltaTime;
                }
                else if (blockhp < maxblockhp)
                {
                    blockhp += 3;
                    secondTimer = 1;
                }
            }

            if (specialActive)
            {
                specialTime += GameWorld.Instance.DeltaTime;
                if (specialTime > SpecialDuration)
                {
                    specialActive = false;
                    specialTime = 0;
                }
            }

            KeyboardState keystate = Keyboard.GetState();
            if (keystate.IsKeyUp(Keys.LeftShift))
            {
                blocking = false;
            }

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

            // Plays animation when taking damage
            if (hit)
            {
                animator.PlayAnimation("Hit");
            }
            // Timer for attack
            if (atkCooldown > 0)
            {
                atkCooldown -= GameWorld.Instance.DeltaTime;
            }


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
            if (hit)
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.1f), gameObject.Transform.CurrentVelocity.Y);
            }
            else if (grounded)
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.5f), gameObject.Transform.CurrentVelocity.Y);
                hit = false;
            }
            else
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.3f), gameObject.Transform.CurrentVelocity.Y);
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

            if (specialActive)
            {
                animator.PlayAnimation("SawMove");
            }
            else
            {
                animator.PlayAnimation("Walk");
            }
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
            if (atkCooldown > 0 || hit)
            {
                return;
            }
            atkCooldown = 0.5f;

            switch (atkNumber)
            {
                case 1:
                    if (lastPunchRight)
                    {
                        animator.PlayAnimation("LPunch");
                        animator.RegisterFrameEvent("LPunch", 1, () => SpawnHitbox(atkNumber));
                        lastPunchRight = false;
                    }
                    else
                    {
                        animator.PlayAnimation("RPunch");
                        animator.RegisterFrameEvent("RPunch", 1, () => SpawnHitbox(atkNumber));
                        lastPunchRight = true;
                    }
                    break;

                case 2:
                    if (lastPunchRight)
                    {
                        animator.PlayAnimation("LKick");
                        animator.RegisterFrameEvent("LKick", 1, () => SpawnHitbox(atkNumber));

                        lastPunchRight = false;
                    }
                    else
                    {
                        animator.PlayAnimation("RKick");
                        animator.RegisterFrameEvent("RKick", 1, () => SpawnHitbox(atkNumber));
                        lastPunchRight = true;
                    }
                    break;

                case 3:
                    animator.PlayAnimation("TailSwipe");
                    animator.RegisterFrameEvent("TailSwipe", 3, () => SpawnHitbox(atkNumber));
                    break;

                case 4:
                    if (lastPunchRight)
                    {
                        animator.PlayAnimation("LPunch");
                        animator.RegisterFrameEvent("LPunch", 1, () => SpawnHitbox(atkNumber));
                        lastPunchRight = false;
                    }
                    else
                    {
                        animator.PlayAnimation("RPunch");
                        animator.RegisterFrameEvent("RPunch", 1, () => SpawnHitbox(atkNumber));

                        lastPunchRight = true;

                    }
                    break;

                case 5:
                    animator.PlayAnimation("Kick");
                    animator.RegisterFrameEvent("Kick", 1, () => SpawnHitbox(atkNumber));
                    break;

                case 6:
                    animator.PlayAnimation("Beam");
                    Vector2 fireDirection = facingRight ? Vector2.UnitX : -Vector2.UnitX;
                    animator.RegisterFrameEvent("Beam", 5, () => SpawnProjectile(fireDirection, atkNumber));
                    break;

            }
        }

        public void Special(int specialNumber)
        {
            switch (specialNumber)
            {
                case 1:
                    animator.PlayAnimation("Special");
                    Vector2 fireDirection = facingRight ? Vector2.UnitX : -Vector2.UnitX;
                    animator.RegisterFrameEvent("Special", 4, () => SpawnProjectile(fireDirection, specialNumber));
                    break;

                case 2:
                    animator.PlayAnimation("SawStill");
                    specialActive = true;
                    break;
            }
        }

        public void Block()
        {
            if (blockhp >= 1)
            {
                blocking = true;
            }
            else
            {
                blocking = false;
            }

            animator.PlayAnimation("Block");
        }

        public override void OnCollisionEnter(Collider collider)
        {
            if ((collider.isAttack && !hit) || (collider.isProjectile && !hit))
            {
                if (!blocking || blockhp < collider.Damage)
                {
                    GameWorld.Instance.Destroy(collider.gameObject);

                    TakeDamage(collider.Damage);
                    hit = true;
                    hitTimer = 0.5f;
                    Vector2 knockback = gameObject.Transform.Position - collider.Owner.gameObject.Transform.Position;
                    knockback.Normalize();

                    gameObject.Transform.CurrentVelocity = knockback * GameWorld.Instance.DeltaTime * 50 * Damage;
                    gameObject.Transform.AddVelocity(new Vector2(0, -1) * GameWorld.Instance.DeltaTime * 10 * Damage);
                }
                else
                {
                    TakeDamage(collider.Damage);
                    GameWorld.Instance.Destroy(collider.gameObject);
                }
            }


        }
        public void TakeDamage(int amount)
        {
            if (!blocking || blockhp < amount)
            {
                Damage += amount;
                Debug.WriteLine($"{this} took damage: {Damage}"); // tjek om TakeDamage faktisk bliver kaldt
                Notify();
            }
            else
            {
                blockhp -= amount;
                if (blockhp < 0)
                {
                    blockhp = 0;
                }
            }
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
            Debug.WriteLine($"{this} Notifying {observers.Count} observers."); //tjek om Notify bliver kaldt og sender vider til observer
            foreach (var observer in observers)
            {
                observer.Updated();
            }
        }

        public void SpawnHitbox(int atkNumber)
        {
            GameObject attackGo = new();
            Rectangle position = new Rectangle();
            int damage = 0;

            switch (atkNumber)
            {
                case 1:
                    if (facingRight)
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X + 100), (int)Math.Round(gameObject.Transform.Position.Y - 75), 50, 50);
                    }
                    else
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 150), (int)Math.Round(gameObject.Transform.Position.Y - 75), 50, 50);
                    }
                    damage = 5;
                    break;
                case 2:
                    if (facingRight)
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X + 100), (int)Math.Round(gameObject.Transform.Position.Y - 15), 50, 50);
                    }
                    else
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 150), (int)Math.Round(gameObject.Transform.Position.Y - 15), 50, 50);
                    }
                    damage = 5;
                    break;
                case 3:
                    if (facingRight)
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X), (int)Math.Round(gameObject.Transform.Position.Y - 50), 150, 100);
                    }
                    else
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 150), (int)Math.Round(gameObject.Transform.Position.Y - 50), 150, 100);
                    }
                    damage = 10;
                    break;
                case 4:
                    if (facingRight)
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X + 75), (int)Math.Round(gameObject.Transform.Position.Y - 75), 50, 50);
                    }
                    else
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 125), (int)Math.Round(gameObject.Transform.Position.Y - 75), 50, 50);
                    }
                    damage = 5;
                    break;
                case 5:
                    if (facingRight)
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X + 85), (int)Math.Round(gameObject.Transform.Position.Y), 50, 50);
                    }
                    else
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 135), (int)Math.Round(gameObject.Transform.Position.Y), 50, 50);
                    }
                    damage = 10;
                    break;
            }
            attackGo.AddComponent<Collider>(true, 0.2f, position, this, damage);
            GameWorld.Instance.Instantiate(attackGo);
        }

        public void SpawnProjectile(Vector2 direction, int atkNumber)
        {
            int damage;
            Rectangle position;
            SpriteRenderer spriteRenderer;
            Collider collider;
            switch (atkNumber)
            {
                case 1:
                    GameObject beam = new();
                    var beamComponent = beam.AddComponent<Beam>();
                    beamComponent.owner = this;

                    beam.Transform.Position = this.gameObject.Transform.Position + new Vector2(facingRight ? 300 : -300, -25);
                    beam.Transform.Scale = new Vector2(3);

                    spriteRenderer = beam.AddComponent<SpriteRenderer>();
                    spriteRenderer.SetSprite("GZ_Sprites\\GZ_Beath_Proj\\GZ_Beath_Proj_01");
                    spriteRenderer.SetFlipHorizontal(!facingRight);

                    damage = 10;
                    position = new Rectangle((int)gameObject.Transform.Position.X, (int)gameObject.Transform.Position.Y - 80, 300, 160);
                    collider = beam.AddComponent<Collider>(false, 5, position, this, damage);
                    collider.isProjectile = true;

                    GameWorld.Instance.Instantiate(beam);
                    break;

                case 6:
                    GameObject projectile = new();
                    var projComponent = projectile.AddComponent<Projectile>();
                    projComponent.direction = direction;
                    projComponent.speed = 100;
                    projComponent.owner = this;

                    projectile.Transform.Position = (this.gameObject.Transform.Position + new Vector2(0, -75)) + new Vector2(facingRight ? 100 : -100, 0);
                    projectile.Transform.Scale = new Vector2(2);

                    var spriteRendererProj = projectile.AddComponent<SpriteRenderer>();
                    spriteRendererProj.SetSprite("GG_Sprites\\GG_Beam_Proj\\GG_Beam_Proj_01");

                    if (facingRight)
                    {
                        spriteRendererProj.SetFlipHorizontal(true);
                    }

                    damage = 5;
                    position = new();
                    collider = projectile.AddComponent<Collider>(false, 5, position, this, damage);
                    collider.isProjectile = true;

                    GameWorld.Instance.Instantiate(projectile);
                    break;
            }
        }

        public Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            float rSquared = radius * radius;

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    int index = x + y * diameter;
                    float dx = x - radius;
                    float dy = y - radius;

                    if (dx * dx + dy * dy <= rSquared)
                    {
                        data[index] = color;
                    }
                    else
                    {
                        data[index] = Color.Transparent;
                    }
                }
            }
            texture.SetData(data);
            return texture;
        }

        public void DrawShield(SpriteBatch spriteBatch)
        {
            if (!blocking || shieldTexture == null)
            {
                return;
            }

            float shieldRatio = blockhp / maxblockhp;
            float scale = shieldRatio;

            Vector2 position = gameObject.Transform.Position;
            Vector2 origin = new Vector2(shieldTexture.Width / 2, shieldTexture.Height / 2);

            spriteBatch.Draw(shieldTexture, position, null, Color.White * 0.5f, 0, origin, scale, SpriteEffects.None, 0);
        }
    }
}
