using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController: MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform camera;
    [SerializeField] private Transform crouchedCameraTarget;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float clampLookY = 90f;

    [Header("Key Binds")]
    [SerializeField] private KeyCode keyRun = KeyCode.LeftShift;
    [SerializeField] private KeyCode keyJump = KeyCode.Space;
    [SerializeField] private KeyCode keyCrouch = KeyCode.LeftControl;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchedHeight = 1f;

    [Header("Debug/Testing")]
    [SerializeField] private KeyCode toggleMouseLockKey = KeyCode.Escape;

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 moveDirection = Vector3.zero;

    private float originalHeight;
    private Vector3 originalCameraPos;

    private bool _isMobileDevice = false;

    void Start() {
        controller = GetComponent<CharacterController>();
        controller.center = Vector3.zero;

        originalHeight = controller.height;
        originalCameraPos = camera.localPosition;

        LockCursor();

        leftStickHandle.gameObject.SetActive(false);
        rightStickHandle.gameObject.SetActive(false);
    }

    void Update() {

        if (_isMobileDevice |= Input.touchCount > 0) {
            // Mobile
            HandleTouchInput();

        } else {
            // PC

            ToggleMouseLock();

            if (Cursor.lockState == CursorLockMode.Locked) {
                ProcessLook();
            }

            ProcessMovement();
            ProcessCrouch();

            if (Input.GetMouseButtonDown(0)) {
                ProcessInteractions();
            }
        }
    }

    void ProcessLook() {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void ProcessMovement() {

        if (controller.isGrounded) {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            Vector3 input = new(horizontalInput, 0f, verticalInput);
            Vector3 move = transform.TransformDirection(input.normalized);

            float speed = walkSpeed;
            if (Input.GetKey(keyCrouch)) speed = crouchSpeed;
            else if (Input.GetKey(keyRun)) speed = runSpeed;

            moveDirection = move * speed;

            if (Input.GetKeyDown(keyJump) && !Input.GetKey(keyCrouch)) {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ProcessCrouch() {
        float targetHeight = Input.GetKey(keyCrouch) ? crouchedHeight : originalHeight;
        Vector3 targetCamPos = Input.GetKey(keyCrouch) ? crouchedCameraTarget.localPosition : originalCameraPos;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
        camera.localPosition = Vector3.Lerp(camera.localPosition, targetCamPos, Time.deltaTime * 10f);

        controller.center = new Vector3(0f, -0.28f, 0f);
    }

    // === Toggle Mouse Lock for Testing ===
    private void ToggleMouseLock() {
        if (Input.GetKeyDown(toggleMouseLockKey)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                LockCursor();
            }
        }
    }

    private void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ProcessInteractions() {

        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit)) {

            var interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null) {
                interactable.Interact();
            }
        }
    }

    [Header("UI-Elements")]
    [SerializeField]
    private RectTransform leftStickBase;
    [SerializeField]
    private RectTransform leftStickHandle;
    [SerializeField]
    private RectTransform rightStickBase;
    [SerializeField]
    private RectTransform rightStickHandle;


    [Header("Movement Settings")]
    [SerializeField]
    private float maxTouchDistance = 200f; // Max distance from touch start for full input

    private Vector2 leftTouchStartPos;
    private Vector2 rightTouchStartPos;

    private int leftTouchId = -1;
    private int rightTouchId = -1;

    private bool leftTouchActive = false;
    private bool rightTouchActive = false;

    void HandleTouchInput() {


        // Reset tracking at start of frame
        if (Input.touchCount == 0) {

            leftTouchActive = false;
            rightTouchActive = false;

            leftTouchId = -1;
            rightTouchId = -1;

            leftStickHandle.gameObject.SetActive(false);
            rightStickHandle.gameObject.SetActive(false);
        }

        // Process all current touches
        for (int i = 0; i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase) {
                case TouchPhase.Began:
                    AssignTouchToStick(touch);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    ProcessActiveTouch(touch);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    ReleaseTouch(touch);
                    break;
            }
        }
    }

    void AssignTouchToStick(Touch touch) {

        // Determine if touch is on left or right side of screen
        bool isLeftSide = touch.position.x < Screen.width / 2;

        if (isLeftSide && !leftTouchActive) {
            leftTouchId = touch.fingerId;
            leftTouchStartPos = touch.position;
            leftTouchActive = true;

        } else if (!isLeftSide && !rightTouchActive) {
            rightTouchId = touch.fingerId;
            rightTouchStartPos = touch.position;
            rightTouchActive = true;
        }
    }


    void ProcessActiveTouch(Touch touch) {

        // Left stick - Movement
        if (leftTouchActive && touch.fingerId == leftTouchId) {
            Vector2 delta = touch.position - leftTouchStartPos;
            Vector2 normalizedDelta = delta / maxTouchDistance;

            // Clamp the input to prevent excessive values
            normalizedDelta = Vector2.ClampMagnitude(normalizedDelta, 1f);

            // Apply movement
            MoveCharacter(normalizedDelta);

            if (Vector2.Distance(touch.position, leftTouchStartPos) > maxTouchDistance) {
                leftStickHandle.gameObject.SetActive(true);
                leftStickHandle.position = /*leftTouchStartPos + (delta * maxTouchDistance)*/ touch.position;
            }
        } else {
            leftStickHandle.gameObject.SetActive(false);
        }

        // Right stick - Camera rotation 
        if (rightTouchActive && touch.fingerId == rightTouchId) {
            Vector2 delta = touch.position - rightTouchStartPos;
            Vector2 normalizedDelta = delta / maxTouchDistance;

            // Clamp the input to prevent excessive values
            normalizedDelta = Vector2.ClampMagnitude(normalizedDelta, 1f);

            // Apply camera rotation
            RotateCamera(normalizedDelta);

            if(Vector2.Distance(touch.position, rightTouchStartPos) > maxTouchDistance){
                rightStickHandle.gameObject.SetActive(true);
                rightStickHandle.position = /*leftTouchStartPos + (delta * maxTouchDistance)*/ touch.position;
            } 
        } else {
            rightStickHandle.gameObject.SetActive(false);
        }
    }

    void RotateCamera(Vector2 input) {

        // Horizontal rotation (Y-axis)
        transform.Rotate(0, input.x * mouseSensitivity, 0);

            // Adjust camera pitch (X-axis rotation)
            float currentPitch = camera.localEulerAngles.x;

            float newPitch = currentPitch - input.y * mouseSensitivity;

            // Clamp vertical look to prevent over-rotation
            //newPitch = Mathf.Clamp(newPitch, -80f, 80f);

            //camera.localEulerAngles = new Vector3(newPitch, 0, 0);


        xRotation -= /*camera.localEulerAngles.x - */ input.y * mouseSensitivity;

        xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void MoveCharacter(Vector2 input) {

        // Get camera forward direction without Y component
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Calculate movement direction relative to camera
        Vector3 moveDirection = (cameraForward * input.y + Camera.main.transform.right * input.x);

        // Apply movement
        transform.Translate(moveDirection * walkSpeed * Time.deltaTime, Space.World);
    }

    void ReleaseTouch(Touch touch) {
        if (touch.fingerId == leftTouchId) {
            leftTouchActive = false;
            leftTouchId = -1;
        } else if (touch.fingerId == rightTouchId) {
            rightTouchActive = false;
            rightTouchId = -1;
        }
    }
}