using System;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class UpdateCaller : MonoBehaviour {

        Action callback;

        public void SetUpdate(Action callback) {
            this.callback = callback;
            this.enabled = true;
        }

        public void Update() {
            callback();
        }
    }
}