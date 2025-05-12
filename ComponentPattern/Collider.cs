using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        private List<RectangleData> pixelPerfectRectangles = new();

        public Collider(GameObject gameObject) : base(gameObject)
        {
        }

        public Rectangle CollisionBox
        {
            get
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

        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            pixel = GameWorld.Instance.Content.Load<Texture2D>("pixel");
            CreateRectangles();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var rect in pixelPerfectRectangles)
            {
                DrawRectangle(rect.Rectangle, spriteBatch);
            }
            DrawRectangle(CollisionBox, spriteBatch);
        }

        public override void Update()
        {
            UpdatePixelCollider();
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
            for (int i = 0; i < pixelPerfectRectangles.Count; i++)
            {
                pixelPerfectRectangles[i].UpdatePosition(gameObject, sr.Sprite.Width, sr.Sprite.Height);
            }
        }

        private void CreateRectangles()
        {
            List<Color[]> lines = new();

            for (int y = 0; y < sr.Sprite.Height; y++)
            {
                Color[] colors = new Color[sr.Sprite.Width];
                sr.Sprite.GetData(0, new Rectangle(0, y, sr.Sprite.Width, 1), colors, 0, sr.Sprite.Width);
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
                            RectangleData rd = new(x, y);
                            pixelPerfectRectangles.Add(rd);
                        }
                    }
                }
            }
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

        public void UpdatePosition(GameObject gameObject, int width, int height)
        {
            Rectangle = new Rectangle((int)gameObject.Transform.Position.X + X - width / 2, (int)gameObject.Transform.Position.Y + Y - width / 2, 1, 1);
        }
    }
}
