using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UI_TutorialPopUp : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("Settings")]
    public float duration = 2f;
    public bool isRequired = false;              // Only fade out after a key press if true
    public KeyCode requiredKey = KeyCode.None;   // Key to continue

    [Header("Events")]
    public UnityEvent onFadeInComplete;
    public UnityEvent onFadeOutComplete;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start hidden
    }

    void Start()
    {
        FadeInAndFadeOut(duration);
    }

    /// <summary>
    /// Fade from 0 to 1.
    /// </summary>
    public void FadeIn(float duration)
    {
        AudioManager.instance.PlayTutorialPopUpSFX();
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(0f, 1f, duration, onFadeInComplete));
    }

    /// <summary>
    /// Fade from 1 to 0.
    /// </summary>
    public void FadeOut(float duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(1f, 0f, duration, onFadeOutComplete));
    }

    private IEnumerator FadeRoutine(float start, float end, float duration, UnityEvent onComplete)
    {
        float time = 0f;
        canvasGroup.alpha = start;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }

        canvasGroup.alpha = end;
        onComplete?.Invoke();
        fadeRoutine = null;

        // If fade-in completed and requiredKey is active, wait for player input
        if (end == 1f && isRequired && requiredKey != KeyCode.None)
        {
            yield return StartCoroutine(WaitForRequiredKey());
        }
    }

    private IEnumerator WaitForRequiredKey()
    {
        Debug.Log($"[TutorialPopUp] Waiting for key: {requiredKey}");
        while (!Input.GetKeyDown(requiredKey))
        {
            yield return null;
        }

        Debug.Log($"[TutorialPopUp] Required key pressed: {requiredKey}");
        FadeOut(0.5f);
    }

    public void FadeInAndFadeOut(float duration)
    {
        StartCoroutine(FadeInAndFadeOutRoutine(duration));
    }

    private IEnumerator FadeInAndFadeOutRoutine(float duration)
    {
        FadeIn(0.5f);

        // Only auto-fade-out if not required
        if (!isRequired)
        {
            yield return new WaitForSeconds(duration);
            FadeOut(0.5f);
        }
    }
}
