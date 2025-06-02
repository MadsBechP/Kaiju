using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// Handles collision detection, including pixel-perfect and rectangular collision boxes
    /// Based on the Collider Design Pattern
    /// Is part of Component Design Pattern
    /// Made by: Mads & Julius
    /// </summary>
    public class Collider : Component
    {
        private SpriteRenderer sr;
        private Texture2D pixel;
        private Texture2D previousSprite;
        private List<RectangleData> pixelPerfectRectangles = new();
        public List<RectangleData> PixelPerfectRectangles { get => pixelPerfectRectangles; }
        private Dictionary<Texture2D, List<RectangleData>> colliderChache = new();

        public bool isAttack = false;
        private bool forStage = false;
        public bool isProjectile;
        private float currentTime;
        public float maxTime;
        private Rectangle position;
        public Player Owner { get; set; }
        public int Damage { get; private set; }

        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Collider(GameObject gameObject) : base(gameObject)
        {

        }

        /// <summary>
        /// Stage Collider constructor
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        /// <param name="owner">specifies the owner of the collider</param>
        public Collider(GameObject gameObject, Player owner) : base(gameObject)
        {
            this.Owner = owner;
            forStage = true;

        }

        /// <summary>
        /// Attack collider constuctor
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        /// <param name="isAttack">Specifies if it is an attack</param>
        /// <param name="maxTime">The duration the collider is on the screen</param>
        /// <param name="position">The position of the collider</param>
        /// <param name="owner">specifies the owner of the collider</param>
        /// <param name="damage">The damage of the collider</param>
        public Collider(GameObject gameObject, bool isAttack, float maxTime, Rectangle position, Player owner, int damage) : base(gameObject)
        {
            this.isAttack = isAttack;
            this.maxTime = maxTime;
            this.position = position;
            this.Owner = owner;
            this.Damage = damage;
        }

        /// <summary>
        /// Gets the bounding collision rectangle of the collider
        /// Varies depending on whether it's for the stage, a normal sprite, or an attack
        /// </summary>
        public Rectangle CollisionBox
        {
            get
            {
                if (isAttack)
                {
                    return position;
                }
                else if (forStage)
                {
                    float scaleY = gameObject.Transform.Scale.Y;
                    int scaledHeight = (int)(sr.Sprite.Height * scaleY);
                    int scaledWidth = 100;

                    int bottom = (int)Math.Round(gameObject.Transform.Position.Y + scaledHeight / 2f);

                    return new Rectangle(
                        (int)Math.Round(gameObject.Transform.Position.X) - scaledWidth / 2,
                        bottom - scaledHeight,
                        scaledWidth,
                        scaledHeight
                    );
                }
                else
                {
                    float scaleX = gameObject.Transform.Scale.X;
                    float scaleY = gameObject.Transform.Scale.Y;
                    int scaledWidth = (int)(sr.Sprite.Width * scaleX);
                    int scaledHeight = (int)(sr.Sprite.Height * scaleY);
                    int x = (int)(gameObject.Transform.Position.X - scaledWidth / 2);
                    int y = (int)(gameObject.Transform.Position.Y - scaledHeight / 2);

                    return new Rectangle(x, y, scaledWidth, scaledHeight);
                }

            }
        }

        /// <summary>
        /// Starts the collider and loads the pixel texture and retrieves SpriteRenderer
        /// </summary>
        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            pixel = GameWorld.Instance.Content.Load<Texture2D>("pixel");
        }

        /// <summary>
        /// Draws the colliders if the game is debugging
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to render</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
#if DEBUG
            foreach (var rect in pixelPerfectRectangles)
            {
                DrawRectangle(rect.Rectangle, spriteBatch);
            }
            DrawRectangle(CollisionBox, spriteBatch);
#endif
        }

        /// <summary>
        /// Updates the collider including pixel-perfect colliders
        /// </summary>
        public override void Update()
        {
            if (!forStage)
            {
                if (!isAttack)
                {
                    if (sr.Sprite != previousSprite)
                    {
                        previousSprite = sr.Sprite;
                        if (!colliderChache.TryGetValue(sr.Sprite, out pixelPerfectRectangles))
                        {
                            pixelPerfectRectangles = CreateRectangles(sr.Sprite);
                            colliderChache[sr.Sprite] = pixelPerfectRectangles;
                        }

                        pixelPerfectRectangles = pixelPerfectRectangles.Select(p => new RectangleData(p.X, p.Y)).ToList();
                    }

                    UpdatePixelCollider();
                    if (isProjectile)
                    {
                        currentTime += GameWorld.Instance.DeltaTime;
                        if (currentTime > maxTime)
                        {
                            GameWorld.Instance.Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    currentTime += GameWorld.Instance.DeltaTime;
                    if (currentTime > maxTime)
                    {
                        GameWorld.Instance.Destroy(gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a colored rectangle outline around the specified collision box for debugging
        /// </summary>
        /// <param name="collisionBox">The rectangle representing the bounds of the collider to be drawn</param>
        /// <param name="spriteBatch">The SpriteBatch used to render</param>
        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Color color = Color.Red;
            if (isAttack)
            {
                color = Color.DarkGreen;
            }
            if (forStage)
            {
                color = Color.Yellow;
            }
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(pixel, topLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, bottomLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, rightLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, leftLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        /// <summary>
        /// Updates the position of pixel-perfect rectangles based on the object's transform
        /// </summary>
        public void UpdatePixelCollider()
        {
            Vector2 scale = gameObject.Transform.Scale;
            bool isFlipped = sr.IsFlipped;

            for (int i = 0; i < pixelPerfectRectangles.Count; i++)
            {
                pixelPerfectRectangles[i].UpdatePosition(gameObject, sr.Sprite.Width, sr.Sprite.Height, scale, isFlipped);
            }
        }

        /// <summary>
        /// Creates a list of edge pixels from a sprite texture for pixel-perfect collision
        /// </summary>
        /// <param name="texture">The texture to create the pixelperfect rectangles around</param>
        /// <returns>The list of rectangleData created</returns>
        private List<RectangleData> CreateRectangles(Texture2D texture)
        {
            List<RectangleData> rectangles = new();
            List<Color[]> lines = new();

            for (int y = 0; y < texture.Height; y++)
            {
                Color[] colors = new Color[texture.Width];
                texture.GetData(0, new Rectangle(0, y, texture.Width, 1), colors, 0, texture.Width);
                lines.Add(colors);
            }

            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x].A != 0)
                    {
                        if ((x == 0
                            || lines[y][x - 1].A == 0)
                            || (x == texture.Width - 1
                            || lines[y][x + 1].A == 0)
                            || (y == 0 || lines[y - 1][x].A == 0)
                            || (y == texture.Height - 1
                            || lines[y + 1][x].A == 0))

                        {
                            rectangles.Add(new RectangleData(x, y));
                        }
                    }
                }
            }

            return rectangles;
        }

        /// <summary>
        /// Updates the attack collider's position
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPosition(Rectangle newPosition)
        {
            if (isAttack)
            {
                position = newPosition;
            }
        }
    }

    /// <summary>
    /// Represents a single pixel-based rectangle used for pixel-perfect collision
    ///Based on the Pixel Perfect Collider Design Pattern
    /// Made by: Julius
    /// </summary>
    public class RectangleData
    {
        public Rectangle Rectangle { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        public RectangleData(int x, int y)
        {
            this.Rectangle = new Rectangle();
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Recalculates the rectangle's position and size based on transform, scaling, and flipping
        /// </summary>
        /// <param name="gameObject">The game object whose transform is used for positioning</param>
        /// <param name="width">The original width of the sprite this rectangle belongs to</param>
        /// <param name="height">The original height of the sprite this rectangle belongs to</param>
        /// <param name="scale">The scale of the sprite</param>
        /// <param name="isFlipped">Indicates if the sprite is flipped horizontally</param>
        public void UpdatePosition(GameObject gameObject, int width, int height, Vector2 scale, bool isFlipped)
        {
            int finalX = X;
            if (isFlipped)
            {
                finalX = width - X - 1;
            }

            int scaledX = (int)(finalX * scale.X);
            int scaledY = (int)(Y * scale.Y);

            int scaledWidth = (int)(1 * scale.X);  // width of pixel scaled
            int scaledHeight = (int)(1 * scale.Y); // height of pixel scaled

            Rectangle = new Rectangle(
                (int)(gameObject.Transform.Position.X + scaledX - (width * scale.X) / 2),
                (int)(gameObject.Transform.Position.Y + scaledY - (height * scale.Y) / 2),
                Math.Max(scaledWidth, 1),
                Math.Max(scaledHeight, 1)
            );
        }
    }
}
