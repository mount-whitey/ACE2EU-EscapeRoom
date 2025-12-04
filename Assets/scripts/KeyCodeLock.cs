using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ACE2EU {

    public class KeyCodeLock: MonoBehaviour {
        public GameObject keycodePanel; // UI Panel for the key code lock
        public TextMeshProUGUI inputDisplay; // Display for entered numbers
        public Image codeImage; // Image containing the random code
        public TextMeshProUGUI codeText; // Text that displays the code on the image
        public GameObject paper; // Paper object to be collected
        public Button[] numberButtons; // Array of buttons for number input
        public Button enterButton;
        public Button clearButton;
        private string correctCode;
        private string playerInput = "";

        public AdvancedDoorKeyLock doorScript; // Reference to the door script
        public PlayerController playerMovement; // Reference to PlayerMovement script

        private bool isNearKeypad = false;

        void Start() {
            keycodePanel.SetActive(false); // Hide keycode panel at start
            codeImage.gameObject.SetActive(false); // Hide the code image at start
            codeText.gameObject.SetActive(false); // Hide the code text until paper is picked up
            GenerateRandomCode();
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider.CompareTag("Keypad")) {
                        ToggleKeypad();
                    } else if (hit.collider.CompareTag("Paper")) {
                        DestroyPaperAndRevealCode();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) && keycodePanel.activeSelf) {
                CloseKeypad();
            }
        }

        private void GenerateRandomCode() {
            correctCode = "";
            for (int i = 0; i < 4; i++) {
                correctCode += Random.Range(0, 10).ToString();
            }
            if (codeText != null) {
                codeText.text = "Code: " + correctCode; // Update the text with the generated code
            }
        }

        public void DestroyPaperAndRevealCode() {
            if (paper != null) {
                Destroy(paper); // Remove the paper from the scene
            }
            if (codeText != null) {
                codeText.gameObject.SetActive(true); // Reveal the code text
            }
            if (codeImage != null) {
                codeImage.gameObject.SetActive(true); // Reveal the code image
            }
        }

        private void ToggleKeypad() {
            bool isOpening = !keycodePanel.activeSelf;
            keycodePanel.SetActive(isOpening);
            Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpening;

            if (playerMovement != null) {
                playerMovement.enabled = !isOpening; // Disable movement when the keypad is open
            }
        }

        public void EnterDigit(string digit) {
            if (playerInput.Length < 4) {
                playerInput += digit;
                inputDisplay.text = playerInput;
            }
        }

        public void ClearInput() {
            playerInput = "";
            inputDisplay.text = "";
        }

        public void SubmitCode() {
            if (playerInput == correctCode) {
                if (doorScript != null) {
                    doorScript.isCodeUnlocked = true;
                }
                CloseKeypad();
                Destroy(gameObject); // Destroy the key code lock
            } else {
                ClearInput();
            }
        }

        public void CloseKeypad() {
            keycodePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClearInput();

            if (playerMovement != null) {
                playerMovement.enabled = true; // Enable movement when closing the keypad
            }
        }
    }
}
