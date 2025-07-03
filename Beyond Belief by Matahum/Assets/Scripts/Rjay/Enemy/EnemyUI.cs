using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{

    private EnemyStats m_enemyStats;
    private EnemyCanvas m_enemyCanvas;

    [Header("Health UI Properties")]
    [SerializeField] private GameObject healthBarRoot;
    private CanvasGroup healthBarCanvasGroup;
    private Coroutine healthBarFadeRoutine;
    [SerializeField] private Image healthFillDelayed;
    [SerializeField] private Image healthFillInstant;
    [SerializeField] private float healthFillDelaySpeed = 2f;

    [Header("Alert/Surprised UI Animation Settings")]
    [SerializeField] GameObject alertObj;
    private CanvasGroup alertCanvasGroup;
    private Coroutine alertFadeRoutine;
    [SerializeField] private float alertRiseAmount = 5f;
    [SerializeField] private float alertOvershoot = 3f; // how much higher it goes before settling
    [SerializeField] private float alertDuration = 0.3f;
    [SerializeField] private AnimationCurve alertEaseCurve; // optional
    private Coroutine alertRoutine;

    void Start()
    {
        m_enemyStats = GetComponent<EnemyStats>();
        m_enemyCanvas = GetComponentInChildren<EnemyCanvas>();

        alertCanvasGroup = alertObj.GetComponent<CanvasGroup>();
        healthBarCanvasGroup = healthBarRoot.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        UIHealthUpdate();
    }
    #region HEALTH PROPERTIES
    void UIHealthUpdate()
    {
        float current = m_enemyStats.e_currentHealth;
        float max = m_enemyStats.e_maxHealth;

        float targetFill = current / max;

        healthFillInstant.fillAmount = targetFill;

        if (healthFillDelayed.fillAmount > targetFill)
        {
            healthFillDelayed.fillAmount = Mathf.Lerp(healthFillDelayed.fillAmount, targetFill, Time.deltaTime * healthFillDelaySpeed);
        }
        else
        {
            healthFillDelayed.fillAmount = targetFill;
        }
    }
    public void HealthBarFadeIn(float duration)
    {
        if (healthBarFadeRoutine != null) StopCoroutine(healthBarFadeRoutine);
        healthBarRoot.SetActive(true);
        healthBarFadeRoutine = StartCoroutine(FadeHealthBar(0f, 1f, duration));
    }

    public void HealthBarFadeOut(float duration)
    {
        if (healthBarFadeRoutine != null) StopCoroutine(healthBarFadeRoutine);
        healthBarFadeRoutine = StartCoroutine(FadeHealthBar(1f, 0f, duration));
    }
    private IEnumerator FadeHealthBar(float from, float to, float duration)
    {
        float currentAlpha = healthBarCanvasGroup.alpha;

        // Skip fade if already at or near the target
        if (Mathf.Approximately(currentAlpha, to))
        {
            healthBarCanvasGroup.alpha = to;
            healthBarRoot.SetActive(to > 0f);
            yield break;
        }

        float time = 0f;
        healthBarCanvasGroup.alpha = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            healthBarCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        healthBarCanvasGroup.alpha = to;
        healthBarRoot.SetActive(to > 0f);

        healthBarFadeRoutine = null;
    }

    #endregion
    #region ALERT/SURPRISED PROPERTIES
    public void ShowAlert()
    {
        if (alertRoutine != null) StopCoroutine(alertRoutine);
        alertObj.SetActive(true);
        alertRoutine = StartCoroutine(AnimateAlert());
    }
    private IEnumerator AnimateAlert()
    {
        Vector3 startPos = alertObj.transform.localPosition;
        Vector3 overshootPos = startPos + Vector3.up * (alertRiseAmount + alertOvershoot);
        Vector3 finalPos = startPos + Vector3.up * alertRiseAmount;

        float t = 0f;

        // Phase 1: Rise to overshoot
        while (t < alertDuration)
        {
            float percent = t / alertDuration;
            float eased = alertEaseCurve != null ? alertEaseCurve.Evaluate(percent) : percent;

            alertObj.transform.localPosition = Vector3.Lerp(startPos, overshootPos, eased);
            t += Time.deltaTime;
            yield return null;
        }

        // Phase 2: Ease down to final
        t = 0f;
        while (t < alertDuration)
        {
            float percent = t / alertDuration;
            float eased = alertEaseCurve != null ? alertEaseCurve.Evaluate(percent) : percent;

            alertObj.transform.localPosition = Vector3.Lerp(overshootPos, finalPos, eased);
            t += Time.deltaTime;
            yield return null;
        }

        alertObj.transform.localPosition = finalPos;
    }
    public void AlertFadeIn(float duration)
    {
        if (alertFadeRoutine != null) StopCoroutine(alertFadeRoutine);
        alertObj.SetActive(true); // Ensure it's visible
        alertFadeRoutine = StartCoroutine(FadeAlert(0f, 1f, duration));
    }

    public void AlertFadeOut(float duration)
    {
        if (alertFadeRoutine != null) StopCoroutine(alertFadeRoutine);
        alertFadeRoutine = StartCoroutine(FadeAlert(1f, 0f, duration));
    }
    private IEnumerator FadeAlert(float from, float to, float duration)
    {
        float currentAlpha = alertCanvasGroup.alpha;

        // Skip fade if already at or near the target
        if (Mathf.Approximately(currentAlpha, to))
        {
            alertCanvasGroup.alpha = to;
            alertObj.SetActive(to > 0f);
            yield break;
        }

        float time = 0f;
        alertCanvasGroup.alpha = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            alertCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        alertCanvasGroup.alpha = to;
        alertObj.SetActive(to > 0f);

        alertFadeRoutine = null;
    }
    #endregion
}