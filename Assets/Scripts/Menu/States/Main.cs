using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HistoryHex {
    namespace MenuStates {
        public class Main : MBState {

            public MBState options;
            public MBState credits;

            public GameObject mainMenu;

            private Action<IState> changeState;

            public void OnStartPressed() {
                SceneManager.LoadScene("Scenes/DevelopmentSceneArt");
            }

            public void OnCreditsPressed() {
                changeState(credits);
            }

            public void OnOptionsPressed() {
                changeState(options);
            }

            public override void Enter(IState previousState) {
                mainMenu.SetActive(true);
            }

            public override void Execute(Action<IState> changeState) {
                this.changeState = changeState;
            }

            public override void Exit() {
                mainMenu.SetActive(false);
            }
        }
    }
}
