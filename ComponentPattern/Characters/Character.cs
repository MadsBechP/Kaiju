using DesignPatterns.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern.Characters
{
    public class Character : Component
    {
        protected SpriteRenderer sr;
        protected Animator ani;
        public Character(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            ani = gameObject.GetComponent<Animator>() as Animator;
        }
    }
}
