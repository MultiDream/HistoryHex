using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace HistoryHex {
    namespace GameStates {
        public class GameEnd : MBState {
            public GameObject ui;
            public TextMeshProUGUI winner;
            public GameObject returnToMenu;
            public GameObject tileUi;

            private int winnerId = -0xFF;

            private Action<IState> changeState;

            private IEnumerator KickOffTimer() {
                yield return new WaitForSeconds(3.0f);
                returnToMenu.SetActive(true);
            }

            public void OnReturnToMenuPressed() {
                SceneManager.LoadScene("Scenes/TitleScreen");
            }

            public void SetDisplayResults(int winnerId) {
                this.winnerId = winnerId;
            }

            public override void Enter(IState previousState) {
                ui.SetActive(true);
                tileUi.SetActive(false);
                winner.SetText("Player " + (winnerId + 1) + " wins");
                returnToMenu.SetActive(false);
                StartCoroutine(KickOffTimer());
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
