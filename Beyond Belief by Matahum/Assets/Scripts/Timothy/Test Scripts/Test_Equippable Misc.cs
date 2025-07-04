using Unity.VisualScripting;
using UnityEngine;

public class SampleMisc_Equippable : MonoBehaviour, IEquippable
{
    public bool isItemEquipped;
    public GameObject itGoesWhere;
    private void Start()
    {
        ItemSlot slot = GetComponentInParent<ItemSlot>();
        if (slot != null)
        {
            isEquipped = slot.isEquipmentSlot;
            isItemEquipped = isEquipped;
        }
    }

    [SerializeField]
    public void Equip()
    {
        string equipTarget = $"Miscellaneous Item Slot";
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
        isItemEquipped = isEquipped;
    }

    public void Equip(ItemSlot itemSlot)
    {
        //Disable this Shit
    }

    public bool isEquipped { get; set; }

    public void Unequip()
    {
        string equipTarget = $"Main Inventory";
        GameObject slotObject = GameObject.Find(equipTarget);
        itGoesWhere = slotObject;
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
        isItemEquipped = isEquipped;
    }
}
