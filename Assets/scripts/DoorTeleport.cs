using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTeleport : MonoBehaviour
{
    [Tooltip("Name of the scene to load when door is clicked.")]
    public string sceneToLoad = "NextScene";

    [Tooltip("Display name of the door.")]
    public string doorDisplayName = "Mysterious Door";

    void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name not set in DoorTeleport script.");
        }
    }

    // Method to get the door's name
    public string GetDoorName()
    {
        return doorDisplayName;
    }
}
