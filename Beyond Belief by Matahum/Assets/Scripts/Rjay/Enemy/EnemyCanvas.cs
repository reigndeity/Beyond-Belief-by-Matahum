using System.Collections;
using UnityEngine;

public class EnemyCanvas : MonoBehaviour
{
    [Header("Billboard Settings")]
    [SerializeField] private Transform targetCamera;
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
        FaceCamera();
    }

    private void FaceCamera()
    {
        if (targetCamera == null) return;

        Vector3 direction = transform.position - targetCamera.position;
        transform.rotation = Quaternion.LookRotation(direction);
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
