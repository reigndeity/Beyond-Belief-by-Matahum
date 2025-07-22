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

        // ✅ Deep clone to prevent shared reference issues
        var spawned = ScriptableObject.CreateInstance<R_ItemData>();

        // Copy metadata
        spawned.itemName = template.itemName;
        spawned.itemIcon = template.itemIcon;
        spawned.itemBackdropIcon = template.itemBackdropIcon;
        spawned.inventoryHeaderImage = template.inventoryHeaderImage;
        spawned.inventoryBackdropImage = template.inventoryBackdropImage;
        spawned.description = template.description;

        spawned.itemType = R_ItemType.Agimat;
        spawned.rarity = R_LootTable.GetRandomRarityFromLevel(playerLevel);

        spawned.slot1Ability = Object.Instantiate(template.slot1Ability);
        spawned.slot2Ability = Object.Instantiate(template.slot2Ability);

        // 🔹 Assign visuals based on rarity
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

        Debug.Log($"🎉 Spawned Agimat: {spawned.itemName} ({spawned.rarity}) with visuals: " +
                  $"{spawned.itemBackdropIcon?.name}, {spawned.inventoryHeaderImage?.name}, {spawned.inventoryBackdropImage?.name}");
    }
}
