using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum R_InventoryFilter
{
    Consumable,
    QuestItem,
    Pamana,
    Agimat,
    UpgradeMaterial,
    Ingredient
}

public class R_InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private R_Inventory playerInventory;

    // 🔹 ADD THIS LINE
    [SerializeField] private R_InventoryItemInfoPanel infoPanel;

    private List<R_InventorySlotUI> slotUIs = new List<R_InventorySlotUI>();
    private R_InventoryFilter currentFilter = R_InventoryFilter.Consumable;

    private void Start()
    {
        GenerateSlots();
        RefreshUI();
        SetFilter(R_InventoryFilter.Consumable);
    }

    public void SetFilter(R_InventoryFilter filter)
    {
        currentFilter = filter;
        RefreshUI();
    }

    public void RefreshUI()
    {
        List<R_InventoryItem> filteredItems = new List<R_InventoryItem>();

        foreach (var item in playerInventory.items)
        {
            if (PassesFilter(item.itemData))
            {
                filteredItems.Add(item);
            }
        }

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < filteredItems.Count)
                slotUIs[i].SetSlot(filteredItems[i]);
            else
                slotUIs[i].SetSlot(null);
        }

        // 👇 Auto-select first item for info panel
        if (filteredItems.Count > 0 && infoPanel != null)
        {
            infoPanel.ShowItem(filteredItems[0].itemData);
        }
        else if (infoPanel != null)
        {
            infoPanel.ClearPanel();
        }
    }


    private bool PassesFilter(R_ItemData item)
    {
        return currentFilter switch
        {
            R_InventoryFilter.Consumable => item.itemType == R_ItemType.Consumable,
            R_InventoryFilter.QuestItem => item.itemType == R_ItemType.QuestItem,
            R_InventoryFilter.Pamana => item.itemType == R_ItemType.Pamana,
            R_InventoryFilter.Agimat => item.itemType == R_ItemType.Agimat,
            R_InventoryFilter.UpgradeMaterial => item.itemType == R_ItemType.UpgradeMaterial,
            R_InventoryFilter.Ingredient => item.itemType == R_ItemType.Ingredient,
            _ => false
        };
    }

    private void GenerateSlots()
    {
        for (int i = 0; i < playerInventory.maxSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            var slotUI = go.GetComponent<R_InventorySlotUI>();
            
            // 🔹 PASS THE INFO PANEL TO EACH SLOT
            slotUI.Initialize(infoPanel);
            
            slotUIs.Add(slotUI);
        }
    }
}
