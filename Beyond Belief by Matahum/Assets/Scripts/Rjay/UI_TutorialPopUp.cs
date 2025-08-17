using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UI_TutorialPopUp : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public float duration;

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
    }

    public void FadeInAndFadeOut(float duration)
    {
        StartCoroutine(FadeInAndFadeOutRoutine(duration));
    }

    private IEnumerator FadeInAndFadeOutRoutine(float duration)
    {
        FadeIn(0.5f);
        yield return new WaitForSeconds(duration);
        FadeOut(0.5f);
    }
}
