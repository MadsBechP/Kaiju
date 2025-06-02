using Kaiju.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.Command
{
    public class ConfirmSelectionCommand : ICommand
    {
        private MenuState menu;
        private bool isPlayer1;

        public ConfirmSelectionCommand(MenuState menu, bool isPlayer1)
        {
            this.menu = menu;
            this.isPlayer1 = isPlayer1;
        }
        public void Execute()
        {
            menu.ConfirmSelection(isPlayer1);
        }
    }
}
