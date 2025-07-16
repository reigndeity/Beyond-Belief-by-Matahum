using UnityEngine;
using System.Collections.Generic;

public enum R_ItemType
{
    Consumable,
    QuestItem,
    UpgradeMaterial,
    Ingredient,
    Pamana,
    Agimat
}

public enum R_PamanaSet
{
    KatiwalaNgAraw,
    HinagpisNgDigmaan,
    KatiwalaNgBuwan,
    MalabulkangManiniil,
    TagapangasiwaNgLayog,
    SoberanoNgUnos,
    YakapNgIlog,
    BulongSaKagubatan,
    PropetaNgKamatayan
}

public enum R_PamanaSlotType
{
    Diwata,
    Lihim,
    Salamangkero
}

public enum R_ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Inventory/Item Data", fileName = "New Item")]
public class R_ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public Sprite itemBackdropIcon;
    public Sprite inventoryHeaderImage;
    public Sprite inventoryBackdropImage;
    [TextArea] public string description;

    public R_ItemType itemType;

    public bool isStackable;
    [Min(1)] public int maxStackSize = 1;

    public bool isConsumable; // Only for QuestItem

    public R_ItemRarity rarity;

    // 🔁 Pamana-specific metadata
    public R_PamanaData pamanaData;
    public R_PamanaSet set;
    public R_PamanaSlotType pamanaSlot;

    [TextArea]
    public string twoPieceBonusDescription;

    [TextArea]
    public string threePieceBonusDescription;
    public R_AgimatAbility slot1Ability;
    public R_AgimatAbility slot2Ability;

    [TextArea]
    public string effectText; // UI-only, displayed in info panel
    public R_ConsumableEffect consumableEffect; // Logic-only
}
