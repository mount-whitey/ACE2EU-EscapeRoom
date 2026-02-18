using System;
using System.Collections;
using System.Drawing.Text;
using UnityEngine;

using UnityEngine.UI;

namespace ACE2EU {

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController: MonoBehaviour {

        public static PlayerController Instance { get; private set; } = null;

        public bool CanMove { get; set; } = true;

        [Header("References")]
        private Camera _camera;
        [SerializeField] private Transform crouchedCameraTarget;
        [SerializeField] private Image fade;
        [SerializeField] private GameObject _pc;

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 7f;
        [SerializeField] private float runSpeed = 12f;
        [SerializeField] private float crouchSpeed = 3f;
        [SerializeField] private float jumpSpeed = 8f;
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float interactDistance = float.PositiveInfinity;

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

        private CharacterController _physic;
        private float xRotation = 0f;
        private Vector3 moveDirection = Vector3.zero;

        private float originalHeight;
        private Vector3 originalCameraPos;

        private bool _fade;

        private TextBubble _textBubble;
        private Action _afterShowed;

        [SerializeField]
        private GameObject _home;

        private void Awake() {
            Instance = this;
        }

        void Start() {

            _textBubble = GetComponentInChildren<TextBubble>(true);

            _physic = GetComponentInChildren<CharacterController>(true);
            _physic.center = Vector3.zero;

            _camera = GetComponentInChildren<Camera>(true);
            originalHeight = _physic.height;
            originalCameraPos = _camera.transform.localPosition;

            LockCursor();

            leftStickHandle.gameObject.SetActive(false);
            rightStickHandle.gameObject.SetActive(false);
        }

        void Update() {

            if (Input.touchCount > 0) {
                // Mobile
                HandleTouchInput();

                _pc.SetActive(false);

            } else {
                // PC

                ToggleMouseLock();

                if (Cursor.lockState == CursorLockMode.Locked) {
                    ProcessLook();
                }

                if (_home.activeInHierarchy) {

                    if (_home.transform.GetChild(0).gameObject.activeInHierarchy) {

                        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Z)) {
                            FindAnyObjectByType<Portal>(FindObjectsInactive.Include).Teleport();
                        }

                        if (Input.GetKeyDown(KeyCode.N)) {
                            _home.transform.GetChild(0).gameObject.SetActive(false);
                        }

                    } else {
                        if (Input.GetKeyDown(KeyCode.H)){
                            _home.transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }


                if (CanMove) {
                    ProcessMovement();
                }

                ProcessCrouch();

                if (Input.GetMouseButtonDown(0)) {
                    ProcessInteraction(new Ray(_camera.transform.position, _camera.transform.forward));
                }
            }

            if (_justForward) {

                moveDirection = transform.TransformDirection(new(0, 0, 1)) * walkSpeed * 2f;
                moveDirection.y -= gravity * Time.deltaTime;

                _physic.Move(moveDirection * Time.deltaTime);
            }
        }

        void ProcessLook() {

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);
            _camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        void ProcessMovement() {

            if (_physic.isGrounded) {
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
            _physic.Move(moveDirection * Time.deltaTime);
        }

        void ProcessCrouch() {
            float targetHeight = Input.GetKey(keyCrouch) ? crouchedHeight : originalHeight;
            Vector3 targetCamPos = Input.GetKey(keyCrouch) ? crouchedCameraTarget.localPosition : originalCameraPos;

            _physic.height = Mathf.Lerp(_physic.height, targetHeight, Time.deltaTime * 10f);
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, targetCamPos, Time.deltaTime * 10f);

            _physic.center = new Vector3(0f, -0.28f, 0f);
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

        private void ProcessInteraction(Ray ray) {

            if (_bubbleActive) {
                CanMove = true;
                _textBubble.gameObject.SetActive(_bubbleActive = false);
                _afterShowed?.Invoke();
                return;
            }

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)) {

                var interactables = hit.collider.GetComponentsInParent<Interactable>();

                if(interactables == null) {
                    return;
                }

                GameObject go = null;

                foreach(var interactable in interactables){
                    if(go == null) {
                        go = interactable.gameObject;
                    } else if(go != interactable.gameObject) {
                        continue;
                    }

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
                        CheckTouchInteraction(touch);
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

        private void CheckTouchInteraction(Touch touch) {
            ProcessInteraction(_camera.ScreenPointToRay(touch.position));
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
                if (CanMove) {
                    MoveCharacter(normalizedDelta);
                }

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

                if (Vector2.Distance(touch.position, rightTouchStartPos) > maxTouchDistance) {
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
            float currentPitch = _camera.transform.localEulerAngles.x;

            float newPitch = currentPitch - input.y * mouseSensitivity;

            // Clamp vertical look to prevent over-rotation
            //newPitch = Mathf.Clamp(newPitch, -80f, 80f);

            //camera.localEulerAngles = new Vector3(newPitch, 0, 0);


            xRotation -= /*camera.localEulerAngles.x - */ input.y * mouseSensitivity;

            xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);
            _camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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

        private void OnTriggerEnter(Collider other) {

            if (_fade) {
                return;
            }

            var interactable = other.GetComponentInParent<Interactable>();

            if (interactable == null) {
                return;
            }

            StopAllCoroutines();

            if (interactable is Portal) {

                float delay = (interactable as Portal).Delay;

                if (delay > 0) {
                    interactable.Interact();
                }

                StartCoroutine(FadeRoutine(delay: delay, afterFade: interactable.Interact));
            } else {
                interactable.Interact();
            }
        }

        public void FadeIn() {
            StartCoroutine(FadeRoutine(false));
        }

        public void FadeOut(Action afterFade = null) {
            StartCoroutine(FadeRoutine(true, afterFade: afterFade ));
        }

        private IEnumerator FadeRoutine(bool fadeOut = true, float duration = 1f, float delay = 0f, Action afterFade = null) {

            // Delay
            if (delay > 0) {
                yield return new WaitForSeconds(delay);
            }

            // Transition
            float refTime = Time.realtimeSinceStartup;
            float curTime = 0;

            if (fadeOut) {
                Debug.Log("Fade-OUT");

                while (curTime < duration) {

                    yield return null;

                    fade.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, (curTime = Time.realtimeSinceStartup - refTime) / duration));
                }
            } else {
                Debug.Log("Fade-IN");

                while (curTime < duration) {

                    yield return null;

                    fade.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, (curTime = Time.realtimeSinceStartup - refTime) / duration));
                }
            }

            afterFade?.Invoke();
        }

        public void JustForward(bool state) {
            _justForward = state;
            CanMove = !state;
        }
        private bool _justForward = false;

        public void ShowInformation(string text, Action afterShowed = null) {

            _textBubble.Style = Style.Message;

            _textBubble.PrePart = "";
            _textBubble.PostPart = "";

            _textBubble.MainLeft = "";
            _textBubble.MainMid = "";
            _textBubble.MainRight = "";

            _textBubble.MainMid = text;

            _textBubble.gameObject.SetActive(_bubbleActive = true);

            _afterShowed = afterShowed;
            CanMove = false;
        }

        public void ShowHeader(string mainLeft = "", string mainMid = "", string mainRight = "", string prePart = "", string postPart = "") {

            _textBubble.Style = Style.Header;

            _textBubble.PrePart = prePart;
            _textBubble.PostPart = postPart;

            _textBubble.MainLeft = mainLeft;
            _textBubble.MainMid = mainMid;
            _textBubble.MainRight = mainRight;

            _textBubble.gameObject.SetActive(_bubbleActive = true);

            CanMove = false;
        }
        private bool _bubbleActive = false;

        public void ShowHome(bool visible) {

            _home.SetActive(visible);
            _home.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}