using DesignPatterns.ComponentPattern;
using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kaiju.ComponentPattern.Characters
{
    public class Gigan : Character
    {
        public Gigan(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            base.Start();
            sr.SetSprite("GG_Sprites\\GG_Walk\\GG_Walk_01");
            gameObject.Transform.Scale = new Vector2(3f, 3f);

            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Idle", new string[] { "GG_Sprites\\GG_Walk\\GG_Walk_01" }));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Walk", new string[] {
                "GG_Sprites\\GG_Walk\\GG_Walk_01",
                "GG_Sprites\\GG_Walk\\GG_Walk_02",
                "GG_Sprites\\GG_Walk\\GG_Walk_03",
                "GG_Sprites\\GG_Walk\\GG_Walk_04"}));

            if (this.gameObject == GameWorld.Instance.player1Go)
            {
                InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(GameWorld.Instance.player1, new Vector2(-1, 0)));
                InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(GameWorld.Instance.player1, new Vector2(1, 0)));
                InputHandler.Instance.AddButtonDownCommand(Keys.W, new JumpCommand(GameWorld.Instance.player1));
            }
            else if (this.gameObject == GameWorld.Instance.player2Go)
            {
                InputHandler.Instance.AddUpdateCommand(Keys.Left, new MoveCommand(GameWorld.Instance.player2, new Vector2(-1, 0)));
                InputHandler.Instance.AddUpdateCommand(Keys.Right, new MoveCommand(GameWorld.Instance.player2, new Vector2(1, 0)));
                InputHandler.Instance.AddButtonDownCommand(Keys.Up, new JumpCommand(GameWorld.Instance.player2));
            }
        }
        public override void Flip(bool x)
        {
            sr.SetFlipHorizontal(!x);
        }
    } 
        
}
