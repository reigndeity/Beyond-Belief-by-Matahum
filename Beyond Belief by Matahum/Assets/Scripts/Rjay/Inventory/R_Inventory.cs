using System.Collections.Generic;
using UnityEngine;

public class R_Inventory : MonoBehaviour
{
    public List<R_InventoryItem> items = new List<R_InventoryItem>();
    public int maxSlots = 30;

    public bool AddItem(R_ItemData itemData, int amount)
    {
        if (itemData == null || amount <= 0)
            return false;

        // If item is stackable, try to add to existing stack
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
                    return true; // all added
            }
        }

        // Add new stacks or unique items
        while (amount > 0 && items.Count < maxSlots)
        {
            int stackAmount = itemData.isStackable ? Mathf.Min(amount, itemData.maxStackSize) : 1;
            items.Add(new R_InventoryItem(itemData, stackAmount));
            amount -= stackAmount;
        }

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
                    return true;
            }
        }

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
}
