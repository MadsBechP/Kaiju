using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.State
{
    /// <summary>
    /// An interface that allow selection and confirmation of options.
    /// Used for character and profile selection.
    /// Made by Emilie and Julius
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Changes the current selection in a given direction.
        /// </summary>
        /// <param name="direction">The direction to move the selection</param>
        /// <param name="isPlayer1">Checks whether the selection is for player 1 (true) or player 2 (false)</param>
        void ChangeSelection(int direction, bool isPlayer1);
        
        /// <summary>
        /// Confirms the current selection for the player 
        /// </summary>
        /// <param name="isPlayer1">Checks whether the selection is for player 1 (true) or player 2 (false)</param>
        void ConfirmSelection(bool isPlayer1);

        void ChangeToProfileState(bool isPlayer1);
    }
}
