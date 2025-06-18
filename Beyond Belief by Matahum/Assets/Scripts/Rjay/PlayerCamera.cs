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

        // Smooth rotation
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);

        // Calculate desired camera position
        Vector3 desiredCameraPos = targetPosition - (rotation * Vector3.forward * desiredDistance);

        // Sphere cast for collision
        if (Physics.SphereCast(targetPosition, cameraRadius, desiredCameraPos - targetPosition, out RaycastHit hit, desiredDistance, collisionLayers))
        {
            float adjustedDistance = hit.distance - 0.1f;
            currentDistance = Mathf.SmoothDamp(currentDistance, Mathf.Clamp(adjustedDistance, minDistance, maxDistance), ref distanceSmoothVelocity, 1f / collisionSmoothSpeed);
        }
        else
        {
            currentDistance = Mathf.SmoothDamp(currentDistance, desiredDistance, ref distanceSmoothVelocity, 1f / collisionSmoothSpeed);
        }

        // Final camera position
        Vector3 finalCameraPos = targetPosition - (rotation * Vector3.forward * currentDistance);
        transform.position = Vector3.SmoothDamp(transform.position, finalCameraPos, ref currentVelocity, 1f / followSmoothSpeed);
        transform.rotation = rotation;
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
        if (Input.GetKey(KeyCode.LeftAlt))
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
