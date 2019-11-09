using System;
using System.Collections.Generic;
using UnityEngine;


namespace HistoryHex {
    namespace MenuStates {
        public class StartTest : MBState {

            private Action<IState> changeState;

            public GameObject startTestButton;

            public MBState middleState;

            public void ButtonPressed() {
                changeState(middleState);
            }

            public override void Enter(IState previousState) {
                startTestButton.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                startTestButton.SetActive(false);
            }
        }
    }
}
