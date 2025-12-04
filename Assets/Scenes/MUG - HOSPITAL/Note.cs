
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ACE2EU {

    public class Note: Storable {

        private void OnValidate() {
            Type = StorableType.Note;
    }

        public override void Interact() {

            if (Inventory.Instance.Contains(Type) > 0) {
                var note = Inventory.Instance.Remove(Type);

                note.GetComponentInChildren<TMP_Text>(true).text = "3 7 1 9";

                Inventory.Instance.Add(note);
            }

            base.Interact();
        }
    }
}
