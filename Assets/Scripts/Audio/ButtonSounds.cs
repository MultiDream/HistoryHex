using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class ButtonSounds : MonoBehaviour {
        private AudioManager audioManager;

        public void Start() {
            audioManager = gameObject.GetComponentInParent<AudioManager>();
        }

        public void OnButtonPress() {
            audioManager.Play2dEffect("buttonPress");
        }

        public void OnButtonHover() {
            audioManager.Play2dEffect("buttonHover");
        }
    }
}
