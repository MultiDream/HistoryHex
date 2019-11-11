using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HistoryHex {
    namespace GameStates {
        public class ConfirmExit : MBState {
            public Pause pause;

            public GameObject ui;

            private Action<IState> changeState;

            public void OnEndGamePressed() {
                SceneManager.LoadScene("Scenes/Menus/MainMenu");
            }

            public void OnCancelPressed() {
                changeState(pause);
            }

            public override void Enter(IState previousState) {
                ui.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                ui.SetActive(false);
            }
        }
    }
}
