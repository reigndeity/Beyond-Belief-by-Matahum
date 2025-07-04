using UnityEngine;
using UnityEngine.UI;

public class PamanaUI : MonoBehaviour, IEquippable, IDescription
{
    public Image pamanaImage;
    public Pamana assignedPamana;
    public string uniqueID;
    public PamanaRarity rarity;
    public int pamanaLevel;
    public string pamanaName;
    public PamanaType pamanaType;
    public SetPieceBonus setPiece;
    public string mainStatType;
    public float mainStatValue;
    public string subStatType;
    public float subStatValue;

    public void Initialize(Pamana pamana)
    {
        assignedPamana = pamana;
        pamanaImage.sprite = pamana.icon; // Assign icon
        uniqueID = pamana.uniqueID;
        rarity = pamana.rarity;
        pamanaLevel = pamana.pamanaLevel;
        pamanaName = pamana.pamanaName;
        pamanaType = pamana.type;
        setPiece = pamana.setPiece;
        mainStatType = pamana.mainStat.statType.ToString();
        mainStatValue = pamana.mainStat.value;
        subStatType = pamana.subStat.statType.ToString();
        subStatValue = pamana.subStat.value;

        InventoryItem inventoryItem = GetComponent<InventoryItem>();
        if (inventoryItem == null) return;
        inventoryItem.SetItemDetails(pamanaName, $"{pamanaName}_{uniqueID}", pamanaImage.sprite, ItemType.Pamana);
    }

    public void Equip()
    {
        string equipTarget = $"{pamanaType} Slot";
        GameObject slotObject = GameObject.Find(equipTarget);
        ItemSlot equipSlot = slotObject?.GetComponent<ItemSlot>();
        if (equipSlot.isSlotLocked) return;

        ItemSlot currentItemSlot = GetComponentInParent<ItemSlot>();
        currentItemSlot?.ClearItem();

        //Checking for existing equipped pamana, unequip if there is one
        InventoryItem equippedItem = equipSlot.GetComponentInChildren<InventoryItem>();
        if (equippedItem != null)
        {
            PamanaUI equippedUI = equippedItem.GetComponent<PamanaUI>();
            equippedUI.Unequip();
        }

        InventoryItem item = GetComponent<InventoryItem>();
        equipSlot.SetItem(item);


        isEquipped = true;
    }

    public void Equip(ItemSlot itemSlot)
    {
        //Disable this Shit
    }

    public void Unequip()
    {
        string equipTarget = $"Main Inventory";
        GameObject slotObject = GameObject.Find(equipTarget);
        Inventory inventory = slotObject?.GetComponent<Inventory>();
        InventoryItem item = GetComponent<InventoryItem>();
        ItemSlot currentSlot = GetComponentInParent<ItemSlot>();
        currentSlot?.ClearItem();

        if (inventory == null || item == null)
            return;

        // Step 1: Find an empty inventory slot
        foreach (var slot in inventory.slots)
        {
            if (!slot.HasItem())
            {
                slot.SetItem(item);
                isEquipped = false;
                return;
            }
        }
        // Step 2: If no empty slots, create a new one
        int previousSlotCount = inventory.slots.Count;
        inventory.AddSlot(); // Assumes this just adds to inventory.slots

        // Step 3: Get the newly added slot (should be the last one)
        if (inventory.slots.Count > previousSlotCount)
        {
            ItemSlot newSlot = inventory.slots[inventory.slots.Count - 1];
            newSlot.SetItem(item);
        }
        isEquipped = false;
    }

    public bool isEquipped { get; set; } = false;

    public void LevelUpPamana()
    {
        if (assignedPamana != null)
        {
            assignedPamana.LevelUp();
        }
        else
        {
            Debug.LogWarning("No Pamana assigned!");
        }
    }

    public void DisplayUI()
    {
        PamanaDescription pamanaDescription = FindFirstObjectByType<PamanaDescription>();
        if (pamanaDescription != null)
        {
            pamanaDescription.pamanaName.text = $"{pamanaType}:{pamanaName}";
            pamanaDescription.mainStatName.text = mainStatType;
            pamanaDescription.mainStatValue.text = mainStatValue.ToString("F2");
            pamanaDescription.subStatName.text = subStatType;
            pamanaDescription.subStatValue.text = subStatValue.ToString("F2");
            pamanaDescription.setPieceName.text = setPiece.setPieceName.ToString();
            pamanaDescription.firstSetPieceValue.text = $"2 Piece: {setPiece.firstSetBonus.statType}      {setPiece.firstSetBonus.value}";
            pamanaDescription.secondSetPieceValue.text = $"3 Piece: {setPiece.secondSetBonus.statType}      {setPiece.secondSetBonus.value}";
        }
    }

    // ⬇️ ADD THIS BELOW your methods (but still inside the class)
    public string pamanaDisplayName => assignedPamana != null ? assignedPamana.pamanaName : pamanaName;
    public Sprite pamanaDisplayIcon => assignedPamana != null ? assignedPamana.icon : pamanaImage.sprite;

}
