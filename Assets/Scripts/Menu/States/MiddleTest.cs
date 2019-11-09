using System;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    namespace MenuStates {
        public class MiddleTest : MBState {

            private Action<IState> changeState;

            public GameObject restartTestButton;
            public GameObject endTestButton;

            public MBState mainState;
            public MBState endState;

            public void GoBackToStart() {
                changeState(mainState);
            }

            public void GoToEndState() {
                changeState(endState);
            }

            public override void Enter(IState previousState) {
                restartTestButton.SetActive(true);
                endTestButton.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                restartTestButton.SetActive(false);
                endTestButton.SetActive(false);
            }
        }
    }
}
