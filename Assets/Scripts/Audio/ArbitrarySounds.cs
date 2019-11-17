using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class ArbitrarySounds : MonoBehaviour {
        private AudioManager audioManager;

        public void Start() {
            audioManager = gameObject.GetComponentInParent<AudioManager>();
        }

        private IEnumerator StopPlaying() {
            yield return new WaitForSeconds(1.0f);
            audioManager.StopSound();
        }

        public void OnTurnEnd() {
            audioManager.Play2dEffect("turnEnd");
        }

        public void OnArmyMove() {
            audioManager.Play2dEffect("armyMove", 1.0f, Random.value * 20.0f);
            StartCoroutine(StopPlaying());
        }

        public void OnArmyAttack() {
            audioManager.Play2dEffect("armyAttack", 1.0f, Random.value * 10.0f);
            StartCoroutine(StopPlaying());
        }
    }
}
