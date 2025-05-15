using Kaiju.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
