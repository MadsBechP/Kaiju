namespace Kaiju.State
{
    public interface IState<T>
    {
        void Enter(T parrent);
        void Exit();
        void Execute();
    }
}
