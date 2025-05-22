using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    internal class Projectile : Component
    {
        public Vector2 direction;
        public float speed;
        public Player owner;

        public Projectile(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Update()
        {
            gameObject.Transform.Position += direction * speed * GameWorld.Instance.DeltaTime;
        }

        public override void OnCollisionEnter(Collider collider)
        {
            Debug.WriteLine($"Projectile collided with {collider}");
            if (collider.Owner == owner)
            {
                return;
            }

            GameWorld.Instance.Destroy(gameObject);
        }
    }
}
