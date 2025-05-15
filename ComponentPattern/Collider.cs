using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kaiju.ComponentPattern
{
    public class Collider : Component
    {
        private SpriteRenderer sr;
        private Texture2D pixel;
        private Texture2D previousSprite;
        private List<RectangleData> pixelPerfectRectangles = new();
        public List<RectangleData> PixelPerfectRectangles { get => pixelPerfectRectangles; }
        private Dictionary<Texture2D, List<RectangleData>> colliderChache = new();

        public bool isAttack;
        private float currentTime;
        private float maxTime;
        private Rectangle position;
        public Player Owner { get; set; }
        public float Damage { get; private set; }


        public Collider(GameObject gameObject) : base(gameObject)
        {

        }
        public Collider(GameObject gameObject, float maxTime, Rectangle position, Player owner, float damage) : base(gameObject)
        {
            this.isAttack = true;
            this.maxTime = maxTime;
            this.position = position;
            this.Owner = owner;
            this.Damage = damage;
        }

        public Rectangle CollisionBox
        {
            get
            {
                if (isAttack)
                {
                    return position;
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

        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            pixel = GameWorld.Instance.Content.Load<Texture2D>("pixel");
        }

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

        public override void Update()
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

        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(pixel, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        public void UpdatePixelCollider()
        {
            Vector2 scale = gameObject.Transform.Scale;
            bool isFlipped = sr.IsFlipped;

            for (int i = 0; i < pixelPerfectRectangles.Count; i++)
            {
                pixelPerfectRectangles[i].UpdatePosition(gameObject, sr.Sprite.Width, sr.Sprite.Height, scale, isFlipped);
            }
        }

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
                        if ((x == 0)
                            || (x == lines[y].Length)
                            || (x > 0 && lines[y][x - 1].A == 0)
                            || (x < lines[y].Length - 1 && lines[y][x + 1].A == 0)
                            || (y == 0)
                            || (y > 0 && lines[y - 1][x].A == 0)
                            || (y < lines.Count - 1 && lines[y + 1][x].A == 0))
                        {
                            rectangles.Add(new RectangleData(x, y));
                        }
                    }
                }
            }

            return rectangles;
        }
    }

    public class RectangleData
    {
        public Rectangle Rectangle { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public RectangleData(int x, int y)
        {
            this.Rectangle = new Rectangle();
            this.X = x;
            this.Y = y;
        }

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
