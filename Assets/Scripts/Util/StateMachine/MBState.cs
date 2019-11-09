using System;
using UnityEngine;

namespace HistoryHex {
    public abstract class MBState : MonoBehaviour, IState {
        public abstract void Enter(IState previousState);
        public abstract void Execute(Action<IState> changeState);
        public abstract void Exit();
    }
}