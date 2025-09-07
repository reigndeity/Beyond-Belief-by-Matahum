using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuestMarker : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Camera mainCamera;
    public TextMeshProUGUI distanceText;
    public Image iconImage;
    private CanvasGroup canvasGroup;

    [Header("Position Settings")]
    public Vector3 offset = new Vector3(0, 2.3f, 0);
    public float edgeBuffer = 50f;

    [Header("Scaling Settings")]
    public float minScale = 0.6f;
    public float maxScale = 1.2f;
    public float minDistance = 5f;
    public float maxDistance = 100f;

    [Header("Fade Settings")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    [Header("Clamping Shape")]
    public ClampShape clampShape = ClampShape.Circle;
    public enum ClampShape { Circle, Square }
    

    private RectTransform rectTransform;
    private Coroutine fadeRoutine;
    private bool isVisible = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // start hidden
            fadeRoutine = StartCoroutine(FadeTo(1f, fadeInDuration));
        }
    }

    void Update()
    {
        if (target == null || mainCamera == null) return;

        // World → Screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);
        if (screenPos.z < 0) screenPos *= -1;

        // Center of screen
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = ((Vector2)screenPos - screenCenter).normalized;
        Vector2 offsetFromCenter = (Vector2)screenPos - screenCenter;

        if (clampShape == ClampShape.Circle)
        {
            // Circle radius
            float radius = Mathf.Min(Screen.width, Screen.height) / 2f - edgeBuffer;
            if (offsetFromCenter.magnitude > radius)
                offsetFromCenter = dir * radius;
        }
        else // Square clamp
        {
            float halfW = Screen.width / 2f - edgeBuffer;
            float halfH = Screen.height / 2f - edgeBuffer;
            offsetFromCenter.x = Mathf.Clamp(offsetFromCenter.x, -halfW, halfW);
            offsetFromCenter.y = Mathf.Clamp(offsetFromCenter.y, -halfH, halfH);
        }

        // Final position
        rectTransform.position = screenCenter + offsetFromCenter;

        // Distance-based scaling
        float dist = Vector3.Distance(mainCamera.transform.position, target.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist);
        float scale = Mathf.Lerp(maxScale, minScale, t);
        rectTransform.localScale = Vector3.one * scale;

        // Distance text
        distanceText.text = $"{dist:F0}m";

        // Fade near/far
        if (dist <= 5f && isVisible)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeTo(0f, fadeOutDuration));
            isVisible = false;
        }
        else if (dist > 5f && !isVisible)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeTo(1f, fadeInDuration));
            isVisible = true;
        }
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    IEnumerator FadeOutAndDestroy()
    {
        yield return FadeTo(0f, fadeOutDuration);
        Destroy(gameObject);
    }

    public void FadeOutAndDestroyMarker()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutAndDestroy());
    }

    public void SetQuestType(bool isMainQuest, Sprite mainSprite, Sprite sideSprite)
    {
        if (iconImage != null)
            iconImage.sprite = isMainQuest ? mainSprite : sideSprite;
    }

    public void SetOffset(Vector3 customOffset)
    {
        offset = customOffset;
    }
}
