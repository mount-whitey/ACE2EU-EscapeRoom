using UnityEngine;

public class ChangeMeshes: MonoBehaviour {

#if UNITY_EDITOR
    private void OnValidate() {

        if (TryGetComponent(out MeshFilter filter)) {

            var mesh = Resources.Load<Mesh>("Meshes/" + filter.sharedMesh.name);

            if (mesh != null) {
                filter.sharedMesh = mesh;
            }

            if (TryGetComponent(out MeshCollider collider)) {
                collider.sharedMesh = mesh;
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
