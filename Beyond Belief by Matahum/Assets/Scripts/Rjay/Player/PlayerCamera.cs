using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 100f;
    public float rotationSmoothTime = 0.05f;
    public float pitchMin = -30f;
    public float pitchMax = 80f;

    [Header("Zoom Settings")]
    public float minDistance = 2f;
    public float maxDistance = 8f;
    public float zoomSpeed = 5f;

    [Header("Camera Collision")]
    public LayerMask collisionLayers;
    public float collisionSmoothSpeed = 10f;
    public float cameraRadius = 0.3f;

    [Header("Follow Settings")]
    public float followSmoothSpeed = 10f;

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

    void Start()
    {
        desiredDistance = currentDistance = maxDistance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        HandleMouseLock();

        if (Cursor.lockState == CursorLockMode.Locked)
            HandleRotation();

        HandleZoom();
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;
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

        if (Physics.SphereCast(targetPosition, cameraRadius, desiredCameraPos - targetPosition, out RaycastHit hit, desiredDistance, collisionLayers))
        {
            float adjustedDistance = Mathf.Max(hit.distance - 0.1f, minDistance);
            currentDistance = Mathf.SmoothDamp(currentDistance, adjustedDistance, ref distanceSmoothVelocity, 1f / collisionSmoothSpeed);
        }
        else
        {
            currentDistance = Mathf.SmoothDamp(currentDistance, desiredDistance, ref distanceSmoothVelocity, 1f / collisionSmoothSpeed);
        }
    }

    void UpdateCameraPosition(Vector3 targetPosition)
    {
        Quaternion rotation = Quaternion.Euler(currentRotation);
        Vector3 finalCameraPos = targetPosition - (rotation * Vector3.forward * currentDistance);
        transform.position = Vector3.SmoothDamp(transform.position, finalCameraPos, ref currentVelocity, 1f / followSmoothSpeed);
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            desiredDistance -= scroll * zoomSpeed;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        }
    }

    void HandleMouseLock()
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
}
