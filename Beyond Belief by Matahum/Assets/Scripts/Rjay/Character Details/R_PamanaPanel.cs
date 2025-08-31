using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class R_PamanaPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private R_Inventory playerInventory;
    [SerializeField] private PlayerStats playerStats;

    [Header("Info Display")]
    [SerializeField] private R_InfoPanel_Pamana infoPanel;
    [SerializeField] private GameObject inventoryParent;
    [SerializeField] private Transform pamanaSlotContainer;    // actual parent for spawned slots
    [SerializeField] private GameObject infoPanelObject;

    [Header("Slot Buttons")]
    [SerializeField] private Button slotDiwata;
    [SerializeField] private Button slotLihim;
    [SerializeField] private Button slotSalamangkero;

    [Header("Default Empty Slot Sprites")]
    [SerializeField] private Sprite emptyDiwataSprite;
    [SerializeField] private Sprite emptyLihimSprite;
    [SerializeField] private Sprite emptySalamangkeroSprite;

    [Header("Slot Highlights")]
    [SerializeField] private GameObject highlightDiwata;
    [SerializeField] private GameObject highlightLihim;
    [SerializeField] private GameObject highlightSalamangkero;

    [Header("Equipped Icons")]
    [SerializeField] private Image iconDiwata;
    [SerializeField] private Image iconLihim;
    [SerializeField] private Image iconSalamangkero;

    [Header("Inventory Display")]
    [SerializeField] private Transform pamanaListParent;
    [SerializeField] private GameObject pamanaSlotPrefab;

    [Header("Equip/Unequip Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    private List<R_InventoryItem> filteredPamanaItems = new();
    private List<GameObject> uiSlots = new();

    private R_PamanaSlotType? activeFilter = null;
    private R_PamanaSlotType? pendingEquipSlot = null;

    private R_InventoryItem selectedItem;
    private Player player;

    private List<R_PamanaSlotUI> slotUIs = new();


    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        equipButton.onClick.AddListener(EquipSelectedPamana);
        unequipButton.onClick.AddListener(UnequipCurrentSlot);
    }

    private void OnEnable()
    {
        ClearFilter();
        RefreshPamanaList();
        inventoryParent.SetActive(false);
        infoPanelObject.SetActive(false);
    }

    public void RefreshPamanaList()
    {
        filteredPamanaItems.Clear();
        slotUIs.Clear();

        foreach (var item in playerInventory.items)
        {
            if (item.itemData == null || item.itemData.itemType != R_ItemType.Pamana)
                continue;

            if (activeFilter.HasValue && item.itemData.pamanaSlot != activeFilter.Value)
                continue;

            filteredPamanaItems.Add(item);
        }

        foreach (var obj in uiSlots)
            Destroy(obj);
        uiSlots.Clear();

        foreach (var item in filteredPamanaItems)
        {
            GameObject slotObj = Instantiate(pamanaSlotPrefab, pamanaSlotContainer);

            R_PamanaSlotUI slotUI = slotObj.GetComponent<R_PamanaSlotUI>();
            slotUI.Setup(item, this);

            uiSlots.Add(slotObj);
            slotUIs.Add(slotUI); // 👈 keep typed reference
        }
    }


    public void OnClick_EquipSlot_Diwata() => SelectEquipSlot(R_PamanaSlotType.Diwata);
    public void OnClick_EquipSlot_Lihim() => SelectEquipSlot(R_PamanaSlotType.Lihim);
    public void OnClick_EquipSlot_Salamangkero() => SelectEquipSlot(R_PamanaSlotType.Salamangkero);

    private void SelectEquipSlot(R_PamanaSlotType slotType)
    {
        pendingEquipSlot = slotType;
        activeFilter = slotType;

        RefreshPamanaList();
        UpdateSelectedSlotVisual();
        UpdateSlotHighlight();
        inventoryParent.SetActive(true);

        R_InventoryItem equippedItem = GetEquippedPamanaForSlot(slotType);

        if (equippedItem != null)
        {
            selectedItem = equippedItem;
            infoPanel.Show(equippedItem.itemData);
            infoPanelObject.SetActive(true);
            unequipButton.interactable = true;
            equipButton.interactable = false; // already equipped

            UpdateSelectedSlotVisual();
        }
        else if (filteredPamanaItems.Count > 0)
        {
            selectedItem = filteredPamanaItems[0];
            infoPanel.Show(selectedItem.itemData);
            infoPanelObject.SetActive(true);

            bool alreadyEquipped = IsItemEquipped(selectedItem);
            equipButton.interactable = !alreadyEquipped;
            unequipButton.interactable = false;

            UpdateSelectedSlotVisual();
        }
        else
        {
            selectedItem = null;
            infoPanel.Hide();
            infoPanelObject.SetActive(false);
            equipButton.interactable = false;
            unequipButton.interactable = false;
        }
    }

    public void OnPamanaSelected(R_InventoryItem item)
    {
        selectedItem = item;

        if (infoPanel != null)
            infoPanel.Show(item.itemData);

        if (pendingEquipSlot.HasValue)
        {
            var equippedItem = GetEquippedPamanaForSlot(pendingEquipSlot.Value);
            bool isSameSlotMatch = item.itemData.pamanaSlot == pendingEquipSlot.Value;
            bool alreadyEquippedInSlot = equippedItem == item;

            equipButton.interactable = isSameSlotMatch && !alreadyEquippedInSlot;
            unequipButton.interactable = equippedItem != null;
        }
        else
        {
            equipButton.interactable = false;
            unequipButton.interactable = false;
        }

        UpdateSelectedSlotVisual();
    }

    private void EquipSelectedPamana()
    {
        if (selectedItem == null || pendingEquipSlot == null || selectedItem.itemData.pamanaSlot != pendingEquipSlot.Value)
            return;

        player.EquipPamana(selectedItem);
        infoPanel.Show(selectedItem.itemData);
        Debug.Log($"Equipped {selectedItem.itemData.itemName} to {pendingEquipSlot.Value}");

        RefreshPamanaList();
        UpdateSlotHighlight();

        // 🔹 Instant feedback
        equipButton.interactable = false;
        unequipButton.interactable = true;

        UpdateSelectedSlotVisual();
    }


    private void UnequipCurrentSlot()
    {
        if (player == null || pendingEquipSlot == null)
            return;

        player.UnequipPamana(pendingEquipSlot.Value);
        Debug.Log($"Unequipped Pamana from {pendingEquipSlot}");

        RefreshPamanaList();
        UpdateSlotHighlight();

        // 🔹 DO NOT auto-select a new one — just stay on the same item
        if (selectedItem != null)
        {
            infoPanel.Show(selectedItem.itemData);
            infoPanelObject.SetActive(true);

            equipButton.interactable = true;
            unequipButton.interactable = false;
        }
        else
        {
            infoPanel.Hide();
            infoPanelObject.SetActive(false);
            equipButton.interactable = false;
            unequipButton.interactable = false;
        }

        UpdateSelectedSlotVisual();
    }



    private void ClearFilter()
    {
        activeFilter = null;
        pendingEquipSlot = null;
        selectedItem = null;
        unequipButton.interactable = false;
        UpdateSlotHighlight();
    }

    private void UpdateSlotHighlight()
    {
        highlightDiwata.SetActive(pendingEquipSlot == R_PamanaSlotType.Diwata);
        highlightLihim.SetActive(pendingEquipSlot == R_PamanaSlotType.Lihim);
        highlightSalamangkero.SetActive(pendingEquipSlot == R_PamanaSlotType.Salamangkero);
        UpdateEquippedIcons();
    }

    private void UpdateEquippedIcons()
    {
        SetSlotIcon(iconDiwata, GetEquippedPamanaForSlot(R_PamanaSlotType.Diwata), emptyDiwataSprite);
        SetSlotIcon(iconLihim, GetEquippedPamanaForSlot(R_PamanaSlotType.Lihim), emptyLihimSprite);
        SetSlotIcon(iconSalamangkero, GetEquippedPamanaForSlot(R_PamanaSlotType.Salamangkero), emptySalamangkeroSprite);
    }

    private void SetSlotIcon(Image targetImage, R_InventoryItem item, Sprite emptySprite)
    {
        if (item != null && item.itemData != null && item.itemData.itemIcon != null)
        {
            targetImage.sprite = item.itemData.itemIcon;
        }
        else
        {
            targetImage.sprite = emptySprite; // slot-specific fallback
        }

        targetImage.enabled = true; // always enabled
    }


    private R_InventoryItem GetEquippedPamanaForSlot(R_PamanaSlotType slotType)
    {
        return player != null ? player.GetEquippedPamana(slotType) : null;
    }

    public bool IsItemEquipped(R_InventoryItem item)
    {
        return player != null && player.IsPamanaEquipped(item);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pendingEquipSlot = null;
            activeFilter = null;
            selectedItem = null;
            inventoryParent.SetActive(false);
            infoPanelObject.SetActive(false);
            unequipButton.interactable = false;
            UpdateSlotHighlight();
            RefreshPamanaList();
        }
    }

    private void UpdateSelectedSlotVisual()
    {
        foreach (var obj in uiSlots)
        {
            if (obj.TryGetComponent<R_PamanaSlotUI>(out var slotUI))
            {
                slotUI.SetSelected(slotUI.RepresentsItem(selectedItem));
            }
        }
    }
    public Button GetSlotButton(int index)
    {
        if (index >= 0 && index < slotUIs.Count)
            return slotUIs[index].GetButton();
        return null;
    }

}
