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

        public Beam(GameObject gameObject) : base(gameObject)
        {
        }

        public override void OnCollisionEnter(Collider collider)
        {
            Debug.WriteLine($"Beam collided with {collider}");
            if (collider.Owner == owner)
            {
                return;
            }
        }
    }
}
