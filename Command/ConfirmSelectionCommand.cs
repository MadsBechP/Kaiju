using Kaiju.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.Command
{
    /// <summary>
    /// A command that confirms a player's current selection in a selectable state.
    /// Implements the ICommand interface.
    /// Made by Emilie
    /// </summary>
    public class ConfirmSelectionCommand : ICommand
    {
        private ISelectable state;
        private bool isPlayer1;

        /// <summary>
        /// Initialize a new instance of the ConfirmSelectionCommand class.
        /// </summary>
        /// <param name="state">The selectable state where a confirmation will happen.</param>
        /// <param name="isPlayer1">If it is player 1 confirming then it will be True. If it is player 2 then False</param>
        public ConfirmSelectionCommand(ISelectable state, bool isPlayer1)
        {
            this.state = state;
            this.isPlayer1 = isPlayer1;
        }

        /// <summary>
        /// Executes the confirmation command by calling ConfirmationSelection on the state.
        /// </summary>
        public void Execute()
        {
            state.ConfirmSelection(isPlayer1);
        }
    }
}
