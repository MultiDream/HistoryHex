using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class ArbitrarySounds : MonoBehaviour {
        private AudioManager audioManager;

        public void Start() {
            audioManager = gameObject.GetComponentInParent<AudioManager>();
        }

        public void OnTurnEnd() {
            audioManager.Play2dEffect("turnEnd");
        }

        public void OnArmyMove() {
            audioManager.Play2dEffect("armyMove");
        }

        public void OnArmyAttack() {
            audioManager.Play2dEffect("armyAttack");
        }
    }
}
