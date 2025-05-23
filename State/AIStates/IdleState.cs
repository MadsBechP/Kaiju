using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;
using System;

namespace Kaiju.State.AIStates
{
    public class IdleState : IState<AI>
    {
        public AI parrent;
        public Player opponent;
        private Random rnd;
        private float timer;

        Vector2 Pos { get { return parrent.gameObject.Transform.Position; } }
        Vector2 OPos { get { return opponent.gameObject.Transform.Position; } }

        public void Enter(AI parrent)
        {
            this.parrent = parrent;
            opponent = parrent.opponent;
            rnd = new Random();
        }

        public void Execute()
        {
            if (timer > 0)
            {
                timer -= GameWorld.Instance.DeltaTime;
                return;
            }



            if (Vector2.Distance(Pos, OPos) > 200)
            {
                parrent.ChangeGameState(new ChaseState());
            }
            else
            {
                switch (rnd.Next(1, 6))
                {
                    case 1:
                        {
                            parrent.punch.Execute();
                            break;
                        }
                    case 2:
                        {
                            parrent.kick.Execute();
                            break;
                        }
                    case 3:
                        {
                            parrent.special.Execute();
                            break;
                        }
                    case 4:
                        {
                            timer = 1f;
                            break;
                        }
                    case 5:
                        {
                            timer = 1.5f;
                            break;
                        }
                }
            }
        }

        public void Exit()
        {

        }
    }
}
