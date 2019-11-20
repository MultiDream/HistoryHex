using System;

namespace HistoryHex {
    public interface IState {
        void Enter(IState previousState);
        void Execute(Action<IState> changeState);
        void Exit();
    }
}