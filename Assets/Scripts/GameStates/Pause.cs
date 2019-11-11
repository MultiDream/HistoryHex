using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HistoryHex {
    namespace GameStates {
        public class Pause : MBState {
            public PlayerTurn player0;
            public PlayerTurn player1;

            public ConfirmExit confirmExit;

            public GameObject ui;

            private Action<IState> changeState;

            public void OnEndGamePressed() {
                changeState(confirmExit);
            }

            public void OnReturnToGame(int id) {
                if (id == 0) changeState(player0);
                else if (id == 1) changeState(player1);
                else throw new IndexOutOfRangeException("Player Id " + id + " out of range.");
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
