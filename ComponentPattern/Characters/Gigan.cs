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

            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Idle", new string[] { "GG_Sprites\\GG_Walk\\GG_Walk_01" }, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Walk", new string[] {
                "GG_Sprites\\GG_Walk\\GG_Walk_01",
                "GG_Sprites\\GG_Walk\\GG_Walk_02",
                "GG_Sprites\\GG_Walk\\GG_Walk_03",
                "GG_Sprites\\GG_Walk\\GG_Walk_04"}, 5, true));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("Hit", new string[] {
                "GG_Sprites\\GG_Big_Hit\\GG_Big_Hit_01",
                "GG_Sprites\\GG_Big_Hit\\GG_Big_Hit_02"}, 4, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("LPunch", new string[] {
                "GG_Sprites\\GG_Punch_L\\GG_Punch_L_01",
                "GG_Sprites\\GG_Punch_L\\GG_Punch_L_02"}, 5, false));
            ani.AddAnimation(GameWorld.Instance.BuildAnimation("RPunch", new string[] {
                "GG_Sprites\\GG_Punch_R\\GG_Punch_R_01",
                "GG_Sprites\\GG_Punch_R\\GG_Punch_R_02"}, 5, false));

            if (gameObject.GetComponent<Player>() as Player != null)
            {
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
                    InputHandler.Instance.AddButtonDownCommand(Keys.P, new AttackCommand(GameWorld.Instance.player2, 1));
                } 
            }
        }
        public override void FaceRight(bool x)
        {
            sr.SetFlipHorizontal(x);
        }
    } 
        
}
