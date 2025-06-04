using Kaiju.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.Command
{
    public class ChangeToProfileStateCommand : ICommand
    {
        private ISelectable state;
        private bool isPlayer1;

        public ChangeToProfileStateCommand(ISelectable state, bool isPlayer1)
        {
            this.state = state;
            this.isPlayer1 = isPlayer1;
        }
        public void Execute()
        {
            state.ChangeToProfileState(isPlayer1);
        }
    }
}
