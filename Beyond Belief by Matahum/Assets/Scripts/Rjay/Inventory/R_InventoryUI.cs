using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    [SerializeField] private Button trashButton;
    [SerializeField] private R_ItemActionPrompt itemPrompt;

    private R_InventoryItem selectedItem;

    void Awake()
    {
        trashButton.onClick.AddListener(TryToDelete);
        
    }
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

    if (filteredItems.Count > 0 && infoPanel != null)
    {
        selectedItem = filteredItems[0];
        infoPanel.ShowItem(selectedItem.itemData);

        // 🔹 Hook up Use button + pass in playerInventory to the panel
        if (selectedItem.itemData.itemType == R_ItemType.Consumable)
        {
            var panel = infoPanel.consumablePanelDisplay as R_InfoPanel_Consumable;
            if (panel != null)
            {
                panel.playerInventory = playerInventory;
                panel.SetUseButtonCallback(selectedItem, itemPrompt, RefreshUI);
            }
        }

        trashButton.interactable = true;
        UpdateSelectedSlotVisual();
    }
    else
    {
        selectedItem = null;
        if (infoPanel != null) infoPanel.ClearPanel();
        trashButton.interactable = false;
    }

    trashButton.gameObject.SetActive(currentFilter != R_InventoryFilter.QuestItem);
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
            slotUI.inventoryUI = this;
            
            slotUIs.Add(slotUI);
        }
    }

    public void TryToDelete()
    {
        if (selectedItem != null)
        {
            itemPrompt.Open(selectedItem, R_ActionType.Trash, amount => {
                playerInventory.RemoveItem(selectedItem.itemData, amount);
                RefreshUI();
            });
        }
    }  
    
    public void SetSelectedItem(R_InventoryItem item)
    {
        selectedItem = item;

            if (item != null && infoPanel != null)
            {
                infoPanel.ShowItem(item.itemData);

            if (item.itemData.itemType == R_ItemType.Consumable)
            {
                var panel = infoPanel.consumablePanelDisplay as R_InfoPanel_Consumable;
                if (panel != null)
                {
                    panel.playerInventory = playerInventory;
                    panel.SetUseButtonCallback(item, itemPrompt, RefreshUI);
                 }
            }
        }

        trashButton.interactable = true;
        UpdateSelectedSlotVisual();
    }

    private void UpdateSelectedSlotVisual()
    {
        foreach (var slot in slotUIs)
        {
            slot.SetSelected(slot.RepresentsItem(selectedItem));
        }
    }
}

    

