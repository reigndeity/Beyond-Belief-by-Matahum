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

    private RectTransform rectTransform;

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
            StartCoroutine(FadeIn());
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

        // Circle radius (smaller of width/height / 2, minus buffer)
        float radius = Mathf.Min(Screen.width, Screen.height) / 2f - edgeBuffer;

        // Distance from center
        Vector2 offsetFromCenter = (Vector2)screenPos - screenCenter;

        // If outside circle, clamp to edge of circle
        if (offsetFromCenter.magnitude > radius)
        {
            offsetFromCenter = dir * radius;
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
    }

    IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeOutDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    // 🔑 Public function to call from DialogueQuestLinker or QuestManager
    public void FadeOutAndDestroyMarker()
    {
        StartCoroutine(FadeOutAndDestroy());
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
