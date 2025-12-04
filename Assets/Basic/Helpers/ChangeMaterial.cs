using UnityEngine;

public class ChangeMaterial: MonoBehaviour {

#if UNITY_EDITOR
    private void OnValidate() {

        if (TryGetComponent(out MeshRenderer renderer)) {

            var mat = Resources.Load<Material>("Materials/" + renderer.sharedMaterial.name);

            if (mat != null) {
                renderer.sharedMaterial = mat;
            }
        }

        UnityEditor.EditorApplication.delayCall += () => {
            if (this != null) {
                DestroyImmediate(this);
            }
        };
    }
#endif
}
