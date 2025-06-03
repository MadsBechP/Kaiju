using Kaiju.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.Command
{
    public class ChangeSelectionCommand : ICommand
    {
        private int direction;
        private bool isPlayer1;
        private ISelectable state;


        public ChangeSelectionCommand(int direction, ISelectable state, bool isPlayer1)
        {
            this.direction = direction;
            this.state = state;
            this.isPlayer1 = isPlayer1;
        }
        public void Execute()
        {            
            state.ChangeSelection(direction, isPlayer1);
        }
    }

}
