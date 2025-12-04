using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace ACE2EU {

    public class SkiStart: Portal {

        [Header("Take With")]
        [SerializeField]
        private List<Transform> _attachees;

        [Header("Clear This")]
        [SerializeField]
        private List<Transform> _deactees;

        [Header("Event")]
        [SerializeField]
        private UnityEvent<float> _onStart;


        private bool _started = false;

        public override void Interact() {

            if (_started) {
                return;
            }else {
                _started = true;
            }

            _onStart?.Invoke(Delay);
            StartCoroutine(WaitForDelay());

            foreach (Transform attachee in _attachees) {
                attachee.parent = PlayerController.Instance.transform;
            }

            foreach (Transform deactee in _deactees) {
                deactee.gameObject.SetActive(false);
            }

            PlayerController.Instance.JustForward(true);
        }

        private IEnumerator WaitForDelay() {

            yield return new WaitForSeconds(Delay);

            Teleport();
        } 
    }
}
