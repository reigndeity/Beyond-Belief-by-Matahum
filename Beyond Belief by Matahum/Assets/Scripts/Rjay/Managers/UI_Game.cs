using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class FilterButton
{
    public Button button;
    public Image icon;
}
[System.Serializable]
public class TabButton
{
    public Button button;
    public Image icon;
}
[System.Serializable]
public class QuestTabButton
{
    public Button button;
    public Image icon;
}
[System.Serializable]
public class SettingTabButton
{   
    public Button button;
    public Image icon;
}

public class UI_Game : MonoBehaviour
{
    public static UI_Game Instance;
    [SerializeField] private R_InventoryUI r_inventoryUI;
    [SerializeField] private R_CharacterDetailsPanel m_characterDetailsPanel;
    [SerializeField] private Player m_player;
    [SerializeField] private PlayerMovement m_playerMovement;
    [SerializeField] private PlayerInput m_playerInput;

    [Header("UI Game Behavior")]
    private bool isUIHidden = false;

    [Header("Inventory Properties")]
    [SerializeField] TextMeshProUGUI currentFilteredInventoryText;
    public Button inventoryButton; // Overall Inventory (No Equipment)
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Button closeInventoryButton;
    [Header("------------------------")]
    [SerializeField] private FilterButton consumablesFilter;
    [SerializeField] private FilterButton questItemsFilter;
    [SerializeField] private FilterButton pamanaFilter;
    [SerializeField] private FilterButton agimatFilter;
    [SerializeField] private FilterButton upgradeMaterialsFilter;
    [SerializeField] private FilterButton rawIngredientsFilter;

    [Header("Character Details Properties")]
    public Button characterDetailsButton;
    [SerializeField] GameObject characterDetailPanel;
    [SerializeField] Button closeCharacterDetailsButton;
    [Header("------------------------")]
    [SerializeField] private TabButton attributesTab;
    [SerializeField] private TabButton weaponTab;
    [SerializeField] private TabButton agimatTab;
    [SerializeField] private TabButton pamanaTab;
    private TabButton activeTab;

    [Header("------------------------")]
    [SerializeField] Button diwataSlotButton;
    [SerializeField] Button lihimSlotButton;
    [SerializeField] Button salamangkeroSlotButton;
    [SerializeField] private R_PamanaPanel m_pamanaPanel;

    [Header("Quest Journal Properties")]
    [SerializeField] private BB_Quest_ButtonManager m_questButtonManager;
    public Button questButton;
    [SerializeField] Button closeQuestButton;
    [Header("------------------------")]
    [SerializeField] private QuestTabButton allQuestTab;
    [SerializeField] private QuestTabButton mainQuestTab;
    [SerializeField] private QuestTabButton sideQuestTab;
    [SerializeField] private QuestTabButton completedQuestTab;
    private QuestTabButton activeQuestTab;

    [Header("Archive Properties")]
    [SerializeField] private BB_Archive_ButtonManager m_archiveButtonManager;
    [SerializeField] Button archiveButton;
    [SerializeField] Button closeArchiveButton;

    [Header("Full Screen Map Properties")]
    [SerializeField] GameObject teleportPanel;
    [SerializeField] Button closeTeleportPanelButton;
    [SerializeField] Button teleportButton;
    public Button closeMapButton;
    public static event Action OnCloseTeleportPanel;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private GameObject confirmReturnPanel;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    [Header("Settings Menu")]
    [SerializeField] private GameObject pauseButtonHolder;
    [SerializeField] private Button backToPauseMenuButton;
    [SerializeField] private GameObject settingsContentHolder;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject displayAndGraphicsPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private SettingTabButton audioButton;
    [SerializeField] private SettingTabButton displayAndGraphicsButton;
    [SerializeField] private SettingTabButton controlButton;

    private SettingTabButton activeSettingTab;
    private readonly Color activeColor = new(0.4f, 0.2f, 0f, 1f);
    private readonly Color inactiveColor = Color.white;
    

    [Header("UI Animation")]
    [SerializeField] GameObject topUIObj;
    [SerializeField] GameObject bottomUIObj;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float uiMoveDuration = 0.25f;
    [SerializeField] private float hideYOffset = 350;
    Vector3 topOriginalPos;
    Vector3 bottomOriginalPos;
    private CanvasGroup topCanvasGroup;
    private CanvasGroup bottomCanvasGroup;
    private FilterButton activeFilterButton;
    [SerializeField] Image blurImage;
    [SerializeField] private float blurDuration = 0.5f;
    private float currentAlpha = 0f;
    private Coroutine blurRoutine;
    
    [Header("Button Hover")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleSpeed = 8f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // destroy the whole GameObject, not just the component
            return;
        }

        Instance = this;

        topOriginalPos = topUIObj.transform.localPosition;
        bottomOriginalPos = bottomUIObj.transform.localPosition;
        topCanvasGroup = topUIObj.GetComponent<CanvasGroup>();
        bottomCanvasGroup = bottomUIObj.GetComponent<CanvasGroup>();

        m_playerInput = FindFirstObjectByType<PlayerInput>();
    }
    void Start()
    {
        closeTeleportPanelButton.onClick.AddListener(OnClickCloseTeleportPanel);
        teleportButton.onClick.AddListener(OnClickTeleport);
        closeMapButton.onClick.AddListener(OnClickCloseMapButton);

        inventoryButton.onClick.AddListener(OnClickOpenInventory);
        closeInventoryButton.onClick.AddListener(OnClickCloseInventory);

        consumablesFilter.button.onClick.AddListener(OnClickConsumableFilter);
        questItemsFilter.button.onClick.AddListener(OnClickQuestItemFilter);
        pamanaFilter.button.onClick.AddListener(OnClickCPamanaFilter);
        agimatFilter.button.onClick.AddListener(OnClickAgimatFilter);
        upgradeMaterialsFilter.button.onClick.AddListener(OnClickUpgradeMaterialsFilter);
        rawIngredientsFilter.button.onClick.AddListener(OnClickRawIngredientsFilter);

        characterDetailsButton.onClick.AddListener(OnClickOpenCharacterDetails);
        closeCharacterDetailsButton.onClick.AddListener(OnClickCloseCharacterDetails);
        attributesTab.button.onClick.AddListener(OnClickAttributesTab);
        weaponTab.button.onClick.AddListener(OnClickWeaponTab);
        agimatTab.button.onClick.AddListener(OnClickAgimatTab);
        pamanaTab.button.onClick.AddListener(OnClickPamanaTab);
        diwataSlotButton.onClick.AddListener(OnClickDiwataSort);
        lihimSlotButton.onClick.AddListener(OnClickLihimSort);
        salamangkeroSlotButton.onClick.AddListener(OnClickSalamangkeroSort);

        questButton.onClick.AddListener(OnClickOpenQuestJournal);
        closeQuestButton.onClick.AddListener(OnClickCloseQuestJournal);
        allQuestTab.button.onClick.AddListener(OnClickAllQuest);
        mainQuestTab.button.onClick.AddListener(OnClickMainQuest);
        sideQuestTab.button.onClick.AddListener(OnClickSideQuest);
        completedQuestTab.button.onClick.AddListener(OnClickCompletedQuest);

        archiveButton.onClick.AddListener(OnClickOpenArchive);
        closeArchiveButton.onClick.AddListener(OnClickCloseArchive);

        //PAUSE
        resumeButton.onClick.AddListener(OnClickResumeGame);
        settingsButton.onClick.AddListener(OnClickSettings);
        returnToMenuButton.onClick.AddListener(OnClickReturnToMenu);

        confirmYesButton.onClick.AddListener(OnConfirmReturnYes);
        confirmNoButton.onClick.AddListener(OnConfirmReturnNo);

        backToPauseMenuButton.onClick.AddListener(OnClickBackToPauseMenu);

        audioButton.button.onClick.AddListener(OnClickAudioTab);
        displayAndGraphicsButton.button.onClick.AddListener(OnClickDisplayTab);
        controlButton.button.onClick.AddListener(OnClickControlTab);

        SetActiveSettingTab(audioButton);
        OnClickAudio();

        AddHoverEffect(settingsButton);
        AddHoverEffect(returnToMenuButton);
        AddHoverEffect(resumeButton);
        AddHoverEffect(backToPauseMenuButton);
        AddHoverEffect(audioButton.button);
        AddHoverEffect(displayAndGraphicsButton.button);
        AddHoverEffect(controlButton.button);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !m_player.isDead)
        {
            // Block pause if dialogue is active
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePlaying())
            {
                Debug.Log("Pause blocked: dialogue is active.");
                return;
            }

            // Prevent pause if another major panel or fullscreen map is open
            if (IsAnyMajorPanelOpen() || (PlayerMinimap.instance != null && PlayerMinimap.instance.IsMapOpen()))
            {
                Debug.Log("Pause blocked: major UI or fullscreen map is open.");
                return;
            }

            // --- TOGGLE BEHAVIOR ---
            if (pauseMenuPanel.activeSelf)
            {
                // Resume game
                OnClickResumeGame();

                // Hide cursor when unpausing
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                FindFirstObjectByType<Settings_Manager>().SaveSettingsOnReturn();
                pauseButtonHolder.SetActive(true);
                settingsContentHolder.SetActive(false);
                audioPanel.SetActive(false);
                resumeButton.gameObject.SetActive(true);
            }
            else
            {
                // Open pause menu
                OpenPauseMenu();

                // Show cursor when pausing
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }



        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        // Only allow hotkeys in OpenWorldScene
        if (sceneName != "OpenWorldScene")
            return;

        // Block hotkeys if any major UI, map, pause, or dialogue is active
        bool isAnyPanelOpen = IsAnyMajorPanelOpen() || 
                            (PlayerMinimap.instance != null && PlayerMinimap.instance.IsMapOpen()) || 
                            pauseMenuPanel.activeSelf ||
                            (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePlaying());

        // Journal hotkey
        if (TutorialManager.instance.canHotKeyJournal && Input.GetKeyDown(m_playerInput.questLogKey))
        {
            if (m_questButtonManager.IsJournalOpen())
            {
                OnClickCloseQuestJournal();
            }
            else if (!isAnyPanelOpen)
            {
                OnClickOpenQuestJournal();
            }
        }

        // Character Details hotkey
        if (TutorialManager.instance.canHotKeyCharacterDetails && Input.GetKeyDown(m_playerInput.characterDetailsKey))
        {
            if (characterDetailPanel.activeSelf)
            {
                OnClickCloseCharacterDetails();
            }
            else if (!isAnyPanelOpen)
            {
                OnClickOpenCharacterDetails();
            }
        }

        // Inventory hotkey
        if (TutorialManager.instance.canHotKeyInventory && Input.GetKeyDown(m_playerInput.inventoryKey))
        {
            if (inventoryPanel.activeSelf)
            {
                OnClickCloseInventory();
            }
            else if (!isAnyPanelOpen)
            {
                OnClickOpenInventory();
            }
        }
    }

    void LateUpdate()
    {
        if (PlayerMinimap.instance.IsMapOpen())
        {
            closeMapButton.gameObject.SetActive(true);
        }
        else
        {
            closeMapButton.gameObject.SetActive(false);
        }
    }

    #region MAP
    public void OnClickCloseTeleportPanel()
    {
        teleportPanel.SetActive(false);
        MapManager.instance.HideSelection();

        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickTeleport()
    {
        MapManager.instance.TeleportPlayerToSelected();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickCloseMapButton()
    {
        PlayerMinimap.instance.CloseMap();
        AudioManager.instance.PlayButtonClickSFX();
    }
    #endregion

    #region INVENTORY
    private void SetActiveFilter(FilterButton selected)
    {
        ResetFilter(consumablesFilter);
        ResetFilter(questItemsFilter);
        ResetFilter(pamanaFilter);
        ResetFilter(agimatFilter);
        ResetFilter(upgradeMaterialsFilter);
        ResetFilter(rawIngredientsFilter);

        var circle = selected.button.GetComponent<Image>();
        if (circle != null)
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f); // alpha 1

        if (selected.icon != null)
            selected.icon.color = new Color(0.4f, 0.2f, 0f, 1f); // brown

        activeFilterButton = selected;
    }

    private void ResetFilter(FilterButton filter)
    {
        if (filter == null) return;

        var circle = filter.button.GetComponent<Image>();
        if (circle != null)
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0f); // alpha 0

        if (filter.icon != null)
            filter.icon.color = Color.white;
    }

    public void OnClickOpenInventory()
    {
        PlayerCamera.Instance.SetCursorVisibility(true);

        inventoryPanel.SetActive(true);
        HideUI();
        Blur();
        PauseGame();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickCloseInventory()
    {
        PlayerCamera.Instance.SetCursorVisibility(false);
        inventoryPanel.SetActive(false);
        ShowUI();
        UnBlur();
        ResumeGame();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickConsumableFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Consumable);
        Debug.Log("Currently in Consumable");
        currentFilteredInventoryText.text = "Consumables";
        SetActiveFilter(consumablesFilter);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickQuestItemFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.QuestItem);
        Debug.Log("Currently in Quest Item");
        currentFilteredInventoryText.text = "Quest Items";
        SetActiveFilter(questItemsFilter);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickCPamanaFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Pamana);
        Debug.Log("Currently in Pamana");
        currentFilteredInventoryText.text = "Pamana";
        SetActiveFilter(pamanaFilter);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickAgimatFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Agimat);
        Debug.Log("Currently in Agimat");
        currentFilteredInventoryText.text = "Agimat";
        SetActiveFilter(agimatFilter);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickUpgradeMaterialsFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.UpgradeMaterial);
        Debug.Log("Currently in Materials");
        currentFilteredInventoryText.text = "Materials";
        SetActiveFilter(upgradeMaterialsFilter);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickRawIngredientsFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Ingredient);
        Debug.Log("Currently in Raw Ingredients");
        currentFilteredInventoryText.text = "Collectibles";
        SetActiveFilter(rawIngredientsFilter);
        AudioManager.instance.PlayButtonClickSFX();
    }
    #endregion

    #region CHARACTER DETAILS
    public void OnClickOpenCharacterDetails()
    {
        PlayerCamera.Instance.SetCursorVisibility(true);

        characterDetailPanel.SetActive(true);
        HideUI();
        Blur();
        PauseGame();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickCloseCharacterDetails()
    {
        PlayerCamera.Instance.SetCursorVisibility(false);

        characterDetailPanel.SetActive(false);
        ShowUI();
        UnBlur();
        ResumeGame();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickAttributesTab()
    {
        m_characterDetailsPanel.OnClick_AttributeTab();
        SetActiveTab(attributesTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickWeaponTab()
    {
        m_characterDetailsPanel.OnClick_WeaponTab();
        SetActiveTab(weaponTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickAgimatTab()
    {
        m_characterDetailsPanel.OnClick_AgimatTab();
        SetActiveTab(agimatTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickPamanaTab()
    {
        m_characterDetailsPanel.OnClick_PamanaTab();
        SetActiveTab(pamanaTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickDiwataSort()
    {
        m_pamanaPanel.OnClick_EquipSlot_Diwata();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickLihimSort()
    {
        m_pamanaPanel.OnClick_EquipSlot_Lihim();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickSalamangkeroSort()
    {
        m_pamanaPanel.OnClick_EquipSlot_Salamangkero();
        AudioManager.instance.PlayButtonClickSFX();
    }
    private void SetActiveTab(TabButton selected)
    {
        ResetTab(attributesTab);
        ResetTab(weaponTab);
        ResetTab(agimatTab);
        ResetTab(pamanaTab);

        // Highlight background (button itself)
        var circle = selected.button.GetComponent<Image>();
        if (circle != null)
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f); // alpha 1

        // Highlight icon (brown)
        if (selected.icon != null)
            selected.icon.color = new Color(0.4f, 0.2f, 0f, 1f);

        activeTab = selected;
    }

    private void ResetTab(TabButton tab)
    {
        if (tab == null) return;

        // Reset background (transparent circle)
        var circle = tab.button.GetComponent<Image>();
        if (circle != null)
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0f); // alpha 0

        // Reset icon (white)
        if (tab.icon != null)
            tab.icon.color = Color.white;
    }

    #endregion

    #region QUEST JOURNAL
    private void SetActiveQuestTab(QuestTabButton selected)
    {
        ResetQuestTab(allQuestTab);
        ResetQuestTab(mainQuestTab);
        ResetQuestTab(sideQuestTab);
        ResetQuestTab(completedQuestTab);

        var circle = selected.button.GetComponent<Image>();
        if (circle != null)
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f);

        if (selected.icon != null)
            selected.icon.color = new Color(0.4f, 0.2f, 0f, 1f); // brown

        activeQuestTab = selected;
    }

    private void ResetQuestTab(QuestTabButton tab)
    {
        if (tab == null) return;

        var circle = tab.button.GetComponent<Image>();
        if (circle != null)
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0f);

        if (tab.icon != null)
            tab.icon.color = Color.white;
    }
    public void OnClickOpenQuestJournal()
    {
        PlayerCamera.Instance.SetCursorVisibility(true); // shows cursor when opening

        m_questButtonManager.OpenJournal();
        OnClickAllQuest();
        HideUI();
        Blur();
        PauseGame();
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickCloseQuestJournal()
    {
        PlayerCamera.Instance.SetCursorVisibility(false); // hides cursor when closing

        m_questButtonManager.ExitJournal();
        ShowUI();
        UnBlur();
        ResumeGame();
        AudioManager.instance.PlayButtonClickSFX();
    }

    public void OnClickAllQuest()
    {
        m_questButtonManager.OpenAllQuest();
        SetActiveQuestTab(allQuestTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickMainQuest()
    {
        m_questButtonManager.OpenMainQuest();
        SetActiveQuestTab(mainQuestTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickSideQuest()
    {
        m_questButtonManager.OpenSideQuest();
        SetActiveQuestTab(sideQuestTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickCompletedQuest()
    {
        m_questButtonManager.OpenCompletedQuest();
        SetActiveQuestTab(completedQuestTab);
        AudioManager.instance.PlayButtonClickSFX();
    }
    #endregion

    #region ARCHIVE
    private void OnClickOpenArchive()
    {
        m_archiveButtonManager.OnOpenArchives();
        HideUI();
        Blur();
        PauseGame();
    }
    private void OnClickCloseArchive()
    {
        m_archiveButtonManager.ExitArchives();
        ShowUI();
        UnBlur();
        ResumeGame();
    }
    #endregion

    #region  PAUSE
    public void OpenPauseMenu()
    {
        PlayerCamera.Instance.SetCursorVisibility(true);
        pauseMenuPanel.SetActive(true);
        HideUI();
        Blur();
        PauseGame();

        // Safety: reset settings visuals if player reopened pause menu after using settings
        SetActiveSettingTab(audioButton);
        audioPanel.SetActive(true);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(false);
        AudioManager.instance.PlayButtonClickSFX();
    }


    public void OnClickResumeGame()
    {
        PlayerCamera.Instance.SetCursorVisibility(false);
        pauseMenuPanel.SetActive(false);
        ShowUI();
        UnBlur();
        ResumeGame();
        AudioManager.instance.PlayButtonClickSFX();
    }

    public void OnClickSettings()
    {
        pauseButtonHolder.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        settingsContentHolder.SetActive(true);

        // Reset panels
        audioPanel.SetActive(true);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(false);

        // ✅ Force reset the tab highlight
        SetActiveSettingTab(audioButton);
        AudioManager.instance.PlayButtonClickSFX();
    }


    public void OnClickBackToPauseMenu()
    {
        pauseButtonHolder.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        settingsContentHolder.SetActive(false);

        // Hide all settings panels to prevent overlap
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(false);
        AudioManager.instance.PlayButtonClickSFX();
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
        AudioManager.instance.PlayButtonClickSFX();
    }
    public void OnClickDisplayAndGraphics()
    {
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(true);
        controlPanel.SetActive(false);
        AudioManager.instance.PlayButtonClickSFX();
        
    }
    public void OnClickControl()
    {
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(true);
        AudioManager.instance.PlayButtonClickSFX();
    }



    private void OnClickReturnToMenu()
    {
        confirmReturnPanel.SetActive(true);
        AudioManager.instance.PlayButtonClickSFX();
    }

    private void OnConfirmReturnYes()
    {
        Time.timeScale = 1f; // restore normal speed before scene change
        StartCoroutine(ReturningToMenu());
        AudioManager.instance.PlayButtonClickSFX();
    }

    IEnumerator ReturningToMenu()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0, 1, 0.5f));
        yield return new WaitForSeconds(1f);
        Loader.Load(0);

    }

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

    private void OnConfirmReturnNo()
    {
        confirmReturnPanel.SetActive(false);
    }
    #endregion


    #region UI ANIMATION
    public void HideUI()
    {
        StopAllCoroutines();

        StartCoroutine(AnimateUI_Movement(topUIObj, topOriginalPos, topOriginalPos + Vector3.up * hideYOffset));
        StartCoroutine(AnimateUI_Movement(bottomUIObj, bottomOriginalPos, bottomOriginalPos + Vector3.down * hideYOffset));

        StartCoroutine(AnimateUI_Fade(topCanvasGroup, topCanvasGroup.alpha, 0f));
        StartCoroutine(AnimateUI_Fade(bottomCanvasGroup, bottomCanvasGroup.alpha, 0f));
        InteractionCanvasUI.Instance.HideInteractionPromptUI();
    }
    public void ShowUI()
    {
        StopAllCoroutines();

        StartCoroutine(AnimateUI_Movement(topUIObj, topUIObj.transform.localPosition, topOriginalPos));
        StartCoroutine(AnimateUI_Movement(bottomUIObj, bottomUIObj.transform.localPosition, bottomOriginalPos));

        StartCoroutine(AnimateUI_Fade(topCanvasGroup, topCanvasGroup.alpha, 1f));
        StartCoroutine(AnimateUI_Fade(bottomCanvasGroup, bottomCanvasGroup.alpha, 1f));
        InteractionCanvasUI.Instance.ResetToAutomatic();
    }
    private IEnumerator AnimateUI_Movement(GameObject uiObj, Vector3 fromPos, Vector3 toPos)
    {
        float overshootDistance = 10f;
        float halfDuration = uiMoveDuration * 0.5f;

        Vector3 moveDir = (toPos - fromPos).normalized;
        Vector3 overshootPos = toPos + moveDir * overshootDistance;

        float elapsed1 = 0f;
        while (elapsed1 < halfDuration)
        {
            elapsed1 += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed1 / halfDuration);
            float smoothed = Mathf.SmoothStep(0, 1, t);

            uiObj.transform.localPosition = Vector3.Lerp(fromPos, overshootPos, smoothed);
            yield return null;
        }

        float elapsed2 = 0f;
        while (elapsed2 < halfDuration)
        {
            elapsed2 += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed2 / halfDuration);
            float smoothed = Mathf.SmoothStep(0, 1, t);

            uiObj.transform.localPosition = Vector3.Lerp(overshootPos, toPos, smoothed);
            yield return null;
        }

        uiObj.transform.localPosition = toPos;
    }
    private IEnumerator AnimateUI_Fade(CanvasGroup canvasGroup, float fromAlpha, float toAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float smoothed = Mathf.SmoothStep(0, 1, t);

            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, smoothed);
            yield return null;
        }

        canvasGroup.alpha = toAlpha;
        canvasGroup.interactable = (toAlpha >= 1f);
        canvasGroup.blocksRaycasts = (toAlpha >= 1f);
    }
    public void Blur()
    {
        if (blurRoutine != null) StopCoroutine(blurRoutine);
        blurRoutine = StartCoroutine(AnimateBlur(1f));
    }

    public void UnBlur()
    {
        if (blurRoutine != null) StopCoroutine(blurRoutine);
        blurRoutine = StartCoroutine(AnimateBlur(0f));
    }
    private IEnumerator AnimateBlur(float targetAlpha)
    {
        float startAlpha = currentAlpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // works even if Time.timeScale = 0
            float t = Mathf.Clamp01(elapsed / fadeDuration);
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
    #endregion

    #region GAME BEHAVIOR
    public event Action<bool> OnGamePause;
    public void PauseGame()
    {
        OnGamePause?.Invoke(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        OnGamePause?.Invoke(false);

        Time.timeScale = 1f;
        if (m_player.currentState == PlayerState.FALLING && m_playerMovement.verticalVelocity <= -2)
        {
            m_player.ForceIdleOverride();
        }
    }

    public bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }

    public bool IsAnyMajorPanelOpen()
    {
        return inventoryPanel.activeSelf ||
            characterDetailPanel.activeSelf ||
            m_questButtonManager.IsJournalOpen() ||
            m_archiveButtonManager.IsArchiveOpen();
    }
    #endregion

}
