[System.Serializable]
public class R_InventoryItem
{
    public R_ItemData itemData;
    public int quantity;

    // 🔹 Only used for unique tracking of Agimat or similar non-stackables
    public string runtimeID;

    public R_InventoryItem(R_ItemData data, int amount)
    {
        itemData = data;
        quantity = amount;

        // Assign a unique ID if it's a non-stackable Agimat
        if (data != null && data.itemType == R_ItemType.Agimat)
        {
            runtimeID = System.Guid.NewGuid().ToString();
        }
    }

    public bool IsStackable => itemData != null && itemData.isStackable;
}
