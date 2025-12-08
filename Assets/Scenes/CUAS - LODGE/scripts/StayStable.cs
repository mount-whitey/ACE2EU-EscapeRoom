using UnityEngine;

namespace ACE2EU {

    public class StayStable: MonoBehaviour {
        Quaternion _rotation;

        void Start() {
            _rotation = transform.rotation;
        }

        void Update() {
            transform.rotation = _rotation;
        }
    }
}
