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

			public void OnVolumeOptionChange(float value){
				PlayerPrefs.SetFloat("Volume",value);
			}

			public void OnMapSizeOptionChange(float value) {
				//Need to transform appropriately.
				int transformedValue = Mathf.FloorToInt(value * 5.0f) + 1;
				PlayerPrefs.SetInt("MapSize", transformedValue);
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
