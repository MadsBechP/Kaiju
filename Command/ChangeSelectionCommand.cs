using Kaiju.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.Command
{
    /// <summary>
    /// A command uses to change the current selection in a selectable game state.
    /// Implements the ICommand interface.
    /// Made by Emilie
    /// </summary>
    public class ChangeSelectionCommand : ICommand
    {
        private int direction;
        private bool isPlayer1;
        private ISelectable state;

        /// <summary>
        /// Initialize a new instance of the ChangeSelectionCommand class.
        /// </summary>
        /// <param name="direction">The direction to change selection (-1 = Up/Left, 1 = Down/Right)</param>
        /// <param name="state">The selectable state where the selection change will find place</param>
        /// <param name="isPlayer1">Checks which player is changing selection. (P1 = true, P2 = false)</param>
        public ChangeSelectionCommand(int direction, ISelectable state, bool isPlayer1)
        {
            this.direction = direction;
            this.state = state;
            this.isPlayer1 = isPlayer1;
        }

        /// <summary>
        /// Executes the command by calling the selectable state's ChangeSelection 
        /// with the specified direction for the correct player.
        /// </summary>
        public void Execute()
        {            
            state.ChangeSelection(direction, isPlayer1);
        }
    }

}
