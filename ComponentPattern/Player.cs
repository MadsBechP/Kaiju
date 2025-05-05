using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    public class Player : Component
    {
        public Player(GameObject gameObject) : base(gameObject)
        {

        }
        public override void Start()
        {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Goji");
            gameObject.Transform.Position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
            gameObject.Transform.Scale = new Vector2(0.5f, 0.5f);
        }
        public override void Update()
        {
        }
    }
}
