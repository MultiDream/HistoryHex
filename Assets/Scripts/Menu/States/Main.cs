using System;
using System.Collections;
using UnityEngine;


namespace HistoryHex {
    namespace MenuStates {
        public class Main : MBState {

            public MBState startTest;

            private bool leaveState = false;

            public IEnumerator KickOffTimer() {
                yield return new WaitForSeconds(5.0f);
                leaveState = true;
            }

            public override void Enter(IState previousState) {
                StartCoroutine(KickOffTimer());
            }

            public override void Execute(Action<IState> changeState) {
                if (leaveState) changeState(startTest);
            }

            public override void Exit() {
                leaveState = false;
            }
        }
    }
}
