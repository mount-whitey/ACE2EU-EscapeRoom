using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACE2EU {

    public class Portal: Interactable {

        [Header("Scene of Destination")]
        [SerializeField]
        private string _scene = "";

        [Header("Waiting Time")]
        [SerializeField]
        public float Delay = 0;

        public override void Interact() {
            Teleport();
        }

        protected virtual void Teleport() {
            if (SceneManager.GetSceneByName(_scene) == null) {
                Debug.LogError("Scene not available - " + _scene);
                return;
            }

            if (SceneManager.GetActiveScene().name == "SCHOOL") {

                Vector3 port = transform.position;
                Vector3 diff = transform.TransformDirection(0, 1f, -2);

                StorageManager.Instance.InitialPose = new Pose(port + diff, transform.rotation * Quaternion.Euler(0, 180, 0));
            }

            SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
        }
    }
}
