using UnityEngine;

public class AddMeshCollider : MonoBehaviour
{

#if UNITY_EDITOR
    private void OnValidate() {

        if (TryGetComponent(out MeshFilter filter)) {

            if (!TryGetComponent(out MeshCollider collider)) {
                gameObject.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
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
