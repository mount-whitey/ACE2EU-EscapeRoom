using GLTFast.Schema;
using UnityEngine;

namespace ACE2EU {

    public class Animatable: Requireable {

        [Header("Defaults")]
        [SerializeField]
        protected bool _isOpen = false;

        protected bool _isLocked {
            get {
                return _remains > 0 || (_remains < 0 && _isOpen);
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

                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && !anim.IsInTransition(0)) {
                    return;
                }

                anim.SetTrigger("Interact");
            }
        }
    }
}
