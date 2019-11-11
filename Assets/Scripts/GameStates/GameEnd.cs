using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HistoryHex {
    namespace GameStates {
        public class GameEnd : MBState {
            public GameObject ui;
            public GameObject returnToMenu;

            private Action<IState> changeState;

            private IEnumerator KickOffTimer() {
                yield return new WaitForSeconds(3.0f);
                returnToMenu.SetActive(true);
            }

            public void OnReturnToMenuPressed() {
                SceneManager.LoadScene("Scenes/Menus/MainMenu");
            }

            public override void Enter(IState previousState) {
                returnToMenu.SetActive(false);
                StartCoroutine(KickOffTimer());
                // TODO: ui.DisplayWinner();
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                // do nothing
            }
        }
    }
}
