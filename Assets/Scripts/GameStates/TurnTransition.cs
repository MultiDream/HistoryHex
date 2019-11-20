using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace HistoryHex {
    namespace GameStates {
        public class TurnTransition : MBState {
            public PlayerTurn nextTurnState;

            private bool endTransition = false;

            public GameObject panel;
            public TextMeshProUGUI text;

            private IEnumerator KickOffTimer() {
                yield return new WaitForSeconds(0.75f);
                endTransition = true;
            }

            public override void Enter(IState previousState) {
                StartCoroutine(KickOffTimer());
                panel.SetActive(true);
                text.SetText("Player " + (nextTurnState.id + 1) + " Turn");
                endTransition = false;
            }

            public override void Execute(Action<IState> changeState) {
                if (endTransition) changeState(nextTurnState);
            }

            public override void Exit() {
                panel.SetActive(false);
            }
        }
    }
}
