using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;

public class UI_MainMenu : MonoBehaviour
{
    [System.Serializable]
    public class SettingTabButton
    {
        public Button button;
        public Image icon;
    }

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button closeGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnButton;

    [Header("New Game Confirmation")]
    [SerializeField] private GameObject newGameConfirmationPanel;
    [SerializeField] private Button noNewGameButton;
    [SerializeField] private Button yesNewGameButton;

    [Header("Close Game Confirmation")]
    [SerializeField] private GameObject closeGameConfirmationPanel;
    [SerializeField] private Button noCloseGameButton;
    [SerializeField] private Button yesCloseGameButton;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject displayAndGraphicsPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private SettingTabButton audioButton;
    [SerializeField] private SettingTabButton displayAndGraphicsButton;
    [SerializeField] private SettingTabButton controlButton;

    private SettingTabButton activeSettingTab;
    private readonly Color activeColor = new(0.4f, 0.2f, 0f, 1f);
    private readonly Color inactiveColor = Color.white;

    private string savePath;

    [Header("Blur")]
    [SerializeField] private Image blurImage;
    [SerializeField] private float blurDuration = 0.5f;
    private float currentAlpha = 0f;
    private Coroutine blurRoutine;

    [Header("Button Hover")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleSpeed = 8f;
    [Header("Fade")]
    public UI_CanvasGroup fadeCanvasGroup;

    void Awake()
    {
        string slotId = "Slot_01";
        savePath = Path.Combine(Application.persistentDataPath, $"Auto_{slotId}.es3");
        continueGameButton.gameObject.SetActive(File.Exists(savePath));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        fadeCanvasGroup.FadeOut(1);
        newGameButton.onClick.AddListener(OnClickNewGame);
        noNewGameButton.onClick.AddListener(OnClickNoNewGame);
        yesNewGameButton.onClick.AddListener(OnClickYesNewGame);
        continueGameButton.onClick.AddListener(OnClickContinueGame);
        closeGameButton.onClick.AddListener(OnClickCloseGame);
        yesCloseGameButton.onClick.AddListener(OnClickYesCloseGame);
        noCloseGameButton.onClick.AddListener(OnClickNoCloseGame);
        settingsButton.onClick.AddListener(OnOpenSettings);
        returnButton.onClick.AddListener(OnCloseSettings);

        audioButton.button.onClick.AddListener(OnClickAudioTab);
        displayAndGraphicsButton.button.onClick.AddListener(OnClickDisplayTab);
        controlButton.button.onClick.AddListener(OnClickControlTab);

        // Hover listeners
        AddHoverEffect(newGameButton);
        AddHoverEffect(continueGameButton);
        AddHoverEffect(closeGameButton);
        AddHoverEffect(settingsButton);
        AddHoverEffect(returnButton);
        AddHoverEffect(audioButton.button);
        AddHoverEffect(displayAndGraphicsButton.button);
        AddHoverEffect(controlButton.button);

        AddHoverEffect(yesNewGameButton);
        AddHoverEffect(noNewGameButton);
        AddHoverEffect(yesCloseGameButton);
        AddHoverEffect(noCloseGameButton);

        SetActiveSettingTab(audioButton);
        OnClickAudio();
    }



    #region MAIN MENU FUNCTIONS
    private void OnClickNewGame()
    {
        if (File.Exists(savePath))
        {
            newGameConfirmationPanel.SetActive(true);
        }
        else
        {
            StartCoroutine(NewGame());
        }
    }

    private void OnClickNoNewGame()
    {
        newGameConfirmationPanel.SetActive(false);
    }

    private void OnClickYesNewGame()
    {
        StartCoroutine(NewGame());
    }

    private IEnumerator NewGame()
    {
        settingsButton.enabled = false;
        closeGameButton.enabled = false;
        newGameButton.enabled = false;
        continueGameButton.enabled = false;
        GameManager.instance.DeleteAll();
        fadeCanvasGroup.FadeIn(1);
        yield return new WaitForSeconds(1f);
        Loader.Load(3);
    }
    private void OnClickContinueGame()
    {
        StartCoroutine(ContinueGame());
    }

    private IEnumerator ContinueGame()
    {
        settingsButton.enabled = false;
        closeGameButton.enabled = false;
        newGameButton.enabled = false;
        continueGameButton.enabled = false;
        fadeCanvasGroup.FadeIn(1);
        yield return new WaitForSeconds(1f);
        Loader.Load(4);
    }

    private void OnClickCloseGame()
    {
        closeGameConfirmationPanel.SetActive(true);
    }

    private void OnClickYesCloseGame()
    {
        StartCoroutine(CloseGame());
    }

    private void OnClickNoCloseGame()
    {
        closeGameConfirmationPanel.SetActive(false);
    }
    private IEnumerator CloseGame()
    {
        fadeCanvasGroup.FadeIn(1);
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }
    #endregion
    #region SETTINGS FUNCTION
    private void OnOpenSettings()
    {
        settingsPanel.SetActive(true);
        Blur();
    }

    private void OnCloseSettings()
    {
        settingsPanel.SetActive(false);
        UnBlur();
    }

    private void OnClickAudioTab()
    {
        OnClickAudio();
        SetActiveSettingTab(audioButton);
    }

    private void OnClickDisplayTab()
    {
        OnClickDisplayAndGraphics();
        SetActiveSettingTab(displayAndGraphicsButton);
    }

    private void OnClickControlTab()
    {
        OnClickControl();
        SetActiveSettingTab(controlButton);
    }

    public void OnClickAudio()
    {
        audioPanel.SetActive(true);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    public void OnClickDisplayAndGraphics()
    {
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(true);
        controlPanel.SetActive(false);
    }

    public void OnClickControl()
    {
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion
    #region UI PROPERTIES
    // ---------------------- BLUR EFFECT ----------------------
    public void Blur()
    {
        if (blurRoutine != null)
        {
            StopCoroutine(blurRoutine);
        }

        blurRoutine = StartCoroutine(AnimateBlur(1f));
    }

    public void UnBlur()
    {
        if (blurRoutine != null)
        {
            StopCoroutine(blurRoutine);
        }

        blurRoutine = StartCoroutine(AnimateBlur(0f));
    }

    private IEnumerator AnimateBlur(float targetAlpha)
    {
        float startAlpha = currentAlpha;
        float elapsed = 0f;

        while (elapsed < blurDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / blurDuration);
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color c = blurImage.color;
            c.a = currentAlpha;
            blurImage.color = c;

            yield return null;
        }

        currentAlpha = targetAlpha;
        Color final = blurImage.color;
        final.a = currentAlpha;
        blurImage.color = final;
    }

     // ---------------------- HOVER SCALING ----------------------
    private void AddHoverEffect(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((_) => StartCoroutine(ScaleButton(button.transform, hoverScale)));

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((_) => StartCoroutine(ScaleButton(button.transform, 1f)));

        var pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((_) => button.transform.localScale = Vector3.one);

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
        trigger.triggers.Add(pointerClick);
    }

    private IEnumerator ScaleButton(Transform button, float targetScale)
    {
        Vector3 startScale = button.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.unscaledDeltaTime * scaleSpeed;
            button.localScale = Vector3.Lerp(startScale, endScale, elapsed);
            yield return null;
        }

        button.localScale = endScale;
    }

    // ---------------------- SETTINGS TAB VISUALS ----------------------
    private void SetActiveSettingTab(SettingTabButton selected)
    {
        ResetSettingTab(audioButton);
        ResetSettingTab(displayAndGraphicsButton);
        ResetSettingTab(controlButton);

        var circle = selected.button.GetComponent<Image>();
        if (circle != null)
        {
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f);
        }

        if (selected.icon != null)
        {
            selected.icon.color = activeColor;
        }

        activeSettingTab = selected;
    }

    private void ResetSettingTab(SettingTabButton tab)
    {
        if (tab == null) return;

        var circle = tab.button.GetComponent<Image>();
        if (circle != null)
        {
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0f);
        }

        if (tab.icon != null)
        {
            tab.icon.color = inactiveColor;
        }
    }
    #endregion
}
