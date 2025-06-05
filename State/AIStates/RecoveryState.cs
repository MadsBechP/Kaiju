using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;

namespace Kaiju.State.AIStates
{
    public class RecoveryState : IState<AI>
    {
        public AI parrent;

        Vector2 Pos { get { return parrent.gameObject.Transform.Position; } }

        /// <summary>
        /// Gets reference to owner
        /// </summary>
        /// <param name="parrent"></param>
        public void Enter(AI parrent)
        {
            this.parrent = parrent;
        }
        /// <summary>
        /// Runs towards the stages center
        /// </summary>
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
