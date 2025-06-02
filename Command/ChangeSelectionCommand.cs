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
        private MenuState menu;
        private bool isPlayer1;

        public ChangeSelectionCommand(int direction, MenuState menu, bool isPlayer1)
        {
            this.direction = direction;
            this.menu = menu;
            this.isPlayer1 = isPlayer1;
        }
        public void Execute()
        {
            menu.ChangeSelection(direction, isPlayer1);
        }
    }

}
