using UnityEngine;
using UnityEngine.Events;

namespace ACE2EU {

    public class Portal: Interactable {

        [Header("Scene of Destination")]
        [SerializeField]
        private Scene _scene = (Scene)(-1);

        [Header("Waiting Time")]
        [SerializeField]
        public float Delay = 0;

        [Header("Events")]
        [SerializeField]
        public UnityEvent OnPortalIsLoading = null;
        [SerializeField]
        public UnityEvent OnPortalIsActive = null;

        private void Start() {

            if ((int)_scene < 0) {
                return;
            }

            OverAllManager.Instance.OnSceneGetsLoaded += scene => {
                if (_scene == scene) {
                    OnPortalIsLoading?.Invoke();
                }
            };

            OverAllManager.Instance.OnSceneAvailable += scene => {
                if (_scene == scene) {
                    OnPortalIsActive?.Invoke();
                }
            };

            if (OverAllManager.Instance.IsSceneAvailable(_scene)) {
                OnPortalIsActive?.Invoke();
            } 
        }

        public override void Interact() {
            Teleport();
        }

        public virtual void Teleport() {

            // NOW
            OverAllManager.Instance.LoadScene(_scene);

            // NEXT
            if (_scene != Scene.BASE) {
                OverAllManager.Instance.InitialPose = new Pose(transform.position + transform.TransformDirection(0, 1f, -2), transform.rotation * Quaternion.Euler(0, 180, 0));
            } else {
                OverAllManager.Instance.InitialPose = new Pose(Vector3.zero, Quaternion.identity);
            }
        }
    }
}
