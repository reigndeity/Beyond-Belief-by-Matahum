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
        var spawned = GameObject.Instantiate(template);  // clone

        // Assign rarity
        spawned.rarity = R_LootTable.GetRandomRarityFromLevel(playerLevel);

        // 🔹 Assign visuals based on rarity
        R_AgimatVisualSet visuals = visualConfig.GetVisualSet(spawned.rarity);
        spawned.itemBackdropIcon = visuals.itemBackdropIcon;
        spawned.inventoryHeaderImage = visuals.inventoryHeaderImage;
        spawned.inventoryBackdropImage = visuals.inventoryBackdropImage;

        inventory.AddItem(spawned, 1);
        inventoryUI.RefreshUI();

        Debug.Log($"🎉 Spawned Agimat: {spawned.itemName} ({spawned.rarity}) with visuals: " +
                  $"{visuals.itemBackdropIcon?.name}, {visuals.inventoryHeaderImage?.name}, {visuals.inventoryBackdropImage?.name}");
    }
}
