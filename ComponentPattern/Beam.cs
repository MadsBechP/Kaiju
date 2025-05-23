using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    internal class Beam : Component
    {
        public Player owner;
        public Vector2 direction;
        public SpriteRenderer spriteRenderer;

        public Beam(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Update()
        {
            gameObject.Transform.Position = direction + owner.gameObject.Transform.Position + new Vector2(owner.facingRight ? 300 : -300, -25);
            spriteRenderer.SetFlipHorizontal(!owner.facingRight);
        }
    }
}
