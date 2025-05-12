using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    internal class DamageCounter : Component
    {
        private SpriteFont hudFont;
        private string text;

        public DamageCounter(GameObject gameObject) : base(gameObject)
        {
        }
    }
}
