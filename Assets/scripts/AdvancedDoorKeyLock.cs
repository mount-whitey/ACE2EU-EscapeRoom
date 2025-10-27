using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AdvancedDoorKeyLock : MonoBehaviour
{
    // Key and code lock variables
    private bool hasKey = false;
    public bool isCodeUnlocked = false; // Handled externally by the key code lock system
    public bool isSecondLockUnlocked = false; // Second lock condition
    public bool isBarrierRemoved = false; // Barrier condition

    // UI elements
    public TextMeshProUGUI messageText;
    public GameObject key;
    public GameObject secondKey;
    public GameObject secondLock;

    // UI checkmarks
    public GameObject barrierCheckmark; // Checkmark for the barrier

    // Scene to load when the door is unlocked
    public string nextSceneName;

    void Start()
    {
        // Clear the message text at the start
        if (messageText != null)
        {
            messageText.text = "";
        }

        // Ensure the barrier checkmark is hidden initially
        if (barrierCheckmark != null)
        {
            barrierCheckmark.SetActive(false);
        }
    }

    void Update()
    {
        // Detect player clicks
        if (Input.GetKeyDown(KeyCode.E)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    OnDoorClicked();
                }
                else if (hit.collider.CompareTag("Key"))
                {
                    OnKeyCollected();
                }
                else if (hit.collider.CompareTag("SecondKey"))
                {
                    OnSecondKeyCollected();
                }
                else if (hit.collider.CompareTag("SecondLock"))
                {
                    OnSecondLockUnlocked();
                }
                else if (hit.collider.CompareTag("Barrier"))
                {
                    OnBarrierDestroyed(hit.collider.gameObject);
                }
            }
        }
    }

    private void OnDoorClicked()
    {
        if (hasKey && isCodeUnlocked && isSecondLockUnlocked && isBarrierRemoved)
        {
            // Unlock the door and load the next scene
            SceneManager.LoadScene(nextSceneName);
        }
        else if (!hasKey)
        {
            DisplayMessage("You need a key to open the door.");
        }
        else if (!isCodeUnlocked)
        {
            DisplayMessage("The door is locked. Unlock the code first.");
        }
        else if (!isSecondLockUnlocked)
        {
            DisplayMessage("Another lock is blocking the door. Unlock it first.");
        }
        else if (!isBarrierRemoved)
        {
            DisplayMessage("The barrier is still in place. Remove it first.");
        }
    }

    private void OnKeyCollected()
    {
        hasKey = true;
        if (key != null)
        {
            Destroy(key); // Remove the key from the scene
        }
        DisplayMessage("Key collected!");
    }

    private void OnSecondKeyCollected()
    {
        isSecondLockUnlocked = true;
        if (secondKey != null)
        {
            Destroy(secondKey); // Remove the second key from the scene
        }
        DisplayMessage("Second key collected! You can now unlock the second lock.");
    }

    private void OnSecondLockUnlocked()
    {
        if (isSecondLockUnlocked)
        {
            if (secondLock != null)
            {
                Destroy(secondLock); // Remove the second lock from the scene
            }
            DisplayMessage("Second lock removed! The door is now accessible.");
        }
        else
        {
            DisplayMessage("You need the second key to unlock this lock.");
        }
    }

    public void OnBarrierDestroyed(GameObject barrier)
    {
        isBarrierRemoved = true;
        if (barrier != null)
        {
            Destroy(barrier); // Remove the barrier from the scene
        }
        if (barrierCheckmark != null)
        {
            barrierCheckmark.SetActive(true); // Show the checkmark
        }
        DisplayMessage("Barrier removed! The path is clear.");
    }

    private void DisplayMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            CancelInvoke("ClearMessage");
            Invoke("ClearMessage", 3f); // Clear the message after 3 seconds
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}
