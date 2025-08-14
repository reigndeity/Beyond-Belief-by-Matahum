using UnityEngine;
using System.Collections.Generic;

public class BlacksmithUpgradeManager : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public R_Inventory playerInventory;
    private PlayerStats playerStats;

    private void Awake()
    {
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    // 🔹 Upgrade a specific equipped Pamana
    public void UpgradePamana(R_InventoryItem upgradeMaterialItem, R_ItemData targetPamana, int amount)
    {
        if (upgradeMaterialItem == null || targetPamana == null || targetPamana.pamanaData == null)
            return;

        R_PamanaData pamanaData = targetPamana.pamanaData;

        // Add EXP
        pamanaData.currentExp += upgradeMaterialItem.itemData.xpValue * amount;

        // Level up logic
        while (pamanaData.currentLevel < pamanaData.maxLevel && pamanaData.currentExp >= pamanaData.maxExp)
        {
            pamanaData.currentExp -= pamanaData.maxExp;
            pamanaData.currentLevel++;
            pamanaData.SimulateUpgradeToLevel(pamanaData.currentLevel);
        }

        // Remove used materials
        playerInventory.RemoveItem(upgradeMaterialItem.itemData, amount);

        // Reapply Pamana stats to player
        playerStats.ApplyPamanaBonuses(player.GetEquippedPamanas());

        Debug.Log($"✅ Upgraded {targetPamana.itemName} to Level {pamanaData.currentLevel} ({pamanaData.currentExp}/{pamanaData.maxExp} EXP)");
    }

    // 🔹 Upgrade the player's weapon using weapon EXP materials
    public void UpgradeWeapon(R_InventoryItem upgradeMaterialItem, int amount)
    {
        if (upgradeMaterialItem == null)
            return;

        int totalXP = upgradeMaterialItem.itemData.xpValue * amount;
        player.GainWeaponXP(totalXP);

        // Remove used materials
        playerInventory.RemoveItem(upgradeMaterialItem.itemData, amount);

        Debug.Log($"✅ Weapon gained {totalXP} EXP (Level {playerStats.weaponLevel})");
    }

    // 🔹 Get all Pamana EXP upgrade materials in inventory
    public List<R_InventoryItem> GetPamanaUpgradeMaterials()
    {
        return playerInventory.items.FindAll(i =>
            i.itemData.itemType == R_ItemType.UpgradeMaterial &&
            i.itemData.upgradeMaterialType == R_UpgradeMaterialType.PamanaEXP
        );
    }

    // 🔹 Get all Weapon EXP upgrade materials in inventory
    public List<R_InventoryItem> GetWeaponUpgradeMaterials()
    {
        return playerInventory.items.FindAll(i =>
            i.itemData.itemType == R_ItemType.UpgradeMaterial &&
            i.itemData.upgradeMaterialType == R_UpgradeMaterialType.WeaponEXP
        );
    }

    // 🔹 Get equipped Pamanas from player
    public List<R_ItemData> GetEquippedPamanas()
    {
        var equipped = player.GetEquippedPamanas();
        List<R_ItemData> pamanas = new List<R_ItemData>();

        foreach (var kvp in equipped)
        {
            if (kvp.Value != null && kvp.Value.itemData != null)
                pamanas.Add(kvp.Value.itemData);
        }

        return pamanas;
    }
}
