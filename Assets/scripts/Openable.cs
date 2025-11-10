using UnityEngine;

namespace ACE2EU {

    public class Openable: Requireable {

        [Header("Defaults")]
        [SerializeField]
        protected bool _isOpen = false;

        protected bool _isLocked {
            get {
                return _remains > 0;
            }
        }

        Animator[] _animators;

        protected override void Awake() {
            base.Awake();

            _animators = GetComponentsInChildren<Animator>();
        }

        private void Start() {

            if (_isOpen) {
                Trigger();
            }
        }

        public override void Interact() {
            base.Interact();

            if (_isLocked) {
                return;
            }

            Trigger();
            _isOpen = !_isOpen;
        }

        private void Trigger() {
            foreach (var anim in _animators) {
                anim.SetTrigger("Interact");
            }
        }
    }
}
