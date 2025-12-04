using UnityEngine;

namespace ACE2EU {

    public class StorageManager: MonoBehaviour {

        public static StorageManager Instance = null;

        public bool FirstEncounter { get; set; } = true;

        public Pose InitialPose { get; set; } = new Pose(new Vector3(86.3f, 2.8f, 56), Quaternion.identity);

        private void Awake() {
            
            if(Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
