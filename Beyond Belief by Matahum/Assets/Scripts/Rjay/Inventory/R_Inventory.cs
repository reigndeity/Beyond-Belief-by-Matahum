using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class R_Inventory : MonoBehaviour
{
    public List<R_InventoryItem> items = new List<R_InventoryItem>();
    public int maxSlots = 30;

    public bool AddItem(R_ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0)
            return false;

        if (itemData.isStackable)
        {
            var existingItem = items.Find(i => i.itemData == itemData);
            if (existingItem != null)
            {
                int spaceLeft = itemData.maxStackSize - existingItem.quantity;
                int toAdd = Mathf.Min(spaceLeft, amount);
                existingItem.quantity += toAdd;
                amount -= toAdd;

                if (amount <= 0)
                {
                    SortItems(); // 🔹 auto-sort
                    return true;
                }
            }
        }

        while (amount > 0 && items.Count < maxSlots)
        {
            int stackAmount = itemData.isStackable ? Mathf.Min(amount, itemData.maxStackSize) : 1;
            items.Add(new R_InventoryItem(itemData, stackAmount));
            amount -= stackAmount;
        }

        SortItems(); // 🔹 auto-sort
        return amount <= 0;
    }

    public bool RemoveItem(R_ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0)
            return false;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (item.itemData == itemData)
            {
                int removeAmount = Mathf.Min(item.quantity, amount);
                item.quantity -= removeAmount;
                amount -= removeAmount;

                if (item.quantity <= 0)
                    items.RemoveAt(i);

                if (amount <= 0)
                {
                    SortItems(); // 🔹 auto-sort
                    return true;
                }
            }
        }

        SortItems(); // 🔹 still sort what's left
        return false;
    }

    public bool Contains(R_ItemData itemData, int requiredAmount = 1)
    {
        int total = 0;
        foreach (var item in items)
        {
            if (item.itemData == itemData)
                total += item.quantity;
        }

        return total >= requiredAmount;
    }

    // ✅ NEW: Sort logic injected here
    private void SortItems()
    {
        items.Sort(SortOrder);
    }

    private int SortOrder(R_InventoryItem a, R_InventoryItem b)
    {
        // Type group priority
        int typePriorityA = GetItemTypePriority(a.itemData.itemType);
        int typePriorityB = GetItemTypePriority(b.itemData.itemType);
        int typeComparison = typePriorityA.CompareTo(typePriorityB);
        if (typeComparison != 0) return typeComparison;

        switch (a.itemData.itemType)
        {
            case R_ItemType.Consumable:
            case R_ItemType.QuestItem:
            case R_ItemType.Ingredient:
                return a.itemData.itemName.CompareTo(b.itemData.itemName);

            case R_ItemType.UpgradeMaterial:
                int rarityComp = b.itemData.rarity.CompareTo(a.itemData.rarity);
                return rarityComp != 0 ? rarityComp : a.itemData.itemName.CompareTo(b.itemData.itemName);

            case R_ItemType.Agimat:
                int agimatNameComp = a.itemData.itemName.CompareTo(b.itemData.itemName);
                return agimatNameComp != 0 ? agimatNameComp : b.itemData.rarity.CompareTo(a.itemData.rarity);

            case R_ItemType.Pamana:
                int rarityCompare = b.itemData.rarity.CompareTo(a.itemData.rarity);
                if (rarityCompare != 0) return rarityCompare;

                int slotA = GetPamanaSlotPriority(a.itemData.pamanaSlot);
                int slotB = GetPamanaSlotPriority(b.itemData.pamanaSlot);
                return slotA.CompareTo(slotB);

        }

        return 0;
    }

    private int GetItemTypePriority(R_ItemType type)
    {
        return type switch
        {
            R_ItemType.Consumable => 0,
            R_ItemType.QuestItem => 1,
            R_ItemType.Pamana => 2,
            R_ItemType.Agimat => 3,
            R_ItemType.UpgradeMaterial => 4,
            R_ItemType.Ingredient => 5,
            _ => 99
        };
    }

    private int GetPamanaSlotPriority(R_PamanaSlotType slot)
    {
        return slot switch
        {
            R_PamanaSlotType.Diwata => 0,
            R_PamanaSlotType.Lihim => 1,
            R_PamanaSlotType.Salamangkero => 2,
            _ => 99
        };
    }
}
