using UnityEngine;

public class InventoryFilter : MonoBehaviour
{
    [SerializeField] private Inventory mainInventory;

    public void FilterAll() => mainInventory.ShowAllItems();
    public void FilterConsumables() => mainInventory.FilterAndSortByType(ItemType.Consumable);
    public void FilterMisc() => mainInventory.FilterAndSortByType(ItemType.Misc);
    public void FilterMaterials() => mainInventory.FilterAndSortByType(ItemType.Material);
    public void FilterPamana() => mainInventory.FilterAndSortByType(ItemType.Pamana);
}