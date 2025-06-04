using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.State
{
    public interface ISelectable
    {
        void ChangeSelection(int direction, bool isPlayer1);
        void ChangeToProfileState(bool isPlayer1);
        void ConfirmSelection(bool isPlayer1);
    }
}
