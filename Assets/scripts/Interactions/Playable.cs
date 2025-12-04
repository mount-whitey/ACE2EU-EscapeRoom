using System.Collections;
using UnityEngine;

namespace ACE2EU {

    public class Playable: Interactable {

        [Header("Audio")]
        [SerializeField]
        private AudioClip _clip = null;

        [Header("Settings")]
        [SerializeField]
        private bool _isStopable = false;
        [SerializeField]
        private bool _isOnceOnly = false;

        [Header("Sound")]
        [SerializeField]
        [Range(0f, 1f)]
        private float _volume = 1;


        public bool WasPlayed {
            get {
                return _hasPlayed && !_source.isPlaying;
            }
        }

        AudioSource _source;
        bool _hasPlayed = false;

        private void Awake() {

            _source = gameObject.AddComponent<AudioSource>();

            _source.clip = _clip;
            _source.loop = false;
            _source.volume = _volume;
            _source.playOnAwake = false;
        }

        public override void Interact() {

            if (!enabled) {
                return;
            }

            if (_source.isPlaying) {

                if (!_isStopable) {
                    return;
                }

                _source.Stop();
            } else {

                if (_isOnceOnly && _hasPlayed) {
                    return;
                } else {
                    _hasPlayed = true;
                }

                _source.Play();
            }
        }

        public void Play() {

            if (_source.isPlaying) {
                return;
            }

            _source.Play();
        }

        public void Fade(float duration) {

            if (!_source.isPlaying) {
                return;
            }

            StartCoroutine(FadeAndStop(duration));
        }

        private IEnumerator FadeAndStop(float duration) {

            float time = 0;
            float volume = _source.volume;

            while (time < duration) {
                yield return null;
                _source.volume = Mathf.Clamp01(Mathf.Lerp(volume, 0, (time += Time.deltaTime) / duration));
            }

            _source.Stop();
        }
    }
}
