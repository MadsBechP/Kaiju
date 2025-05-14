using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.State
{
    public interface IState<T>
    {
        void Enter(Task parrent);
        void Exit();
        void Execute();
    }
}
