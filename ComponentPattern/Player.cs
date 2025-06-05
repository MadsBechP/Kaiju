using Kaiju.ComponentPattern.Characters;
using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// The main playerclass in charge of all player behavior
    /// Part of the Component Pattern
    /// Made by: All
    /// </summary>
    public class Player : Component, ISubject
    {
        protected float speed;
        public bool grounded = false;

        protected SpriteRenderer sr;
        private Animator animator;
        public Collider collider;
        public Collider stageCollider;
        public Character chr;
        public bool facingRight;
        private bool lastPunchRight;
        private float atkCooldown;
        private Vector2 startPos;

        // Special attack logic
        private bool specialActive;
        private float SpecialDuration = 3;
        private float specialTime;
        private float specialCooldown;
        private GameObject sawHitBox;

        private bool hit = false; // is player in hitstun
        private float hitTimer; // timer for hitstun

        // blocking logic
        private bool blocking; 
        private float maxblockhp = 30;
        private float blockhp = 30;
        private Texture2D shieldTexture;
        private float shieldMaxRadius = 100;
        private float secondTimer = 1; // timer for shield regen

        private List<IObserver> observers = new List<IObserver>();

        public InputType InputType { get; set; }
        public PlayerIndex GamePadIndex { get; set; }

        public int Damage { get; private set; }
        public int Lives { get; private set; } = 3;

        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Player(GameObject gameObject) : base(gameObject)
        {

        }

        /// <summary>
        /// Initializes the player component
        /// </summary>
        public override void Start()
        {
            // gets reference to relevant components
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            animator = gameObject.GetComponent<Animator>() as Animator;
            collider = gameObject.GetComponent<Collider>() as Collider;

            // sets spawnpoint based on player number
            if (gameObject == GameWorld.Instance.player1Go)
            {
                startPos = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 3) * 1, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
                facingRight = true;
                shieldTexture = CreateCircleTexture(GameWorld.Instance.GraphicsDevice, (int)shieldMaxRadius, Color.Red);
            }
            else if (gameObject == GameWorld.Instance.player2Go)
            {
                startPos = new Vector2((GameWorld.Instance.Graphics.PreferredBackBufferWidth / 3) * 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
                facingRight = false;
                shieldTexture = CreateCircleTexture(GameWorld.Instance.GraphicsDevice, (int)shieldMaxRadius, Color.Blue);
            }
            gameObject.Transform.Position = startPos;
            speed = 600;
        }

        /// <summary>
        /// The main update loop of the player
        /// </summary>
        public override void Update()
        {
            // heals shield hp every second
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
                    animator.PlayAnimation("Idle");
                    if (sawHitBox != null)
                    {
                        GameWorld.Instance.Destroy(sawHitBox);
                        sawHitBox = null;
                    }
                }
                else if (sawHitBox != null)
                {
                    var collider = sawHitBox.GetComponent<Collider>() as Collider;
                    if (collider != null)
                    {
                        if (facingRight)
                        {
                            collider.SetPosition(new Rectangle((int)(gameObject.Transform.Position.X + 55), (int)(gameObject.Transform.Position.Y - 50), 50, 125));

                        }
                        else
                        {
                            collider.SetPosition(new Rectangle((int)(gameObject.Transform.Position.X - 105), (int)(gameObject.Transform.Position.Y - 50), 50, 125));
                        }
                    }
                }
            }
            // ticks down specialcooldown timer
            if (specialCooldown > 0)
            {
                specialCooldown -= GameWorld.Instance.DeltaTime;
            }

            KeyboardState keystate = Keyboard.GetState();
            if (keystate.IsKeyUp(Keys.LeftShift))
            {
                blocking = false;
            }

            // adds downward velocity, aka gravity
            if (gameObject.Transform.CurrentVelocity.Y < 50)
            {
                gameObject.Transform.AddVelocity(new Vector2(0, 2f));
            }
            // moves player based on current velocity and checks for collision with stage
            gameObject.Transform.Translate(stageCollider);

            // respawns player when below the stage
            if (gameObject.Transform.Position.Y > GameWorld.Instance.GraphicsDevice.Viewport.Height * 1.5f)
            {
                gameObject.Transform.Position = startPos;
                hitTimer = 2f;
                gameObject.Transform.CurrentVelocity = Vector2.Zero;

                Lives--;
                Damage = 0;
                Notify();
            }
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


            // Velocity resistance depending on players state
            if (hit)
            {
                gameObject.Transform.CurrentVelocity = new Vector2(Single.Lerp(gameObject.Transform.CurrentVelocity.X, 0, 0.1f), gameObject.Transform.CurrentVelocity.Y);
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

        /// <summary>
        /// The movement logic of the player consisting of velocity, direction and animations
        /// </summary>
        /// <param name="velocity">the direction of movement</param>
        public void Move(Vector2 velocity)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }
            else
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

        /// <summary>
        /// Jump logic making the player jump if on the ground
        /// </summary>
        public void Jump()
        {
            if (grounded)
            {
                gameObject.Transform.AddVelocity(new Vector2(0, -2000f) * GameWorld.Instance.DeltaTime);
            }
        }

        /// <summary>
        /// The attack logic of the player, Uses different attack based on what attacknumber is assigned
        /// </summary>
        /// <param name="atkNumber">number that specifies which attack should be made</param>
        public void Attack(int atkNumber)
        {
            if (atkCooldown > 0 || hit)
            {
                return;
            }
            atkCooldown = 0.5f;
            hitTimer = 0f;

            // 1 - godzilla punch
            // 2 - godzilla kick
            // 3 - godzilla tailswipe
            // 4 - gigan punch
            // 5 - gigan kick
            // 6 - gigan beam
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

        /// <summary>
        /// The special attack logic of the player, Uses different specials based on what specialnumber is assigned
        /// </summary>
        /// <param name="specialNumber">number that specifies which special should be made</param>
        public void Special(int specialNumber)
        {
            if (specialCooldown > 0 || hit)
            {
                return;
            }
            specialCooldown = 5f;
            // 1 - godzilla
            // 2 - gigan
            switch (specialNumber)
            {
                case 1:
                    animator.PlayAnimation("Special");
                    Vector2 fireDirection = facingRight ? Vector2.UnitX : -Vector2.UnitX;
                    animator.RegisterFrameEvent("Special", 4, () => SpawnProjectile(fireDirection, specialNumber));
                    break;

                case 2:
                    animator.PlayAnimation("SawStill");
                    animator.RegisterFrameEvent("SawStill", 3, () => animator.PlayAnimation("SawCont"));
                    specialNumber = 6;
                    SpawnHitbox(specialNumber);
                    specialActive = true;
                    break;
            }
        }

        /// <summary>
        /// Allows the player to block if the shield has enough HP
        /// </summary>
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

        /// <summary>
        /// Handles collision events when the player collides with other colliders
        /// </summary>
        /// <param name="collider">The collider the player collided with</param>
        public override void OnCollisionEnter(Collider collider)
        {
            if (collider.Owner != this)
            {
                if ((collider.isAttack && !hit) || (collider.isProjectile && !hit))
                {
                    if (!blocking || blockhp < collider.Damage)
                    {
                        if (!collider.isProjectile && collider.maxTime < 2)
                        {
                            GameWorld.Instance.Destroy(collider.gameObject);
                        }

                        TakeDamage(collider.Damage);
                        hit = true;
                        hitTimer = 1f;
                        Vector2 knockback = gameObject.Transform.Position - collider.Owner.gameObject.Transform.Position;
                        knockback.Normalize();

                        gameObject.Transform.CurrentVelocity = knockback * GameWorld.Instance.DeltaTime * 50 * Damage;
                        gameObject.Transform.AddVelocity(new Vector2(0, -1) * GameWorld.Instance.DeltaTime * 10 * Damage);
                    }
                    else
                    {
                        TakeDamage(collider.Damage);
                        hitTimer = 0.5f;
                        if (!collider.isProjectile && collider.maxTime < 2)
                        {
                            GameWorld.Instance.Destroy(collider.gameObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies damage and knockback to the player depending on block and attacks
        /// </summary>
        /// <param name="amount">amount of damage to apply</param>
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

        /// <summary>
        /// Attaches an observer to the player for recieving updates
        /// </summary>
        /// <param name="observer">The observer to attach</param>
        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Detaches an observer from the player.
        /// </summary>
        /// <param name="observer">The observer to detach</param>
        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// Notifies all attached observers that the player state has changed
        /// </summary>
        public void Notify()
        {
            Debug.WriteLine($"{this} Notifying {observers.Count} observers.");
            foreach (var observer in observers)
            {
                observer.Updated();
            }
        }

        /// <summary>
        /// Spawns a hitbox game object for the specified attack number, setting its position, damage, and duration
        /// </summary>
        /// <param name="atkNumber">The attack identifier determining hitbox size and position</param>
        public void SpawnHitbox(int atkNumber)
        {
            GameObject attackGo = new();
            Rectangle position = new Rectangle();
            int damage = 0;
            float maxTime = 0.2f;

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
                case 6:
                    if (facingRight)
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X + 55), (int)Math.Round(gameObject.Transform.Position.Y - 50), 50, 125);
                    }
                    else
                    {
                        position = new Rectangle((int)Math.Round(gameObject.Transform.Position.X - 105), (int)Math.Round(gameObject.Transform.Position.Y - 50), 50, 125);
                    }

                    damage = 10;
                    maxTime = SpecialDuration;
                    sawHitBox = attackGo;
                    break;
            }
            attackGo.AddComponent<Collider>(true, maxTime, position, this, damage);
            GameWorld.Instance.Instantiate(attackGo);
        }

        /// <summary>
        /// Spawns a projectile game object fired in the given direction, based on the attack type
        /// </summary>
        /// <param name="direction">The direction the projectile will travel</param>
        /// <param name="atkNumber">The attack identifier for the projectile type</param>
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
                    beamComponent.spriteRenderer = spriteRenderer;
                    spriteRenderer.SetSprite("GZ_Sprites\\GZ_Beath_Proj\\GZ_Beath_Proj_01");
                    spriteRenderer.SetFlipHorizontal(!facingRight);

                    var animator = beam.AddComponent<Animator>();
                    animator.AddAnimation(GameWorld.Instance.BuildAnimation("Breath", new string[] {
                        "GZ_Sprites\\GZ_Beath_Proj\\GZ_Beath_Proj_01",
                        "GZ_Sprites\\GZ_Beath_Proj\\GZ_Beath_Proj_02" }, 5, false));
                    animator.PlayAnimation("Breath");

                    damage = 10;
                    position = new Rectangle((int)gameObject.Transform.Position.X, (int)gameObject.Transform.Position.Y - 80, 300, 160);
                    collider = beam.AddComponent<Collider>(false, 2, position, this, damage);
                    collider.isProjectile = true;

                    GameWorld.Instance.Instantiate(beam);
                    break;

                case 6:
                    GameObject projectile = new();
                    var projComponent = projectile.AddComponent<Projectile>();
                    projComponent.direction = direction;
                    projComponent.speed = 1000;
                    projComponent.owner = this;

                    projectile.Transform.Position = (this.gameObject.Transform.Position + new Vector2(0, -75)) + new Vector2(facingRight ? 100 : -100, 0);
                    projectile.Transform.Scale = new Vector2(2);

                    spriteRenderer = projectile.AddComponent<SpriteRenderer>();
                    spriteRenderer.SetSprite("GG_Sprites\\GG_Beam_Proj\\GG_Beam_Proj_01");
                    spriteRenderer.SetFlipHorizontal(facingRight);

                    damage = 5;
                    position = new();
                    collider = projectile.AddComponent<Collider>(false, 5, position, this, damage);
                    collider.isProjectile = true;

                    GameWorld.Instance.Instantiate(projectile);
                    break;
            }
        }

        /// <summary>
        /// Creates a circular texture of a given radius and color, used for drawing the shield
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used to create the texture</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="color">The fill color of the circle</param>
        /// <returns>A Texture2D object representing a filled circle</returns>
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

        /// <summary>
        /// Draws the player's shield with transparency and scaling based on current block HP
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing</param>
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
