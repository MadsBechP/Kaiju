using Kaiju.Command;
using Kaiju.Observer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaiju.ComponentPattern
{
    public class AI : Player
    {
        ICommand left;
        ICommand right;
        ICommand jump;
        ICommand attack;
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
            attack = new AttackCommand(this, 1);

            gameObject.Transform.Scale = new Vector2(2f, 2f);


            if (this.gameObject == GameWorld.Instance.player1Go)
            {
                opponent = GameWorld.Instance.player2;

            }
            if (this.gameObject == GameWorld.Instance.player2Go)
            {
                opponent = GameWorld.Instance.player1;
            }
            speed = 300;
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
            else
            {
                attack.Execute();
            }
            if (Pos.Y > OPos.Y + 100)
            {
                jump.Execute();
            }
        }
    }
}
