using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Prefabs and Parents")]
    [SerializeField] private GameObject slotPrefab;

    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<InventoryItem> allItemsInInventory = new();

    private const int MAX_STACK_SIZE = 999;

    private void Start()
    {
        slots = GetComponentsInChildren<ItemSlot>().ToList();

        foreach (var slot in slots)
        {
            InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
            allItemsInInventory.Add(item);
        }
    }

    // Add item and auto-spawn slot
    public void AddItem(InventoryItem itemToAdd)
    {
        if (itemToAdd == null) return;

        if (itemToAdd.isStackable)
        {
            int remainingQuantity = itemToAdd.quantity;

            // Step 1: Try to stack on existing same items
            foreach (var slot in slots)
            {
                if (slot.HasItem() && slot.currentItem.itemID == itemToAdd.itemID && slot.currentItem.quantity < MAX_STACK_SIZE)
                {
                    int spaceLeft = MAX_STACK_SIZE - slot.currentItem.quantity;
                    int amountToAdd = Mathf.Min(spaceLeft, remainingQuantity);

                    slot.currentItem.quantity += amountToAdd;
                    slot.UpdateQuantityText();
                    remainingQuantity -= amountToAdd;

                    if (remainingQuantity <= 0)
                        return;
                }
            }

            // Step 2: Use empty slots before creating new ones
            foreach (var slot in slots)
            {
                if (!slot.HasItem())
                {
                    int stackAmount = Mathf.Min(MAX_STACK_SIZE, remainingQuantity);
                    InventoryItem newStackItem = Instantiate(itemToAdd);
                    newStackItem.quantity = stackAmount;

                    newStackItem.transform.SetParent(slot.transform, false);
                    slot.SetItem(newStackItem);
                    allItemsInInventory.Add(newStackItem);

                    remainingQuantity -= stackAmount;

                    if (remainingQuantity <= 0)
                        return;
                }
            }

            // Step 3: No space — create new slots
            while (remainingQuantity > 0)
            {
                int stackAmount = Mathf.Min(MAX_STACK_SIZE, remainingQuantity);
                InventoryItem newStackItem = Instantiate(itemToAdd);
                newStackItem.quantity = stackAmount;

                if (newStackItem.itemType != ItemType.Agimat) AddSlot(false);
                else AddSlot(true);

                ItemSlot newSlot = slots[slots.Count - 1].GetComponent<ItemSlot>();

                newStackItem.transform.SetParent(newSlot.transform, false);
                newSlot.SetItem(newStackItem);

                slots.Add(newSlot);
                allItemsInInventory.Add(newStackItem);
                remainingQuantity -= stackAmount;
            }
        }
        else
        {
            // Non-stackable items
            foreach (var slot in slots)
            {
                if (!slot.HasItem())
                {
                    GameObject newItemObj = Instantiate(itemToAdd.gameObject, slot.transform);
                    InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();
                    slot.SetItem(newItem);
                    allItemsInInventory.Add(newItem);
                    return;
                }
            }

            // No empty slot, create new one
            if(itemToAdd.itemType != ItemType.Agimat) AddSlot(false);
            else AddSlot(true);

            ItemSlot newSlot = slots[slots.Count - 1];
            GameObject newItemObjFinal = Instantiate(itemToAdd.gameObject, newSlot.transform);
            InventoryItem newItemFinal = newItemObjFinal.GetComponent<InventoryItem>();

            newSlot.SetItem(newItemFinal);
            allItemsInInventory.Add(newItemFinal);
        }
        ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    int mainInventorySlotCount = 0;
    int agimatInventorySlotCount = 0;

    public void AddSlot(bool isAgimat = false)
    {
        GameObject newSlotGO = Instantiate(slotPrefab, gameObject.transform); //slotParent);
        ItemSlot newSlot = newSlotGO.GetComponent<ItemSlot>();

        if (isAgimat)
        {
            agimatInventorySlotCount++;
            newSlot.gameObject.name = $"Agimat Item Slot ({agimatInventorySlotCount})";
            newSlot.isAgimatSlot = true;
            newSlot.allowedType = ItemType.Agimat;
        }
        else
        {
            mainInventorySlotCount++;
            newSlot.name = $"Item Slot ({mainInventorySlotCount})";
            newSlot.isAgimatSlot = false;
        }

        slots.Add(newSlot);
        ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    public void DeleteSlot()
    {
        ItemSlot slotToDelete = ItemActionController.Instance.currentSelectedSlot;
        if (slotToDelete == null || slotToDelete.isSlotLocked) return;

        if (slots.Contains(slotToDelete))
        {
            slots.Remove(slotToDelete);
            Destroy(slotToDelete.gameObject);
        }
        ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    public List<ItemSlot> GetAllSlots()
    {
        return slots;
    }

    public void ClearAllSlots()
    {
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();
    }

    // 🧹 Sort all items based on type: Consumable → Material → Misc → Pamana
    public void SortItemsByType()
    {
        List<InventoryItem> allItems = new List<InventoryItem>();
        foreach (var slot in slots)
        {
            if (slot.HasItem() && !slot.isSlotLocked)
            {
                allItems.Add(slot.currentItem);
                slot.ClearItem();
            }
        }

        allItems = allItems.OrderBy(item => GetTypePriority(item.itemType)).ToList();

        for (int i = 0; i < allItems.Count && i < slots.Count; i++)
        {
            if (!slots[i].isSlotLocked)
            {
                slots[i].SetItem(allItems[i]);
                allItems[i].transform.SetParent(slots[i].transform);
                allItems[i].transform.localPosition = Vector3.zero;
            }      
        }
        ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

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
                slot.gameObject.SetActive(false); // hide empty slots
            }
        }
        ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    public void ShowAllItems()
    {
        foreach (var slot in slots)
        {
            if (slot.HasItem())
            {
                var item = slot.currentItem;
                item.gameObject.SetActive(true);
                item.transform.SetParent(slot.transform);
                item.transform.localPosition = Vector3.zero;
            }

            slot.gameObject.SetActive(true);
        }

        ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.currentSelectedSlot = null;
    }

    public void RefreshInventoryState()
    {
        allItemsInInventory = slots
            .Where(slot => slot.HasItem())
            .Select(slot => slot.currentItem)
            .ToList();
    }

    private void ClearAllVisibleSlots()
    {
        foreach (var slot in slots)
        {
            slot.ClearItem();
            slot.gameObject.SetActive(false); // Ensure it's re-enabled for reuse. Set to true if you want all slots to be online
        }

        foreach (var item in allItemsInInventory)
        {
            item.gameObject.SetActive(false); // Hide all items by default
        }
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
