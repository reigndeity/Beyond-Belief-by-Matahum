using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }
    [Header("References")]
    public Transform playerTarget;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 50;
    public float rotationSmoothTime = 0.02f;
    public float pitchMin = -30f;
    public float pitchMax = 60;

    [Header("Zoom Settings")]
    public float minDistance = 2f;
    public float maxDistance = 4.5f;
    public float zoomSpeed = 2f;

    [Header("Camera Collision")]
    public LayerMask collisionLayers;
    public float collisionSmoothSpeed = 10f;
    public float cameraRadius = 0.3f;

    [Header("Follow Settings")]
    public float followSmoothSpeed = 100f;

    private float yaw;
    private float pitch;

    private float currentDistance;
    private float desiredDistance;
    private float distanceSmoothVelocity;

    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    private Vector3 currentVelocity;

    // Camera shake
    private float shakeTimer = 0f;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    // Cursor toggle
    private bool isCursorVisible = false;

    // Toggles
    [SerializeField] private bool rotationEnabled = true;
    [SerializeField] private bool zoomEnabled = true;

    // Rotation API
    public void EnableRotation()  => rotationEnabled = true;
    public void DisableRotation() => rotationEnabled = false;
    public bool IsRotationEnabled => rotationEnabled;

    // Zoom API
    public void EnableZoom()  => zoomEnabled = true;
    public void DisableZoom() => zoomEnabled = false;
    public bool IsZoomEnabled => zoomEnabled;

    private bool isCameraLocked = false;

    private UI_Game uiGame;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        uiGame = FindFirstObjectByType<UI_Game>();

        desiredDistance = currentDistance = maxDistance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        if (playerTarget == null) return;
        if (isCameraLocked) return;
        if (uiGame.IsGamePaused()) return;
        Vector3 targetPosition = playerTarget.position;

        UpdateShake();
        UpdateRotation();
        UpdateDistanceWithCollision(targetPosition);
        UpdateCameraPosition(targetPosition);
    }

    void UpdateRotation()
    {
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(currentRotation + shakeOffset); // apply shake here
    }

    void UpdateDistanceWithCollision(Vector3 targetPosition)
    {
        Quaternion rotation = Quaternion.Euler(currentRotation);
        Vector3 desiredCameraPos = targetPosition - (rotation * Vector3.forward * desiredDistance);

        float targetDistance = desiredDistance;

        // Check along the path (spherecast)
        if (Physics.SphereCast(targetPosition, cameraRadius, desiredCameraPos - targetPosition, out RaycastHit hit, desiredDistance, collisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance - 0.1f, minDistance, desiredDistance);
        }

        // Smooth transition
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceSmoothVelocity, 1f / collisionSmoothSpeed);

        // --- Extra safeguard: check final camera position ---
        Vector3 finalCameraPos = targetPosition - (rotation * Vector3.forward * currentDistance);

        if (Physics.CheckSphere(finalCameraPos, cameraRadius, collisionLayers))
        {
            // Push camera out of walls
            if (Physics.Raycast(targetPosition, (finalCameraPos - targetPosition).normalized, out RaycastHit pushHit, currentDistance, collisionLayers))
            {
                currentDistance = Mathf.Clamp(pushHit.distance - 0.05f, minDistance, currentDistance);
            }
        }
    }



    void UpdateCameraPosition(Vector3 targetPosition)
    {
        Quaternion rotation = Quaternion.Euler(currentRotation);
        Vector3 finalCameraPos = targetPosition - (rotation * Vector3.forward * currentDistance);
        transform.position = Vector3.SmoothDamp(transform.position, finalCameraPos, ref currentVelocity, 1f / followSmoothSpeed);
    }

    public void HandleRotation()
    {
        if (!TutorialManager.instance.tutorial_canCameraDirection) return;
        if (!rotationEnabled) return;   // ← guard here

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw   += mouseX * mouseSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    public void HandleZoom()
    {
        if (!TutorialManager.instance.tutorial_canCameraZoom) return;
        if (!zoomEnabled) return;                       // ← guard here
        if (InteractionManager.IsUsingScrollInput) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            desiredDistance -= scroll * zoomSpeed;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        }
    }


    public void HandleMouseLock()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isCursorVisible = !isCursorVisible;

            if (isCursorVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void CameraShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimer = duration;
    }

    void UpdateShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float shakeStrength = shakeMagnitude * (shakeTimer / shakeDuration); // linear fade
            shakeOffset = Random.insideUnitSphere * shakeStrength;
            shakeOffset.z = 0f; // don't shake forward/backward
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void LockCamera()
    {
        DisableRotation();
        DisableZoom();
    }
    public void UnlockCamera()
    {
        EnableRotation();
        EnableZoom();
    }

    public void AdjustCamera()
    {
        CameraAdjust(3.5f, 25f, instant: false);
    }
    public void CameraAdjust(float targetDistance, float targetPitch, bool instant = false)
    {
        if (playerTarget == null) return;
        desiredDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        yaw   = playerTarget.eulerAngles.y;
        pitch = Mathf.Clamp(targetPitch, pitchMin, pitchMax);

        if (instant)
        {
            currentRotation = new Vector3(pitch, yaw, 0f);
            rotationSmoothVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(currentRotation);

            currentDistance = desiredDistance;
            distanceSmoothVelocity = 0f;
        }
    }

    public void SetCursorVisibility(bool visible)
    {
        isCursorVisible = visible;

        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    
    public void HardLockCamera()
    {
        isCameraLocked = true;
    }

    public void HardUnlockCamera()
    {
        isCameraLocked = false;
    }

}
