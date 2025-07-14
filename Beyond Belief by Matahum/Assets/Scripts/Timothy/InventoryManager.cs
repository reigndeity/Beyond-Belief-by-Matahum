using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("ItemSlot Prefab")]
    public GameObject slotPrefab;

    [Header("Main Inventory Slots and Items")]
    public List<ItemSlot> mainInventorySlots = new List<ItemSlot>();
    public List<InventoryItem> mainInventoryItems = new();
    public int mainSlotNumber;

    [Header("Agimat Inventory Slots and Items")]
    public List<ItemSlot> agimatInventorySlots = new List<ItemSlot>();
    public List<InventoryItem> agimatItems = new();
    public int agimatSlotNumber;

    [Header("Max Number of Items")]
    public int MAX_STACK_SIZE = 999;
    private void Awake() => Instance = this;

    //For Getting Pamana Items Only
    public List<InventoryItem> GetPamanaItemOnly()
    {
        return mainInventoryItems.Where(item => item.itemType == ItemType.Pamana).ToList();
    }

    public List<ItemSlot> GetPamanaSlotOnly()
    {
        return mainInventorySlots
         .Where(slot => slot.HasItem() && slot.currentItem.itemType == ItemType.Pamana)
         .ToList();
    }

    //Add Item
    public void AddItem(InventoryItem itemToAdd, List<ItemSlot> slotList, List<InventoryItem> itemList, Transform inventoryParent)
    {
        if (itemToAdd == null) return;

        if (itemToAdd.isStackable)
        {
            int remainingQuantity = itemToAdd.quantity;

            // Step 1: Try stacking into existing slots
            foreach (var slot in slotList)
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

            // Step 2: Use empty slots
            foreach (var slot in slotList)
            {
                if (!slot.HasItem())
                {
                    int stackAmount = Mathf.Min(MAX_STACK_SIZE, remainingQuantity);
                    InventoryItem newStackItem = Instantiate(itemToAdd);
                    newStackItem.name = itemToAdd.itemID;
                    newStackItem.quantity = stackAmount;

                    newStackItem.transform.SetParent(slot.transform, false);
                    slot.SetItem(newStackItem);
                    itemList.Add(newStackItem);

                    remainingQuantity -= stackAmount;

                    if (remainingQuantity <= 0)
                        return;
                }
            }

            // Step 3: Add new slots and fill them
            while (remainingQuantity > 0)
            {
                int stackAmount = Mathf.Min(MAX_STACK_SIZE, remainingQuantity);

                if (itemToAdd.itemType != ItemType.Agimat)
                    AddSlot(mainInventorySlots, inventoryParent, false);
                else
                    AddSlot(agimatInventorySlots, inventoryParent, true);

                // ✅ Safety check: make sure the slot was added
                if (slotList.Count == 0)
                {
                    Debug.LogError("Slot list is empty after attempting to add a slot!");
                    return;
                }

                ItemSlot newSlot = slotList[slotList.Count - 1];
                InventoryItem newStackItem = Instantiate(itemToAdd);
                newStackItem.name = itemToAdd.itemID;
                newStackItem.quantity = stackAmount;

                newStackItem.transform.SetParent(newSlot.transform, false);
                newSlot.SetItem(newStackItem);
                itemList.Add(newStackItem);

                if(newStackItem.itemType == ItemType.Agimat)
                    agimatItems.Add(newStackItem);
                else
                    mainInventoryItems.Add(newStackItem);

                remainingQuantity -= stackAmount;
            }
        }
        else
        {
            // Non-stackable items
            foreach (var slot in slotList)
            {
                if (!slot.HasItem())
                {
                    GameObject newItemObj = Instantiate(itemToAdd.gameObject, slot.transform);
                    newItemObj.name = itemToAdd.itemID;
                    InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();
                    slot.SetItem(newItem);
                    itemList.Add(newItem);
                    return;
                }
            }

            // No empty slot, add one
            if (itemToAdd.itemType != ItemType.Agimat)
                AddSlot(slotList, inventoryParent, false);
            else
                AddSlot(slotList, inventoryParent, true);

            if (slotList.Count == 0)
            {
                Debug.LogError("Slot list is empty after attempting to add a slot!");
                return;
            }

            ItemSlot newSlot = slotList[slotList.Count - 1];
            GameObject newItemObjFinal = Instantiate(itemToAdd.gameObject, newSlot.transform);
            InventoryItem newItemFinal = newItemObjFinal.GetComponent<InventoryItem>();

            if (newItemFinal == null)
            {
                Debug.LogError("The item prefab is missing InventoryItem!");
                return;
            }

            newSlot.SetItem(newItemFinal);
            itemList.Add(newItemFinal);

            if (newItemFinal.itemType == ItemType.Agimat)
                agimatItems.Add(newItemFinal);
            else
                mainInventoryItems.Add(newItemFinal);
            
        }

        //ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.DisableAllButtons();
        ItemActionController.Instance.currentSelectedSlot = null;
    }


    public void AddSlot(List<ItemSlot> slotList, Transform inventoryParent, bool isAgimat = false)
    {
        if (slotPrefab == null)
        {
            Debug.LogError("Slot prefab is null!");
            return;
        }

        GameObject newSlotGO = Instantiate(slotPrefab, inventoryParent);
        ItemSlot newSlot = newSlotGO.GetComponent<ItemSlot>();

        if (newSlot == null)
        {
            Debug.LogError("Slot prefab does not have an ItemSlot component!");
            return;
        }

        if (isAgimat)
        {
            agimatSlotNumber++;
            newSlot.gameObject.name = $"Agimat Item Slot ({agimatSlotNumber})";
            newSlot.isAgimatSlot = true;
            newSlot.allowedType = ItemType.Agimat;

            agimatInventorySlots.Add(newSlot);
        }
        else
        {
            mainSlotNumber++;
            newSlot.name = $"Item Slot ({mainSlotNumber})";
            newSlot.isAgimatSlot = false;

            mainInventorySlots.Add(newSlot);
        }

        slotList.Add(newSlot);
        //ItemActionController.Instance.ShowChoices(false);
        ItemActionController.Instance.DisableAllButtons();
        ItemActionController.Instance.currentSelectedSlot = null;
    }
}
