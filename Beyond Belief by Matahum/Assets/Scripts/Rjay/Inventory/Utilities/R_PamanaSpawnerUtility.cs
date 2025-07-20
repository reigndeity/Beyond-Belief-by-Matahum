
using UnityEngine;

public static class R_PamanaSpawnerUtility
{
    public static R_ItemData SpawnPamana(R_ItemData template, R_Inventory inventory, R_InventoryUI ui, int simulatedPlayerLevel, R_PamanaVisualConfig visualConfig)
    {
        if (template == null || inventory == null || ui == null)
        {
            Debug.LogWarning("Missing references! Please assign template, inventory, and UI.");
            return null;
        }

        // Step 1: Determine rarity
        R_ItemRarity rarity = R_LootTable.GetRandomRarityFromLevel(simulatedPlayerLevel);

        // Step 2: Clone template
        R_ItemData clone = ScriptableObject.CreateInstance<R_ItemData>();
        clone.itemName = template.itemName;
        clone.itemIcon = template.itemIcon;
        clone.description = template.description;
        clone.itemType = R_ItemType.Pamana;
        clone.set = template.set;
        clone.pamanaSlot = template.pamanaSlot;
        clone.twoPieceBonusDescription = template.twoPieceBonusDescription;
        clone.threePieceBonusDescription = template.threePieceBonusDescription;
        clone.rarity = rarity;

        // Step 3: Assign visuals
        if (visualConfig != null)
        {
            var visuals = visualConfig.GetVisuals(rarity);
            if (visuals != null)
            {
                clone.itemBackdropIcon = visuals.itemBackdropIcon;
                clone.inventoryHeaderImage = visuals.inventoryHeaderImage;
                clone.inventoryBackdropImage = visuals.inventoryBackdropImage;
            }
        }

        // Step 4: Generate pamana stats
        clone.pamanaData = R_PamanaGeneratorUtility.GenerateFrom(clone);

        // Step 5: Add to inventory
        inventory.AddItem(clone, 1);
        ui.RefreshUI();

        Debug.Log($"[SpawnerUtility] Spawned Pamana: {clone.itemName} [{clone.rarity}]");
        return clone;
    }

    public static R_ItemData SpawnRandomPamanaFromPool(R_ItemData[] pamanaTemplates, R_Inventory inventory, R_InventoryUI ui, int simulatedPlayerLevel, R_PamanaVisualConfig visualConfig)
    {
        if (pamanaTemplates == null || pamanaTemplates.Length == 0)
        {
            Debug.LogWarning("Pamana template pool is empty.");
            return null;
        }

        R_ItemData randomTemplate = pamanaTemplates[Random.Range(0, pamanaTemplates.Length)];
        return SpawnPamana(randomTemplate, inventory, ui, simulatedPlayerLevel, visualConfig);
    }
}
