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
        spawned.slot1RollValue = new float[2]; // or larger if needed
        spawned.slot2RollValue = new float[2];

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
            if (spawned.slot1Ability is BahayAlitaptap_S1 alitaptap_S1)
            {
                spawned.slot1RollValue[0] = alitaptap_S1.GetRandomDamagePercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is BatoOmo_S1 batoOmo_S1)
            {
                spawned.slot1RollValue[0] = batoOmo_S1.GetRandomShieldDuration(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgKalamansi_S1 kalamansi_S1)
            {
                spawned.slot1RollValue[0] = kalamansi_S1.GetRandomDamagePercent(spawned.rarity);
                spawned.slot1RollValue[1] = kalamansi_S1.GetRandomDamageReductionPercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgLinta_S1 linta_S1)
            {
                spawned.slot1RollValue[0] = linta_S1.GetRandomSelfDamagePercentage(spawned.rarity);
                spawned.slot1RollValue[1] = linta_S1.GetRandomLifeStealPercentage(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgMais_S1 mais_S1)
            {
                spawned.slot1RollValue[0] = mais_S1.GetRandomHealPercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgSaging_S1 saging_S1)
            {
                spawned.slot1RollValue[0] = saging_S1.GetBulletCount(spawned.rarity);
                spawned.slot1RollValue[1] = saging_S1.GetRandomDamagePercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is MutyaNgSampalok_S1 sampalok_S1)
            {
                spawned.slot1RollValue[0] = sampalok_S1.GetRandomDamagePercent(spawned.rarity);
            }
            else if (spawned.slot1Ability is NgipinNgKalabawS1 kalabaw_S1)
            {
                spawned.slot1RollValue[0] = kalabaw_S1.GetRandomIncreaseAttack(spawned.rarity);
            }
            else if (spawned.slot1Ability is NgipinNgKidlat_S1 kidlat_S1)
            {
                spawned.slot1RollValue[0] = kidlat_S1.GetRandomDamagePercent(spawned.rarity);
            }
        }

        if (spawned.slot2Ability != null)
        {
            if (spawned.slot2Ability is BahayAlitaptap_S2 alitaptap_S2)
            {
                //No values needed to be changed
            }
            else if (spawned.slot2Ability is BatoOmo_S2 batoOmo_S2)
            {
                spawned.slot2RollValue[0] = batoOmo_S2.GetRandomDefensePercent(spawned.rarity);
                spawned.slot2RollValue[1] = batoOmo_S2.GetRandomHealthThreshold(spawned.rarity);
            }
            else if (spawned.slot2Ability is MutyaNgKalamansi_S2 kalamansi_S2)
            {
                spawned.slot2RollValue[0] = kalamansi_S2.GetRandomDamagePercent(spawned.rarity);
                spawned.slot2RollValue[1] = kalamansi_S2.GetRandomDefenseReductionPercent(spawned.rarity);
            }
            else if (spawned.slot2Ability is MutyaNgLinta_S2 linta_S2)
            {
                spawned.slot2RollValue[0] = linta_S2.GetRandomLifeStealPercentage(spawned.rarity);
            }
            else if (spawned.slot2Ability is MutyaNgMais_S2 mais_S2)
            {
                spawned.slot2RollValue[0] = mais_S2.GetRandomHealPercent(spawned.rarity);
            }
            else if (spawned.slot2Ability is MutyaNgSaging_S2 saging_S2)
            {
                spawned.slot2RollValue[0] = saging_S2.GetBulletCount(spawned.rarity);
                spawned.slot2RollValue[1] = saging_S2.GetRandomDamagePercent(spawned.rarity);
            }
            else if (spawned.slot2Ability is MutyaNgSampalok_S2 sampalok_S2)
            {
                spawned.slot2RollValue[0] = sampalok_S2.GetRandomHealthRestore(spawned.rarity);
                spawned.slot2RollValue[1] = sampalok_S2.GetRandomStaminaRestore(spawned.rarity);
            }
            else if (spawned.slot2Ability is NgipinNgKalabawS2 kalabaw_S2)
            {
                spawned.slot2RollValue[0] = kalabaw_S2.GetRandomIncreaseDefense(spawned.rarity);
            }   
            else if (spawned.slot2Ability is NgipinNgKidlat_S2 kidlat_S2)
            {
                spawned.slot2RollValue[0] = kidlat_S2.GetRandomDamagePercent(spawned.rarity);
            }
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
          $"[Slot1={string.Join(", ", spawned.slot1RollValue)}, " +
          $"Slot2={string.Join(", ", spawned.slot2RollValue)}]");

    }
}
