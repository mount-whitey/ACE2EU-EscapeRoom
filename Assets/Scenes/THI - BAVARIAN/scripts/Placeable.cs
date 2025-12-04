using UnityEngine;
using UnityEngine.Events;

namespace ACE2EU {

    public class Placeable: Storable {

        [Header("Correct")]
        [SerializeField]
        private Transform _parent;

        [Header("Events")]
        [SerializeField]
        private UnityEvent _onPickup;
        [SerializeField]
        private UnityEvent _onPutDown;
        [SerializeField]
        private UnityEvent _onWrong;
        [SerializeField]
        private UnityEvent _onRight;

        private Vector3 _position;
        private Quaternion _rotation;

        private bool _wasCorrect = false;

        public override void Interact() {

            if (!enabled) {
                return;
            }

            if (Inventory.Instance.Contains(Type) != 0) {
                PlayerController.Instance.ShowInformation("I should place my current <b>car</b> first,\nbefore picking up another.");
                return;
            }

            // Event
            _onPickup?.Invoke();

            // Orientation 
            _position = transform.localPosition;
            _rotation = transform.localRotation;

            // Visual
            transform.parent.parent.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(true);

            // Logic
            base.Interact();
        }

        public void Place() {

            // Event
            _onPutDown?.Invoke();

            // Orientation 
            transform.SetLocalPositionAndRotation(_position, _rotation);

            // Visual
            transform.parent.parent.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(false);

            // Logic
            if (transform.parent == _parent) {

                if (!_wasCorrect) {
                    _wasCorrect = true;
                    _onRight?.Invoke();
                }
            } else {

                if (_wasCorrect) {
                    _wasCorrect = false;
                    _onWrong?.Invoke();
                }
            }

            transform.GetComponentInParent<Requireable>(true).IncrementRemainByOne();
        }
    }
}
