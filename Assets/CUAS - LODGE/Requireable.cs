using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ACE2EU {

    public class Requireable: Interactable {

        [Header("Requires")]
        [SerializeField]
        private StorableType Type;

        [Header("Parameters")]
        [SerializeField]
        private bool _allAtOnce = false;

        [Header("Succeded")]
        public UnityEvent _finish;

        [Header("Locations")]
        [SerializeField]
        private List<Transform> _positions;

        protected int _remains = 0;

        public override void Interact() {

            if (_remains <= 0) {
                return;
            }

            if (_allAtOnce && _remains > Inventory.Instance.Contains(Type)) {
                return;
            }

            int max = _remains;

            for (int i = 0; i < max; i++) {

                if (Inventory.Instance.Contains(Type) == 0) {
                    return;
                }

                var storable = Inventory.Instance.Remove(Type);

                storable.transform.parent = _positions[_positions.Count - _remains];

                storable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                storable.transform.localScale = Vector3.one;

                Destroy(storable);

                _remains--;
            }

            if (_remains == 0) {
                _finish.Invoke();
            }
        }

        protected virtual void Awake() {
            _remains = _positions.Count;
        }
    }
}
