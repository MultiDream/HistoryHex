using UnityEngine;

namespace HistoryHex {
    public class SMInit : MonoBehaviour {

        public MBState initState;
        public UpdateCaller caller;
        private StateMachine stateMachine;

        public void Start() {
            stateMachine = gameObject.GetComponent<StateMachine>();

            StartStateMachine(caller);
        }

        public void StartStateMachine(UpdateCaller caller) {
            stateMachine.Enable(initState, caller);
        }
    }
}