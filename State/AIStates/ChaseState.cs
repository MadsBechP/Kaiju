using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;

namespace Kaiju.State.AIStates
{
    public class ChaseState : IState<AI>
    {
        public AI parrent;
        public Player opponent;

        Vector2 Pos { get { return parrent.gameObject.Transform.Position; } }
        Vector2 OPos { get { return opponent.gameObject.Transform.Position; } }

        public void Enter(AI parrent)
        {
            this.parrent = parrent;
            opponent = parrent.opponent;
        }

        public void Execute()
        {
            // chases opponent if they are more than 200 away
            if (Pos.X < OPos.X - 200)
            {
                parrent.right.Execute();
            }
            else if (Pos.X > OPos.X + 200)
            {
                parrent.left.Execute();
            }
            
            else
            {
                parrent.ChangeGameState(new IdleState());
            }
        }

        public void Exit()
        {

        }
    }
}
