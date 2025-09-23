using System;
using System.Collections;
using TMPro;
using UnityEngine;
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

public class UI_Game : MonoBehaviour
{
    [SerializeField] private R_InventoryUI r_inventoryUI;
    [SerializeField] private R_CharacterDetailsPanel m_characterDetailsPanel;
    [SerializeField] private Player m_player;
    [SerializeField] private PlayerMovement m_playerMovement;

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

    void Awake()
    {
        topOriginalPos = topUIObj.transform.localPosition;
        bottomOriginalPos = bottomUIObj.transform.localPosition;
        topCanvasGroup = topUIObj.GetComponent<CanvasGroup>();
        bottomCanvasGroup = bottomUIObj.GetComponent<CanvasGroup>();
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

        archiveButton.onClick.AddListener(OnClickOpenArchive);
        closeArchiveButton.onClick.AddListener(OnClickCloseArchive);

        //PAUSE
        resumeButton.onClick.AddListener(OnClickResumeGame);
        settingsButton.onClick.AddListener(OnClickSettings);
        returnToMenuButton.onClick.AddListener(OnClickReturnToMenu);

        confirmYesButton.onClick.AddListener(OnConfirmReturnYes);
        confirmNoButton.onClick.AddListener(OnConfirmReturnNo);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PlayerMinimap.instance.IsMapOpen())
            {
                OnClickCloseMapButton();
            }
            else if (IsAnyMajorPanelOpen())
            {
                if (inventoryPanel.activeSelf) OnClickCloseInventory();
                else if (characterDetailPanel.activeSelf) OnClickCloseCharacterDetails();
                else if (m_questButtonManager.IsJournalOpen()) OnClickCloseQuestJournal();
                else if (m_archiveButtonManager.IsArchiveOpen()) OnClickCloseArchive();
            }
            else if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePlaying())
            {
                // ✅ Ignore ESC while in dialogue
                Debug.Log("Pause blocked: dialogue is active.");
            }
            else
            {
                // Toggle pause menu
                if (pauseMenuPanel.activeSelf) OnClickResumeGame();
                else OpenPauseMenu();
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
    }
    public void OnClickTeleport()
    {
        MapManager.instance.TeleportPlayerToSelected();
    }
    public void OnClickCloseMapButton()
    {
        PlayerMinimap.instance.CloseMap();
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
        inventoryPanel.SetActive(true);
        HideUI();
        PauseGame();
    }
    public void OnClickCloseInventory()
    {
        inventoryPanel.SetActive(false);
        ShowUI();
        ResumeGame();
    }
    public void OnClickConsumableFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Consumable);
        Debug.Log("Currently in Consumable");
        currentFilteredInventoryText.text = "Consumables";
        SetActiveFilter(consumablesFilter);
    }
    public void OnClickQuestItemFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.QuestItem);
        Debug.Log("Currently in Quest Item");
        currentFilteredInventoryText.text = "Quest Items";
        SetActiveFilter(questItemsFilter);
    }
    public void OnClickCPamanaFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Pamana);
        Debug.Log("Currently in Pamana");
        currentFilteredInventoryText.text = "Pamana";
        SetActiveFilter(pamanaFilter);
    }
    public void OnClickAgimatFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Agimat);
        Debug.Log("Currently in Agimat");
        currentFilteredInventoryText.text = "Agimat";
        SetActiveFilter(agimatFilter);
    }
    public void OnClickUpgradeMaterialsFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.UpgradeMaterial);
        Debug.Log("Currently in Materials");
        currentFilteredInventoryText.text = "Materials";
        SetActiveFilter(upgradeMaterialsFilter);
    }
    public void OnClickRawIngredientsFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Ingredient);
        Debug.Log("Currently in Raw Ingredients");
        currentFilteredInventoryText.text = "Raw Ingredients";
        SetActiveFilter(rawIngredientsFilter);
    }
    #endregion

    #region CHARACTER DETAILS
    public void OnClickOpenCharacterDetails()
    {
        characterDetailPanel.SetActive(true);
        HideUI();
        PauseGame();
    }
    public void OnClickCloseCharacterDetails()
    {
        characterDetailPanel.SetActive(false);
        ShowUI();
        ResumeGame();
    }
    public void OnClickAttributesTab()
    {
        m_characterDetailsPanel.OnClick_AttributeTab();
        SetActiveTab(attributesTab);
    }
    public void OnClickWeaponTab()
    {
        m_characterDetailsPanel.OnClick_WeaponTab();
        SetActiveTab(weaponTab);
    }
    public void OnClickAgimatTab()
    {
        m_characterDetailsPanel.OnClick_AgimatTab();
        SetActiveTab(agimatTab);
    }
    public void OnClickPamanaTab()
    {
        m_characterDetailsPanel.OnClick_PamanaTab();
        SetActiveTab(pamanaTab);
    }
    public void OnClickDiwataSort()
    {
        m_pamanaPanel.OnClick_EquipSlot_Diwata();
    }
    public void OnClickLihimSort()
    {
        m_pamanaPanel.OnClick_EquipSlot_Lihim();
    }
    public void OnClickSalamangkeroSort()
    {
        m_pamanaPanel.OnClick_EquipSlot_Salamangkero();
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
    public void OnClickOpenQuestJournal()
    {
        m_questButtonManager.OpenJournal();
        HideUI();
        PauseGame();
    }
    public void OnClickCloseQuestJournal()
    {
        m_questButtonManager.ExitJournal();
        ShowUI();
        ResumeGame();
    }
    #endregion

    #region ARCHIVE
    private void OnClickOpenArchive()
    {
        m_archiveButtonManager.OnOpenArchives();
        HideUI();
        PauseGame();
    }
    private void OnClickCloseArchive()
    {
        m_archiveButtonManager.ExitArchives();
        ShowUI();
        ResumeGame();
    }
    #endregion

    #region  PAUSE
    private void OpenPauseMenu()
    {
        PlayerCamera.Instance.SetCursorVisibility(true);
        pauseMenuPanel.SetActive(true);
        HideUI();
        PauseGame();
    }

    private void OnClickResumeGame()
    {
        PlayerCamera.Instance.SetCursorVisibility(false);
        pauseMenuPanel.SetActive(false);
        ShowUI();
        ResumeGame();
    }

    private void OnClickSettings()
    {
        // Leave empty for now
        Debug.Log("Settings menu clicked (not implemented yet).");
    }

    private void OnClickReturnToMenu()
    {
        confirmReturnPanel.SetActive(true);
    }

    private void OnConfirmReturnYes()
    {
        Time.timeScale = 1f; // restore normal speed before scene change
        StartCoroutine(ReturningToMenu());
    }

    IEnumerator ReturningToMenu()
    {
        UI_TransitionController.instance.Fade(0, 1, 0.5f);
        yield return new WaitForSeconds(1f);
        Loader.Load(0);

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
    #endregion

    #region GAME BEHAVIOR
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
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
