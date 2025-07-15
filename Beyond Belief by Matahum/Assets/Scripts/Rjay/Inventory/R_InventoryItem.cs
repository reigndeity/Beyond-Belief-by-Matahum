[System.Serializable]
public class R_InventoryItem
{
    public R_ItemData itemData;
    public int quantity;

    public R_InventoryItem(R_ItemData data, int amount)
    {
        itemData = data;
        quantity = amount;
    }

    public bool IsStackable => itemData != null && itemData.isStackable;
}
