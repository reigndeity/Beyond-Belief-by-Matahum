using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mais_Agimat : Agimat, IEquippable//, IDescription,
{
    private PlayerStats player;
    //private PlayerHealth playerHealth;
    [Header("Icon Sprite")]
    public Sprite firstIcon;
    public Sprite secondIcon;
    [Header("Particle Effects")]
    public GameObject firstSkillPrefab;
    public GameObject secondSkillPrefab;
    public Transform healTransform;

    [Header("Cooldown Properties")]
    public float firstCooldown;
    public float secondCooldown;
    [Header("Skill Properties")]
    private float heal;
    bool canFirst = true;
    bool canSecond = true;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerStats>();
        //  playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    public void Equip()
    {
        //Disable this shit
    }
    public void Equip(ItemSlot itemSlot)
    {
        if (itemSlot.isSlotLocked) return;

        ItemSlot currentItemSlot = GetComponentInParent<ItemSlot>();
        currentItemSlot?.ClearItem();

        //Checking for existing equipped pamana, unequip if there is one
        InventoryItem equippedItem = itemSlot.GetComponentInChildren<InventoryItem>();
        if (equippedItem != null)
        {
            IEquippable equippable = equippedItem.GetComponent<IEquippable>();
            equippable.Unequip();
        }

        InventoryItem item = GetComponent<InventoryItem>();
        itemSlot.SetItem(item);


        isEquipped = true;
    }

    public void Unequip()
    {
        string equipTarget = $"Agimat Inventory";
        GameObject slotObject = GameObject.Find(equipTarget);
        Inventory inventory = slotObject?.GetComponent<Inventory>();
        InventoryItem item = GetComponent<InventoryItem>();

        if (inventory == null || item == null)
            return;

        //Current Item Slot
        ItemSlot currentItemSlot = GetComponentInParent<ItemSlot>();

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

    public override Sprite FirstIcon() => firstIcon;

    public override Sprite SecondIcon() => secondIcon;

    public override float FirstCooldown() => firstCooldown;
    public override float SecondCooldown() => secondCooldown;

    public override bool FirstPassive() => false;
    public override bool SecondPassive() => true;

    public override void FirstAbility()
    {
        if (canFirst)
        {
            canFirst = false;
            heal = 50 * player.p_level;
            player.p_currentHealth += heal;
            if (player.p_currentHealth >= player.p_maxHealth)
            {
                player.p_currentHealth = player.p_maxHealth;
            }
            Debug.Log($"Used {gameObject.name} First Ability. Healed player for {heal} health");
            GameObject healObj = Instantiate(firstSkillPrefab, player.transform.position, Quaternion.identity);
            ParticleSystem ps = healObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(healObj, ps.main.duration);
            }
            else
            {
                Destroy(healObj, 2f);
            }
            Invoke("FirstCooldownTimer", firstCooldown);
        }
    }

    public override void SecondAbility()
    {
        if (canSecond)
        {
            canSecond = false;
            heal = 0.1f * player.p_maxHealth;
            player.p_currentHealth += heal;
            if (player.p_currentHealth >= player.p_maxHealth)
            {
                player.p_currentHealth = player.p_maxHealth;
            }
            Debug.Log($"Used {gameObject.name} Second Ability. Healed player for {heal} health");
            GameObject healObj = Instantiate(secondSkillPrefab, player.transform.position, Quaternion.identity);
            ParticleSystem ps = healObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(healObj, ps.main.duration);
            }
            else
            {
                Destroy(healObj, 2f);
            }
            Invoke("SecondCooldownTimer", secondCooldown);
        }
    }

    void FirstCooldownTimer()
    {
        canFirst = true;
    }

    void SecondCooldownTimer()
    {
        canSecond = true;
    }

    public void DisplayUI()
    {
        //AgimatDescription agimatDesc = FindFirstObjectByType<AgimatDescription>();

        //agimatDesc.agimatName.text = "Mutya ng Mais";
        //agimatDesc.firstAbilityDescription.text = "Golden Kernel (Active): \nActivate to heal self by a huge amount.";
        //agimatDesc.secondAbilityDescription.text = "Endless Harvest (Passive): \nAutomatically heal self by a small amount.";
    }
}
