using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;

namespace Kaiju.State.AIStates
{
    public class RecoveryState : IState<AI>
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
            if (Pos.X > (GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2) + 500)
            {
                parrent.left.Execute();
            }
            else if (Pos.X < (GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2) - 500)
            {
                parrent.right.Execute();
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
