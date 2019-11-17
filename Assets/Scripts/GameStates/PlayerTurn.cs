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

            public UIMaster tileUi;

            public ArbitrarySounds sounds;

            private Action<IState> changeState;

            public void OnTurnEnd() {
                sounds.OnTurnEnd();
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
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                // TODO: ui.Disable();
                Debug.Log("PLAYER TURN EXIT");
                //tileUi.UnregisterUIComponent();
                //      ^ this line is supposed to be a way to make the UI for a tile disappear.\
            }
        }
    }
}
