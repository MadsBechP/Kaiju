using DesignPatterns.ComponentPattern;
using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1;

namespace Kaiju.ComponentPattern.Characters
{
    public class Godzilla : Character
    {
        public Godzilla(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            base.Start();
            sr.SetSprite("GZ_Sprites\\GZ_Walk\\GZ_Walk_01");
            gameObject.Transform.Scale = new Vector2(3f, 3f);

            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Idle", new string[] { "GZ_Sprites\\GZ_Walk\\GZ_Walk_01" }));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Walk", new string[] {
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_01",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_02",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_03",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_04",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_05",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_06",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_07",
                "GZ_Sprites\\GZ_Walk\\GZ_Walk_08"}));

            if (this.gameObject == GameWorld.Instance.player1Go)
            {
                InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(GameWorld.Instance.player1, new Vector2(-1, 0)));
                InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(GameWorld.Instance.player1, new Vector2(1, 0)));
                InputHandler.Instance.AddButtonDownCommand(Keys.W, new JumpCommand(GameWorld.Instance.player1));
            }
            if (this.gameObject == GameWorld.Instance.player2Go)
            {
                InputHandler.Instance.AddUpdateCommand(Keys.Left, new MoveCommand(GameWorld.Instance.player2, new Vector2(-1, 0)));
                InputHandler.Instance.AddUpdateCommand(Keys.Right, new MoveCommand(GameWorld.Instance.player2, new Vector2(1, 0)));
                InputHandler.Instance.AddButtonDownCommand(Keys.Up, new JumpCommand(GameWorld.Instance.player2));
            }
        }
    }
}
