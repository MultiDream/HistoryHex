using System;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    namespace MenuStates {
        public class EndTest : MBState {

            public GameObject overLabel;

            public override void Enter(IState previousState) {
                overLabel.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                // do nothing
            }

            public override void Exit() {
                // do nothing
            }
        }
    }
}
