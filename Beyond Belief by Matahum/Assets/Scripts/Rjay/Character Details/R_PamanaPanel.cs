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
    [SerializeField] private GameObject inventoryParent; // container for the pamana grid
    [SerializeField] private GameObject infoPanelObject; // full info panel container

    [Header("Slot Buttons")]
    [SerializeField] private Button slotDiwata;
    [SerializeField] private Button slotLihim;
    [SerializeField] private Button slotSalamangkero;

    [Header("Slot Highlights")]
    [SerializeField] private GameObject highlightDiwata;
    [SerializeField] private GameObject highlightLihim;
    [SerializeField] private GameObject highlightSalamangkero;

    [Header("Inventory Display")]
    [SerializeField] private Transform pamanaListParent;
    [SerializeField] private GameObject pamanaSlotPrefab;

    private List<R_InventoryItem> filteredPamanaItems = new();
    private List<GameObject> uiSlots = new();

    private R_PamanaSlotType? activeFilter = null;
    private R_PamanaSlotType? pendingEquipSlot = null;

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

        foreach (var item in playerInventory.items)
        {
            if (item.itemData == null || item.itemData.itemType != R_ItemType.Pamana)
                continue;

            if (activeFilter.HasValue && item.itemData.pamanaSlot != activeFilter.Value)
                continue; // 🔁 Correct: skip items that don't match

            filteredPamanaItems.Add(item); // ✅ Only add matching items
        }

        foreach (var obj in uiSlots)
            Destroy(obj);
        uiSlots.Clear();

        foreach (var item in filteredPamanaItems)
        {
            GameObject slotObj = Instantiate(pamanaSlotPrefab, pamanaListParent);
            R_PamanaSlotUI slotUI = slotObj.GetComponent<R_PamanaSlotUI>();
            slotUI.Setup(item, this);
            uiSlots.Add(slotObj);
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
        UpdateSlotHighlight();

        inventoryParent.SetActive(true);
        infoPanelObject.SetActive(true);

        // Auto-select equipped Pamana first, else first available
        R_InventoryItem equippedItem = GetEquippedPamanaForSlot(slotType);
        if (equippedItem != null)
        {
            infoPanel.Show(equippedItem.itemData);
        }
        else if (filteredPamanaItems.Count > 0)
        {
            infoPanel.Show(filteredPamanaItems[0].itemData);
        }
        else
        {
            infoPanel.Hide();
        }
    }

    public void OnPamanaSelected(R_InventoryItem item)
    {
        if (infoPanel != null)
            infoPanel.Show(item.itemData);
    }

    private void ClearFilter()
    {
        activeFilter = null;
        pendingEquipSlot = null;
        UpdateSlotHighlight();
    }

    private void UpdateSlotHighlight()
    {
        highlightDiwata.SetActive(pendingEquipSlot == R_PamanaSlotType.Diwata);
        highlightLihim.SetActive(pendingEquipSlot == R_PamanaSlotType.Lihim);
        highlightSalamangkero.SetActive(pendingEquipSlot == R_PamanaSlotType.Salamangkero);
    }

    private R_InventoryItem GetEquippedPamanaForSlot(R_PamanaSlotType slotType)
    {
        // ⛔ Placeholder for now — replace with actual equipped data when we implement equip logic
        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pendingEquipSlot = null;
            activeFilter = null;
            UpdateSlotHighlight();
            inventoryParent.SetActive(false);
            infoPanelObject.SetActive(false);
            RefreshPamanaList();
        }
    }
}
