using System;
using System.Collections;
using UnityEngine;


namespace HistoryHex {
    namespace MenuStates {
        public class Options : MBState {

            public MBState main;

            public GameObject optionsMenu;

            private Action<IState> changeState;

            public void OnBackPressed() {
                changeState(main);
            }

            public override void Enter(IState previousState) {
                optionsMenu.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                optionsMenu.SetActive(false);
            }
        }
    }
}
