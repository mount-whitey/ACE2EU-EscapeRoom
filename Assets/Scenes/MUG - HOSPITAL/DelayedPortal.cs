using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace ACE2EU {

    public class DelayedPortal: Portal {

        private bool _started = false;

        [SerializeField]
        public UnityEvent<float> _onStart = null;

        public override void Interact() {

            if (_started) {
                return;
            } else {
                _onStart?.Invoke(Delay);
                _started = true;
            }

            StartCoroutine(WaitForDelay());
        }

        private IEnumerator WaitForDelay() {

            yield return new WaitForSeconds(Delay);

            PlayerController.Instance.FadeOut(Teleport);
        }
    }
}
