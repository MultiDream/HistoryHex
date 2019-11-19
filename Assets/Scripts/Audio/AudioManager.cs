using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class AudioManager : MonoBehaviour {
        [System.Serializable]
        public struct AudioMap {
            public AudioClip buttonHover;
            public AudioClip buttonPress;

            public AudioClip turnEnd;

            public AudioClip armyMove;
            public AudioClip armyAttack;

            public AudioClip ambienceOcean;

            public AudioClip musicMenu;
            public AudioClip musicGame;
        };

        public AudioSource uiSoundSource;
        public AudioSource oceanSource;
        public AudioSource musicSource;
        public AudioMap audioMap;

        private Dictionary<string, AudioClip> audioDict;

        private float volumeScale = 1.0f;

        public void Start() {
            audioDict = new Dictionary<string, AudioClip>();

            volumeScale = PlayerPrefs.GetFloat("volume", 1.0f);
            oceanSource.volume *= volumeScale;
            musicSource.volume *= volumeScale;

            audioDict.Add(nameof(audioMap.buttonHover), audioMap.buttonHover);
            audioDict.Add(nameof(audioMap.buttonPress), audioMap.buttonPress);

            audioDict.Add(nameof(audioMap.turnEnd), audioMap.turnEnd);

            audioDict.Add(nameof(audioMap.armyMove), audioMap.armyMove);
            audioDict.Add(nameof(audioMap.armyAttack), audioMap.armyAttack);

            audioDict.Add(nameof(audioMap.ambienceOcean), audioMap.ambienceOcean);

            //audioDict.Add(nameof(audioMap.musicMenu), audioMap.musicMenu);
            //audioDict.Add(nameof(audioMap.musicGame), audioMap.musicGame);
        }

        public void Play2dEffect(string id, float oneOffVolumeScale = 1.0f, float startTime = 0.0f) {
            AudioClip soundEffect;
            audioDict.TryGetValue(id, out soundEffect);

            uiSoundSource.time = startTime;
            uiSoundSource.PlayOneShot(soundEffect, volumeScale * oneOffVolumeScale);
        }

        public void StopSound() {
            uiSoundSource.Stop();
        }

        public void SetVolumeScale(float volumeScale) {
            this.volumeScale = volumeScale;

            PlayerPrefs.SetFloat("volume", volumeScale);
        }
    }
}