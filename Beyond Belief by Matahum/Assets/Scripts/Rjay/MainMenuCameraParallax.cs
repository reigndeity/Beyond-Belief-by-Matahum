using UnityEngine;

public class MainMenuCameraParallax : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0.01f, 1f)] private float moveAmount = 0.1f;
    [SerializeField, Range(0.1f, 10f)] private float smoothSpeed = 5f;
    [SerializeField] private bool useRotation = false;
    [SerializeField, Range(1f, 10f)] private float rotationAmount = 2f;

    [Header("Idle Settings")]
    [SerializeField, Range(0.5f, 5f)] private float idleResetDelay = 2f; // seconds before it starts returning
    [SerializeField, Range(0.1f, 5f)] private float idleResetSpeed = 2f; // how quickly it recenters

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 lastMousePos;
    private float idleTimer;
    private bool isIdle;


    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        lastMousePos = Input.mousePosition;
    }

    private void Update()
    {
        Vector3 currentMousePos = Input.mousePosition;

        // Check for mouse movement
        if ((currentMousePos - lastMousePos).sqrMagnitude > 0.5f)
        {
            idleTimer = 0f;
            isIdle = false;
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleResetDelay)
                isIdle = true;
        }

        lastMousePos = currentMousePos;

        if (!isIdle)
        {
            // Normal parallax
            float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
            float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

            if (!useRotation)
            {
                Vector3 targetPosition = startPosition + new Vector3(mouseX * moveAmount, mouseY * moveAmount, 0);
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }
            else
            {
                Quaternion targetRotation = startRotation * Quaternion.Euler(-mouseY * rotationAmount, mouseX * rotationAmount, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
            }
        }
        else
        {
            // Return to center when idle
            if (!useRotation)
                transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * idleResetSpeed);
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * idleResetSpeed);
        }
    }

}
