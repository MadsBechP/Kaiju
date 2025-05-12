using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    public class Collider : Component
    {
        private SpriteRenderer sr;
        private Texture2D texture;

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
            //texture = GameWorld.Instance.Content.Load<Texture2D>("pixel");
        }
    }
}
