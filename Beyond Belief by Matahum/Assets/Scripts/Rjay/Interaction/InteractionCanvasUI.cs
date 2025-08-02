using UnityEngine;

public class InteractionCanvasUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 3f;

    void Update()
    {
        float targetAlpha = InteractionManager.Instance.HasInteractables() ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        canvasGroup.interactable = canvasGroup.blocksRaycasts = targetAlpha > 0.9f;
    }
}
