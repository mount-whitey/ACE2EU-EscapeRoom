using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();

    private void OnValidate() {
        SetLayerRecursiv(transform);
    }

    private void SetLayerRecursiv(Transform parent) {

        foreach (Transform child in parent) {
            SetLayerRecursiv(child);
        }

        parent.gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
