using System;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class StateMachine : MonoBehaviour {

#if UNITY_EDITOR
        public string currStateName;
#endif

        private IState curr;
        private List<IState> nextStates;
        private Action<IState> changeStateCallback;
        private bool changeState = false;

        public void Execute() {
            try {
#if UNITY_EDITOR
                currStateName = curr.GetType().Name;
#endif
                curr.Execute(changeStateCallback);
                if (changeState) {
                    ChangeState();
                }
            }
            catch (Exception ex) {
                Debug.LogError("State Machine threw Exception");
                Debug.LogError("  error in state: " + curr.ToString());
                Debug.LogError(ex);
                this.enabled = false;
            }
        }

        public IState GetCurrentState() {
            return curr;
        }

        public void Enable(IState startState, UpdateCaller caller) {
            Debug.Log("Enable Called");
            nextStates = new List<IState>();
            changeStateCallback = (IState nextState) => {
                changeState = true;
                nextStates.Add(nextState);
            };
            //this.enabled = true;

            caller.SetUpdate(new Action(Execute));
            startState.Enter(null);
            curr = startState;
        }

        private void ChangeState() {
            IState nextState = nextStates[0];
            nextStates.RemoveAt(0);

            curr.Exit();
            nextState.Enter(curr);

            curr = nextState;

            if (nextStates.Count == 0) {
                changeState = false;
            }
        }
    }
}