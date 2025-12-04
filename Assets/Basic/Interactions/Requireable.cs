using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ACE2EU {

    public class Requireable: Interactable {

        [Header("Requires")]
        [SerializeField]
        private StorableType Type = (StorableType)(-1);

        [Header("Parameters")]
        [SerializeField]
        private bool _allAtOnce = false;
        [SerializeField]
        private bool _RemoveItem = true;
        [SerializeField]
        private bool _onlyDirect = false;

        [Header("Events")]
        public UnityEvent _nothing;
        public UnityEvent _progress;
        public UnityEvent _finish;

        [Header("Locations")]
        [SerializeField]
        private List<Transform> _positions;

        protected int _remains = -1;
        private bool _reduced = false;

        public override void Interact() {

            if (!enabled) {
                return;
            }

            if (!gameObject.activeInHierarchy) {
                return;
            }

            if (_remains < 0) {
                return;
            }

            // Nothing
            if (Inventory.Instance.Contains(Type) <= 0 && !_reduced) {
                _nothing?.Invoke();
                return;
            }


            // Progress
            if (_remains > Inventory.Instance.Contains(Type)) {

                _progress?.Invoke();

                if (_allAtOnce) {
                    return;
                }
            }


            // Reduce
            int loops = _remains;

            for (int i = 0; i < loops; i++) {

                if (Inventory.Instance.Contains(Type) == 0) {
                    return;
                }

                if (_RemoveItem) {
                    var storable = Inventory.Instance.Remove(Type);

                    storable.transform.parent = _positions[_positions.Count - _remains];

                    storable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    storable.transform.localScale = Vector3.one;

                    if (storable is Placeable) {
                        (storable as Placeable).Place();
                    } else if (storable.transform.parent == null) {
                        Destroy(storable.gameObject);
                    } else {
                        Destroy(storable);
                    }
                }

                _reduced = true;
                _remains--;
            }

            if (_remains == 0 && _finish.GetPersistentEventCount() > 0) {
                _finish.Invoke();
                _remains--;
            }
        }

        protected virtual void Awake() {
            _remains = _positions.Count;
        }

        public void DecrementRemainByOne() {

            if (!enabled) {
                return;
            }

            if (!gameObject.activeInHierarchy) {
                return;
            }

            if (_remains > 0) {
                _reduced = true;
                _remains--;
            }

            if (!_onlyDirect) {
                Interact();
            }
        }

        public void IncrementRemainByOne() {

            _reduced = true;
            _remains++;
        }
    }
}
