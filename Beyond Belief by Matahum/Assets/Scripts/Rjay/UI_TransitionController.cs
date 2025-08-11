using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TransitionController : MonoBehaviour
{
    public static UI_TransitionController instance;
    private Player m_player;
    [Header("Fade")]
    [SerializeField] CanvasGroup fadeGroup;      // Fullscreen black CG (alpha 0 at rest)
    [SerializeField, Min(0f)] float fadeDuration = 0.25f;

    [Header("Loading UI")]
    [SerializeField] GameObject loadingPanel;    // Parent panel for loading UI (inactive at rest)
    [SerializeField] Image loadingFill;          // Image (Filled, Horizontal)
    [SerializeField] TextMeshProUGUI percentText; // optional
    [SerializeField, Min(0f)] float loadingDuration = 0.75f; // how long to “load” (same-scene)

    [HideInInspector] public bool isTeleporting { get; private set; }

    void Awake()
    {
        if (fadeGroup)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
            fadeGroup.interactable = false;
        }
        if (loadingPanel) loadingPanel.SetActive(false);
        if (loadingFill) loadingFill.fillAmount = 0f;
        if (percentText) percentText.text = "0%";

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        m_player = FindFirstObjectByType<Player>();
    }

    public IEnumerator TeleportTransition(Action teleportAction)
    {
        isTeleporting = true;
        yield return Fade(0f, 1f, fadeDuration);
        MapManager.instance.HideSelection();
        MapManager.instance.teleporterPanel.SetActive(false);
        PlayerMinimap.instance?.CloseMap();
        yield return null;
        teleportAction?.Invoke();
        PlayerCamera.Instance.AdjustCamera();
        yield return new WaitForSecondsRealtime(0.1f);
        isTeleporting = false;
        m_player.ForceIdleOverride();
        yield return new WaitForSecondsRealtime(0.25f);
        isTeleporting = true;
        yield return PlayLoading();
        yield return Fade(1f, 0f, fadeDuration);
        isTeleporting = false;
    }
    /// <summary>
    /// Same as TeleportTransition, but more custom values
    /// </summary>
    public IEnumerator TeleportTransition(Action teleportAction, float customFadeDuration, float customLoadingDuration)
    {
        float oldFade = fadeDuration;
        float oldLoad = loadingDuration;
        fadeDuration = Mathf.Max(0f, customFadeDuration);
        loadingDuration = Mathf.Max(0f, customLoadingDuration);

        yield return TeleportTransition(teleportAction);

        fadeDuration = oldFade;
        loadingDuration = oldLoad;
    }


    public IEnumerator Fade(float from, float to, float duration)
    {
        if (!fadeGroup)
            yield break;

        fadeGroup.blocksRaycasts = true;
        fadeGroup.interactable = false;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, duration <= 0f ? 1f : t / duration);
            yield return null;
        }
        fadeGroup.alpha = to;

        if (to == 0f)
        {
            fadeGroup.blocksRaycasts = false;
        }
    }
    
    IEnumerator PlayLoading()
    {
        if (!loadingPanel || !loadingFill)
        {
            // If there’s no loading UI wired, just wait a short moment for effect
            yield return new WaitForSecondsRealtime(Mathf.Max(0.05f, loadingDuration));
            yield break;
        }

        loadingPanel.SetActive(true);
        loadingFill.fillAmount = 0f;
        if (percentText) percentText.text = "0%";

        float t = 0f;
        float dur = Mathf.Max(0f, loadingDuration);

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float p = dur <= 0f ? 1f : Mathf.Clamp01(t / dur);
            loadingFill.fillAmount = p;
            if (percentText) percentText.text = Mathf.RoundToInt(p * 100f) + "%";
            yield return null;
        }

        // ensure 100%
        loadingFill.fillAmount = 1f;
        if (percentText) percentText.text = "100%";

        // tiny settle so 100% is visible
        yield return new WaitForSecondsRealtime(0.05f);

        loadingPanel.SetActive(false);
        loadingFill.fillAmount = 0f;
        if (percentText) percentText.text = "0%";
    }
}
