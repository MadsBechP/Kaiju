using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;
using System;

namespace Kaiju.State.AIStates
{
    public class IdleState : IState<AI>
    {
        public AI parrent;
        public Player opponent;
        private Random rnd = new();

        Vector2 Pos { get { return parrent.gameObject.Transform.Position; } }
        Vector2 OPos { get { return opponent.gameObject.Transform.Position; } }

        /// <summary>
        /// Gets refrences to owner and opponent
        /// </summary>
        /// <param name="parrent"></param>
        public void Enter(AI parrent)
        {
            this.parrent = parrent;
            opponent = parrent.Opponent;
        }

        /// <summary>
        /// Does a random attack if opponent is close enough, otherwise changes to chase
        /// </summary>
        public void Execute()
        {
            if (Vector2.Distance(Pos, OPos) > 200)
            {
                parrent.ChangeGameState(new ChaseState());
            }
            else
            {
                if (parrent.atkCooldown <= 0)
                {
                    switch (rnd.Next(3))
                    {
                        case (0):
                            parrent.punch.Execute();
                            parrent.atkCooldown = 1;
                            break;
                        case (1):
                            parrent.kick.Execute();
                            parrent.atkCooldown = 1;
                            break;
                        case (2):
                            parrent.special.Execute();
                            parrent.atkCooldown = 4;
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
