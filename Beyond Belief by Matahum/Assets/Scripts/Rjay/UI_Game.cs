using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private R_InventoryUI r_inventoryUI;
    [SerializeField] private R_CharacterDetailsPanel m_characterDetailsPanel;

    
    [Header("UI Game Behavior")]
    private bool isUIHidden = false;

    
    [Header("Inventory Properties")]
    [SerializeField] Button inventoryButton; // Overall Inventory (No Equipment)
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Button closeInventoryButton;
    [Header("------------------------")]
    [SerializeField] Button consumablesFilterButton;
    [SerializeField] Button questItemsFilterButton;
    [SerializeField] Button pamanaFilterButton;
    [SerializeField] Button agimatFilterButton;
    [SerializeField] Button upgradeMaterialsFilterButton;
    [SerializeField] Button rawIngredientsFilterButton;

    [Header("Character Details Properties")]
    [SerializeField] Button characterDetailsButton;
    [SerializeField] GameObject characterDetailPanel;
    [SerializeField] Button closeCharacterDetailsButton;
    [Header("------------------------")]
    [SerializeField] Button attributesTabButton;
    [SerializeField] Button weaponTabButton;
    [SerializeField] Button agimatTabButton;
    [SerializeField] Button pamanaTabButton;
    [Header("------------------------")]
    [SerializeField] Button diwataSlotButton;
    [SerializeField] Button lihimSlotButton;
    [SerializeField] Button salamangkeroSlotButton;
    [SerializeField] private R_PamanaPanel m_pamanaPanel;

    [Header("Quest Journal Properties")]
    [SerializeField] private BB_Quest_ButtonManager m_questButtonManager;
    [SerializeField] Button questButton;
    [SerializeField] Button closeQuestButton;

    [Header("Full Screen Map Properties")]
    [SerializeField] GameObject teleportPanel;
    [SerializeField] Button closeTeleportPanelButton;
    [SerializeField] Button teleportButton;
    public static event Action OnCloseTeleportPanel;

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

    void Awake()
    {
        // r_inventoryUI = FindFirstObjectByType<R_InventoryUI>();
        // m_characterDetailsPanel = FindFirstObjectByType<R_CharacterDetailsPanel>();
        // m_pamanaPanel = FindFirstObjectByType<R_PamanaPanel>();

        topOriginalPos = topUIObj.transform.localPosition;
        bottomOriginalPos = bottomUIObj.transform.localPosition;
        topCanvasGroup = topUIObj.GetComponent<CanvasGroup>();
        bottomCanvasGroup = bottomUIObj.GetComponent<CanvasGroup>();
    }
    void Start()
    {
        closeTeleportPanelButton.onClick.AddListener(OnClickCloseTeleportPanel);
        teleportButton.onClick.AddListener(OnClickTeleport);

        inventoryButton.onClick.AddListener(OnClickOpenInventory);
        closeInventoryButton.onClick.AddListener(OnClickCloseInventory);
        consumablesFilterButton.onClick.AddListener(OnClickConsumableFilter);
        questItemsFilterButton.onClick.AddListener(OnClickQuestItemFilter);
        pamanaFilterButton.onClick.AddListener(OnClickCPamanaFilter);
        agimatFilterButton.onClick.AddListener(OnClickAgimatFilter);
        upgradeMaterialsFilterButton.onClick.AddListener(OnClickUpgradeMaterialsFilter);
        rawIngredientsFilterButton.onClick.AddListener(OnClickRawIngredientsFilter);


        characterDetailsButton.onClick.AddListener(OnClickOpenCharacterDetails);
        closeCharacterDetailsButton.onClick.AddListener(OnClickCloseCharacterDetails);
        attributesTabButton.onClick.AddListener(OnClickAttributesTab);
        weaponTabButton.onClick.AddListener(OnClickWeaponTab);
        agimatTabButton.onClick.AddListener(OnClickAgimatTab);
        pamanaTabButton.onClick.AddListener(OnClickPamanaTab);
        diwataSlotButton.onClick.AddListener(OnClickDiwataSort);
        lihimSlotButton.onClick.AddListener(OnClickLihimSort);
        salamangkeroSlotButton.onClick.AddListener(OnClickSalamangkeroSort);

        questButton.onClick.AddListener(OnClickOpenQuestJournal);
        closeQuestButton.onClick.AddListener(OnClickCloseQuestJournal);
    }

    #region MAP TELEPORT
    public void OnClickCloseTeleportPanel()
    {
        teleportPanel.SetActive(false);
        MapTeleportManager.instance.HideSelection();
    }
    public void OnClickTeleport()
    {
        MapTeleportManager.instance.TeleportPlayerToSelected();
    }
    #endregion
    
    #region INVENTORY
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
    }
    public void OnClickQuestItemFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.QuestItem);
        Debug.Log("Currently in Quest Item");
    }
    public void OnClickCPamanaFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Pamana);
        Debug.Log("Currently in Pamana");
    }
    public void OnClickAgimatFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Agimat);
        Debug.Log("Currently in Agimat");
    }
    public void OnClickUpgradeMaterialsFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.UpgradeMaterial);
        Debug.Log("Currently in Materials");
    }
    public void OnClickRawIngredientsFilter()
    {
        r_inventoryUI.SetFilter(R_InventoryFilter.Ingredient);
        Debug.Log("Currently in Raw Ingredients");
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
    }
    public void OnClickWeaponTab()
    {
        m_characterDetailsPanel.OnClick_WeaponTab();
    }
    public void OnClickAgimatTab()
    {
        m_characterDetailsPanel.OnClick_AgimatTab();
    }
    public void OnClickPamanaTab()
    {
        m_characterDetailsPanel.OnClick_PamanaTab();
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

    #region UI ANIMATION
    public void HideUI()
    {
        StopAllCoroutines();

        StartCoroutine(AnimateUI_Movement(topUIObj, topOriginalPos, topOriginalPos + Vector3.up * hideYOffset));
        StartCoroutine(AnimateUI_Movement(bottomUIObj, bottomOriginalPos, bottomOriginalPos + Vector3.down * hideYOffset));

        StartCoroutine(AnimateUI_Fade(topCanvasGroup, topCanvasGroup.alpha, 0f));
        StartCoroutine(AnimateUI_Fade(bottomCanvasGroup, bottomCanvasGroup.alpha, 0f));
    }
    public void ShowUI()
    {
        StopAllCoroutines();

        StartCoroutine(AnimateUI_Movement(topUIObj, topUIObj.transform.localPosition, topOriginalPos));
        StartCoroutine(AnimateUI_Movement(bottomUIObj, bottomUIObj.transform.localPosition, bottomOriginalPos));

        StartCoroutine(AnimateUI_Fade(topCanvasGroup, topCanvasGroup.alpha, 1f));
        StartCoroutine(AnimateUI_Fade(bottomCanvasGroup, bottomCanvasGroup.alpha, 1f));
    }
    private IEnumerator AnimateUI_Movement(GameObject uiObj, Vector3 fromPos, Vector3 toPos)
    {
        float overshootDistance = 10f;
        float halfDuration = uiMoveDuration * 0.5f;

        Vector3 moveDir = (toPos - fromPos).normalized;
        Vector3 overshootPos = toPos + moveDir * overshootDistance;

        // Phase 1: move to overshoot
        float elapsed1 = 0f;
        while (elapsed1 < halfDuration)
        {
            elapsed1 += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed1 / halfDuration);
            float smoothed = Mathf.SmoothStep(0, 1, t);

            uiObj.transform.localPosition = Vector3.Lerp(fromPos, overshootPos, smoothed);
            yield return null;
        }

        // Phase 2: move to target
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
    }

    public bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }
    #endregion
}
