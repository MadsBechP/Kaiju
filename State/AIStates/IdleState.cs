using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;

namespace Kaiju.State.AIStates
{
    public class IdleState : IState<AI>
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
            if (Vector2.Distance(Pos, OPos) > 200)
            {
                parrent.ChangeGameState(new ChaseState());
            }
        }

        public void Exit()
        {

        }
    }
}
