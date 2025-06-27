using UnityEngine;
using System.Collections;

public class PlayerStaminaCanvas : MonoBehaviour
{
    [Header("Billboard Settings")]
    [SerializeField] private Transform targetCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(0.5f, 1.5f, 0f); // tuned to taste
    [SerializeField] private float positionSmoothSpeed = 10f;
    private CanvasGroup m_canvasGroup;
    private Coroutine currentFadeRoutine;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (targetCamera == null && Camera.main != null)
            targetCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (playerTransform != null && targetCamera != null)
        {
            PositionToRightOfPlayer();
            FaceCamera();
        }
    }

    private void PositionToRightOfPlayer()
    {
        // Get the direction to the right of the camera
        Vector3 cameraRight = targetCamera.right;

        // Desired position is player's position + camera's right direction * x offset + y offset
        Vector3 targetPos = playerTransform.position +
                            cameraRight * offsetFromPlayer.x + 
                            Vector3.up * offsetFromPlayer.y +
                            targetCamera.forward * offsetFromPlayer.z;

        transform.position = targetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionSmoothSpeed);
    }

    private void FaceCamera()
    {
        // Billboard towards the camera
        transform.forward = targetCamera.forward;
    }

    public void FadeIn(float duration) // alpha: 0 to 1
    {
        StartFade(1f, duration);
    }

    public void FadeOut(float duration) // alpha: 1 to 0
    {
        StartFade(0f, duration);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(FadeCanvasGroup(targetAlpha, duration));
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = m_canvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            m_canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            yield return null;
        }

        m_canvasGroup.alpha = targetAlpha;
        currentFadeRoutine = null;
    }
}
