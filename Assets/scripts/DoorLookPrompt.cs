using UnityEngine;
using TMPro;

public class DoorLookPrompt : MonoBehaviour
{
    public float lookDistance = 5f;
    public string doorTag = "Door";
    public TextMeshProUGUI promptText;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            if (hit.collider.CompareTag(doorTag))
            {
                DoorTeleport doorScript = hit.collider.GetComponent<DoorTeleport>();
                if (doorScript != null && promptText != null)
                {
                    promptText.text = doorScript.GetDoorName();
                    promptText.gameObject.SetActive(true);
                }
                return;
            }
        }

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }
}
