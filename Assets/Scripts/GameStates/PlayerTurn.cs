using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace HistoryHex {
    namespace GameStates {
        public class PlayerTurn : MBState {

            public int id;

            public MBState transition;
            public MBState gameEnd;

            public GameObject endTurnButton;

            private Action<IState> changeState;

            public void OnTurnEnd() {
                changeState(transition);
            }

            public void OnPause(IState pauseState) {
                changeState(pauseState);
            }

            public void OnGameEnd() {
                changeState(gameEnd);
            }

            public override void Enter(IState previousState) {
                // TODO: ui.SetForPlayer(id);
                endTurnButton.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                // TODO: ui.Disable();
                endTurnButton.SetActive(false);
            }
        }
    }
}
