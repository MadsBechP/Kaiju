using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.ComponentPattern
{
    public class AI : Player
    {
        ICommand left;
        ICommand right;
        ICommand jump;
        Player opponent;

        Vector2 Pos { get {return gameObject.Transform.Position; } }
        Vector2 OPos { get { return opponent.gameObject.Transform.Position; } }

        public AI(GameObject gameObject) : base(gameObject)
        {
            
        }

        public override void Start()
        {

            base.Start();
            left = new MoveCommand(this, new Vector2(-1, 0));
            right = new MoveCommand(this, new Vector2(1, 0));
            jump = new JumpCommand(this);

            gameObject.Transform.Scale = new Vector2(2f, 2f);


            if (this.gameObject == GameWorld.Instance.player1Go)
            {
                opponent = GameWorld.Instance.player2;

            }
            if (this.gameObject == GameWorld.Instance.player2Go)
            {
                opponent = GameWorld.Instance.player1;
            }
        }
        public override void Update()
        {
            base.Update();
            if (Pos.X < OPos.X - 200)
            {
                right.Execute();
            }
            else if (Pos.X > OPos.X + 200)
            {
                left.Execute();
            }
            if (Pos.Y > OPos.Y + 300)
            {
                jump.Execute();
            }
        }
        public override void OnCollisionEnter(Collider collider)
        {
            if (collider.isAttack)
            {
                gameObject.Transform.Position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth/2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2);
            }


        }
    }
}
