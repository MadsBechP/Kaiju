using Kaiju.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.Command
{
    /// <summary>
    /// Represents a command that changes gamestate when called
    /// Implements the ICommand interface as part of the Command Design Pattern
    /// Made by: Julius
    /// </summary>
    public class ChangeToProfileStateCommand : ICommand
    {
        private ISelectable state;
        private bool isPlayer1;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="state">The state the command should be done in</param>
        /// <param name="isPlayer1">bool if it is player one doing the command</param>
        public ChangeToProfileStateCommand(ISelectable state, bool isPlayer1)
        {
            this.state = state;
            this.isPlayer1 = isPlayer1;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        public void Execute()
        {
            state.ChangeToProfileState(isPlayer1);
        }
    }
}
