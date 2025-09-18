using UnityEngine;

public static class R_AgimatSpawnerUtility
{
    public static void SpawnRandomAgimatFromPool(
        R_ItemData[] agimatTemplates,
        R_Inventory inventory,
        R_InventoryUI inventoryUI,
        int playerLevel,
        R_AgimatVisualConfig visualConfig)
    {
        var template = agimatTemplates[Random.Range(0, agimatTemplates.Length)];

        var spawned = ScriptableObject.CreateInstance<R_ItemData>();

        // Copy metadata
        spawned.name = template.name; // keep inspector clean
        spawned.itemName = template.itemName;
        spawned.itemIcon = template.itemIcon;
        spawned.itemBackdropIcon = template.itemBackdropIcon;
        spawned.inventoryHeaderImage = template.inventoryHeaderImage;
        spawned.inventoryBackdropImage = template.inventoryBackdropImage;
        spawned.description = template.description;

        spawned.itemType = R_ItemType.Agimat;
        spawned.rarity = R_LootTable.GetRandomRarityFromLevel(playerLevel);

        // Instantiate abilities
        spawned.slot1Ability = Object.Instantiate(template.slot1Ability);
        spawned.slot2Ability = Object.Instantiate(template.slot2Ability);

        // ✅ Roll ability values based on ability type
        if (spawned.slot1Ability != null)
        {
            if (spawned.slot1Ability is MutyaNgSaging_S1 sagingS1)
            {
                spawned.slot1RollValue = sagingS1.GetBulletCount(spawned.rarity);
                spawned.slot2RollValue = sagingS1.GetRandomDamagePercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgSaging_S2 sagingS2)
            {
                spawned.slot1RollValue = sagingS2.GetBulletCount(spawned.rarity);
                spawned.slot2RollValue = sagingS2.GetRandomDamagePercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgKalamansi_S1 kalamansiS1)
            {
                spawned.slot1RollValue = kalamansiS1.GetRandomDamagePercent(spawned.rarity);
                spawned.slot2RollValue = kalamansiS1.GetRandomDamageReductionPercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgKalamansi_S2 kalamansiS2)
            {
                spawned.slot1RollValue = kalamansiS2.GetRandomDamagePercent(spawned.rarity);
                spawned.slot2RollValue = kalamansiS2.GetRandomDamageReductionPercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgLinta_S1 lintaS1)
            {
                spawned.slot1RollValue = lintaS1.GetRandomSelfDamagePercentage(spawned.rarity);
                spawned.slot2RollValue = lintaS1.GetRandomLifeStealPercentage(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgLinta_S2 lintaS2)
            {
                spawned.slot1RollValue = lintaS2.GetRandomLifeStealPercentage(spawned.rarity);
            }
            else
            {
                // Default case (Ngipin Ng Kidlat, etc.)
                spawned.slot1RollValue = spawned.slot1Ability.GetRandomDamagePercent(spawned.rarity);
            }
        }

        if (spawned.slot2Ability != null && spawned.slot2RollValue == 0f)
        {
            // Default slot2 handling (if not already set by slot1’s special case)
            spawned.slot2RollValue = spawned.slot2Ability.GetRandomDamagePercent(spawned.rarity);
        }

        // Assign visuals
        if (visualConfig != null)
        {
            R_AgimatVisualSet visuals = visualConfig.GetVisualSet(spawned.rarity);
            spawned.itemBackdropIcon = visuals.itemBackdropIcon;
            spawned.inventoryHeaderImage = visuals.inventoryHeaderImage;
            spawned.inventoryBackdropImage = visuals.inventoryBackdropImage;
        }

        // Add to inventory
        inventory.AddItem(spawned, 1);
        inventoryUI.RefreshUI();

        Debug.Log($"🎉 Spawned Agimat: {spawned.itemName} ({spawned.rarity}) " +
                  $"[Slot1={spawned.slot1RollValue:F1}, Slot2={spawned.slot2RollValue:F1}]");
    }
}
