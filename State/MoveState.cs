using Kaiju.ComponentPattern;

namespace Kaiju.State
{
    public class MoveState : IState<AI>
    {
        public AI parrent;
        public Player player;

        public void Enter(AI parrent)
        {
            this.parrent = parrent;
            player = GameWorld.Instance.player1;
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}
