using UnityEngine;
public enum R_ItemType
{
    Consumable,
    QuestItem,
    UpgradeMaterial,
    Ingredient,
    Pamana,
    Agimat
}

[CreateAssetMenu(menuName = "Inventory/Item Data", fileName = "New Item")]
public class R_ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string description;

    public R_ItemType itemType;

    public bool isStackable;
    [Min(1)] public int maxStackSize = 1;

    // Later: rarity enum, pamana slots, restrictions, etc.
}

