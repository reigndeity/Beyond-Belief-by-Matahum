using UnityEngine;

public class InteractionCanvasUI : MonoBehaviour
{
    public static InteractionCanvasUI Instance { get; private set; }

    [Header("Fade Settings")]
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 3f;

    private bool isForcedHidden = false;
    private bool isForcedShown = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        float targetAlpha;

        if (isForcedHidden)
        {
            targetAlpha = 0f;
        }
        else if (isForcedShown)
        {
            targetAlpha = 1f;
        }
        else
        {
            targetAlpha = InteractionManager.Instance.HasInteractables() ? 1f : 0f;
        }

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        canvasGroup.interactable = canvasGroup.blocksRaycasts = targetAlpha > 0.9f;
    }

    // ==========================
    // 📌 Public Control Functions
    // ==========================

    public void ShowInteractionPromptUI()
    {
        isForcedHidden = false;
        isForcedShown = true;
    }

    public void HideInteractionPromptUI()
    {
        isForcedShown = false;
        isForcedHidden = true;
    }

    public void ResetToAutomatic()
    {
        isForcedHidden = false;
        isForcedShown = false;
    }
}
