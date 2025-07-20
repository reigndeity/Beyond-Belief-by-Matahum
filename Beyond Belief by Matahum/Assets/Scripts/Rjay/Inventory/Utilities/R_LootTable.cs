
using UnityEngine;

public static class R_LootTable
{
    public static R_ItemRarity GetRandomRarityFromLevel(int playerLevel)
    {
        float roll = Random.value;
        R_ItemRarity result;

        if (playerLevel >= 40)
        {
            result = roll < 0.8f ? R_ItemRarity.Legendary : R_ItemRarity.Epic;
        }
        else if (playerLevel >= 30)
        {
            result = roll < 0.8f ? R_ItemRarity.Rare : R_ItemRarity.Epic;
        }
        else if (playerLevel >= 15)
        {
            result = roll < 0.8f ? R_ItemRarity.Common : R_ItemRarity.Rare;
        }
        else
        {
            result = R_ItemRarity.Common;
        }

        Debug.Log($"[LootTable] Simulated Level: {playerLevel}, Roll: {roll:F2}, Result: {result}");
        return result;
    }
}
