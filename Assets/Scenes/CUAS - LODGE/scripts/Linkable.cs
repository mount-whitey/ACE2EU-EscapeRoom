using UnityEngine;

namespace ACE2EU {

    public class Linkable: Interactable {

        [Header("Website")]
        [SerializeField]
        string _link = "";

        public override void Interact() {

            if(_link == "") {
                return;
            }

            Application.OpenURL(_link);
        }
    }
}

