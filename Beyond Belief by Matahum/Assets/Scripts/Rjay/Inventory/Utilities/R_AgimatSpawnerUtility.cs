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

        // ✅ Roll ability values immediately
        if (spawned.slot1Ability != null)
            spawned.slot1RollValue = spawned.slot1Ability.GetRandomDamagePercent(spawned.rarity);
        if (spawned.slot2Ability != null)
            spawned.slot2RollValue = spawned.slot2Ability.GetRandomDamagePercent(spawned.rarity);

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
                  $"[Slot1={spawned.slot1RollValue:F1}%, Slot2={spawned.slot2RollValue:F1}%]");
    }
}
