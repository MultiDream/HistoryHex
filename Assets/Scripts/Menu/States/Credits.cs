using System;
using System.Collections;
using UnityEngine;


namespace HistoryHex {
    namespace MenuStates {
        public class Credits : MBState {

            public MBState main;

            public GameObject creditsMenu;

            private Action<IState> changeState;

            public void OnBackPressed() {
                changeState(main);
            }

            public override void Enter(IState previousState) {
                creditsMenu.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                creditsMenu.SetActive(false);
            }
        }
    }
}
