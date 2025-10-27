using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [SerializeField] private float pushForce = 5f;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        // Only push if the object has a Rigidbody and it's not kinematic
        if (rb != null && !rb.isKinematic)
        {
            // Only push horizontally
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }
    }
}
