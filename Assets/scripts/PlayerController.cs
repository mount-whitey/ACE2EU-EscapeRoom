using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTx;
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

    // UI Movement Input
    private bool moveForward = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.center = Vector3.zero;

        originalHeight = controller.height;
        originalCameraPos = cameraTx.localPosition;

        LockCursor();
    }

    void Update()
    {
        ToggleMouseLock();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            ProcessLook();
        }

        ProcessMovement();
        ProcessCrouch();
    }

    void ProcessLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);
        cameraTx.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void ProcessMovement()
    {
        if (controller.isGrounded)
        {
            float verticalInput = moveForward ? 1f : Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            Vector3 input = new(horizontalInput, 0f, verticalInput);
            Vector3 move = transform.TransformDirection(input.normalized);

            float speed = walkSpeed;
            if (Input.GetKey(keyCrouch)) speed = crouchSpeed;
            else if (Input.GetKey(keyRun)) speed = runSpeed;

            moveDirection = move * speed;

            if (Input.GetKeyDown(keyJump) && !Input.GetKey(keyCrouch))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ProcessCrouch()
    {
        float targetHeight = Input.GetKey(keyCrouch) ? crouchedHeight : originalHeight;
        Vector3 targetCamPos = Input.GetKey(keyCrouch) ? crouchedCameraTarget.localPosition : originalCameraPos;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
        cameraTx.localPosition = Vector3.Lerp(cameraTx.localPosition, targetCamPos, Time.deltaTime * 10f);

        controller.center = new Vector3(0f, -0.28f, 0f);
    }

    // === UI Button Controlled Movement ===
    public void SetMoveForward(bool state)
    {
        moveForward = state;
    }

    // === Toggle Mouse Lock for Testing ===
    private void ToggleMouseLock()
    {
        if (Input.GetKeyDown(toggleMouseLockKey))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                LockCursor();
            }
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
