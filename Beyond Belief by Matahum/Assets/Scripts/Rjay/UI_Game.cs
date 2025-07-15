using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    
    [SerializeField] Button inventoryButton; // Overall Inventory (No Equipment)
    [SerializeField] Button journalButton; // Keeps track of new findings(?)

    [Header("Inventory Properties")]
    private R_InventoryUI r_inventoryUI;
    [SerializeField] Button consumablesFilterButton;
    [SerializeField] Button questItemsFilterButton;
    [SerializeField] Button pamanaFilterButton;
    [SerializeField] Button agimatFilterButton;
    [SerializeField] Button upgradeMaterialsFilterButton;
    [SerializeField] Button rawIngredientsFilterButton;

    [Header("Character Details Properties")]
    [SerializeField] Button characterButton; // Handles Dialogue, Pamana Equipment, Agimat Equipment
    [SerializeField] GameObject characterPanel;

    [Header("Full Screen Map Properties")]
    [SerializeField] GameObject teleportPanel;
    [SerializeField] Button closeTeleportPanelButton;
    [SerializeField] Button teleportButton;
    public static event Action OnCloseTeleportPanel;

    void Awake()
    {
        r_inventoryUI = FindFirstObjectByType<R_InventoryUI>();
    }
    void Start()
    {
        closeTeleportPanelButton.onClick.AddListener(OnClickCloseTeleportPanel);
        teleportButton.onClick.AddListener(OnClickTeleport);

        consumablesFilterButton.onClick.AddListener(OnClickConsumableFilter);
        questItemsFilterButton.onClick.AddListener(OnClickQuestItemFilter);
        pamanaFilterButton.onClick.AddListener(OnClickCPamanaFilter);
        agimatFilterButton.onClick.AddListener(OnClickAgimatFilter);
        upgradeMaterialsFilterButton.onClick.AddListener(OnClickUpgradeMaterialsFilter);
        rawIngredientsFilterButton.onClick.AddListener(OnClickRawIngredientsFilter);
    }

    public void OnClickCloseTeleportPanel()
    {
        teleportPanel.SetActive(false);
        MapTeleportManager.instance.HideSelection();
    }
    public void OnClickTeleport()
    {
        MapTeleportManager.instance.TeleportPlayerToSelected();
    }

    #region INVENTORY
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
}
