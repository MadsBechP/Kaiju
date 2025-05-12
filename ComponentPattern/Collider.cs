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

        public Collider(GameObject gameObject) : base(gameObject)
        {
        }

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(
                    (int)(gameObject.Transform.Position.X - sr.Sprite.Width / 2),
                    (int)(gameObject.Transform.Position.Y - sr.Sprite.Height / 2),
                    sr.Sprite.Width,
                    sr.Sprite.Height);
            }
        }

        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            pixel = GameWorld.Instance.Content.Load<Texture2D>("pixel");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawRectangle(CollisionBox, spriteBatch);
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
    }
}
