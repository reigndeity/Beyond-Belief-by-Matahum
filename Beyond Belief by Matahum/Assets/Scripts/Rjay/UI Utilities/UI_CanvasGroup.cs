using System.Collections;
using UnityEngine;

public class UI_CanvasGroup : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        UpdateInteractableState();
    }

    /// <summary>
    /// Fades the CanvasGroup in over the given duration.
    /// </summary>
    public void FadeIn(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, duration));
    }

    /// <summary>
    /// Fades the CanvasGroup out over the given duration.
    /// </summary>
    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, duration <= 0f ? 1f : time / duration);
            UpdateInteractableState();
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        UpdateInteractableState();
    }

    private void UpdateInteractableState()
    {
        bool fullyVisible = Mathf.Approximately(canvasGroup.alpha, 1f);
        bool fullyHidden = Mathf.Approximately(canvasGroup.alpha, 0f);

        canvasGroup.interactable = fullyVisible;
        canvasGroup.blocksRaycasts = fullyVisible;

        if (fullyHidden)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
