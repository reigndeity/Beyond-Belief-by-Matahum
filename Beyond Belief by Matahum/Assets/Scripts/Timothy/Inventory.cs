using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum InventoryType
{
    MainInventory,
    PamanaOnly,
    AgimatOnly
}

public class Inventory : MonoBehaviour
{
    [Header("Where the slots will spawn")]
    public Transform inventoryParent;
    public Transform charDetailsInventoryParent;
    public bool isInMain = true;

    [Header("What kind of Inventory")]
    public InventoryType inventoryType;

    [Header("Slots and Items in inventory")]
    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<InventoryItem> items = new List<InventoryItem>();

    //For Swapping Main Inventory to Char Details Pamana Inventory
    public void SwapMainInventory(bool isGoingToMain)
    {
        if (isGoingToMain)
        {
            // Cache children before moving them
            List<Transform> childrenToMove = new List<Transform>();
            foreach (Transform child in charDetailsInventoryParent.transform)
            {
                childrenToMove.Add(child);
            }

            // Now move them
            foreach (Transform child in childrenToMove)
            {
                child.gameObject.SetActive(true);
                child.SetParent(inventoryParent.transform, false);
            }

            isInMain = true;
            ShowAllItems();
        }
        else
        {
            List<Transform> childrenToMove = new List<Transform>();
            foreach (Transform child in inventoryParent.transform)
            {
                childrenToMove.Add(child);
            }

            foreach (Transform child in childrenToMove)
            {
                child.gameObject.SetActive(true);
                child.SetParent(charDetailsInventoryParent.transform, false);
            }

            isInMain = false;
            FilterAndSortByType(ItemType.Pamana);
        }
    }

    //For Swapping Agimat Inventory to Char Details Agimat Inventory
    public void SwapAgimatInventory(bool isGoingToMain)
    {
        if (isGoingToMain)        
            foreach (Transform child in charDetailsInventoryParent.transform)            
                child.SetParent(inventoryParent.transform, false);                  
        else        
            foreach (Transform child in inventoryParent.transform)
                child.SetParent(charDetailsInventoryParent.transform, false);                  
    }
    public void AddItem(InventoryItem item)
    {
        InventoryManager.Instance.AddItem(item, slots, items, inventoryParent);
    }

    public void AddSlot(bool isAgimat = false)
    {
        if (isInMain)       
            InventoryManager.Instance.AddSlot(slots, inventoryParent, isAgimat);
        else
            InventoryManager.Instance.AddSlot(slots, charDetailsInventoryParent, isAgimat);
    }

    //For Main Inventory
    public void FilterAndSortByType(ItemType filterType)
    {
        foreach (var slot in slots)
        {
            if (slot.HasItem())
            {
                bool match = slot.currentItem.itemType == filterType;

                slot.gameObject.SetActive(match);
                slot.currentItem.gameObject.SetActive(match);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
        ItemActionController.Instance.DisableAllButtons();
        //ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    //For Main Inventory
    public void ShowAllItems()
    {
        foreach (var slot in slots)
        {
            if (slot.HasItem())
            {
                slot.gameObject.SetActive(true);
                var item = slot.currentItem;
                item.gameObject.SetActive(true);
                item.transform.SetParent(slot.transform);
                item.transform.localPosition = Vector3.zero;
            }
            else
            {
                slot.gameObject.SetActive(false);
            }

            //slot.gameObject.SetActive(true);
        }
        ItemActionController.Instance.DisableAllButtons();
        //ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    public void ClearAllSlots()
    {
        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(false);
        }
        slots.Clear();
        items.Clear();
    }

    public void SortItemsByType()
    {
        List<InventoryItem> allItems = new List<InventoryItem>();

        // Collect all non-locked items
        foreach (var slot in slots)
        {
            if (slot.HasItem() && !slot.isSlotLocked)
            {
                allItems.Add(slot.currentItem);
                slot.ClearItem();
            }
        }

        // Sort first by type priority, then alphabetically by name
        allItems = allItems
            .OrderBy(item => GetTypePriority(item.itemType))
            .ThenBy(item => item.itemName) // Alphabetical
            .ToList();

        // Place sorted items back
        for (int i = 0, j = 0; i < slots.Count && j < allItems.Count; i++)
        {
            if (!slots[i].isSlotLocked)
            {
                slots[i].SetItem(allItems[j]);
                allItems[j].transform.SetParent(slots[i].transform);
                allItems[j].transform.localPosition = Vector3.zero;
                j++;
            }
        }
        ItemActionController.Instance.DisableAllButtons();
        //ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }


    private int GetTypePriority(ItemType type)
    {
        switch (type)
        {
            case ItemType.Consumable: return 0;
            case ItemType.Material: return 1;
            case ItemType.Misc: return 2;
            case ItemType.Pamana: return 3;
            default: return 999;
        }
    }
}
