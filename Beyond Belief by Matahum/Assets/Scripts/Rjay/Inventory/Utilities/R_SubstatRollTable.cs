
using System.Collections.Generic;
using UnityEngine;

public static class R_SubstatRollTable
{
    public static readonly Dictionary<R_StatType, float[]> SubstatTiers = new()
    {
        { R_StatType.FlatHP, new float[] { 448.13f, 896.25f, 1344.38f, 1792.5f } },
        { R_StatType.FlatATK, new float[] { 29.18f, 58.35f, 87.52f, 116.7f } },
        { R_StatType.FlatDEF, new float[] { 8.75f, 17.49f, 26.23f, 34.98f } },//original stat. just remove the text and leave the stat     { 34.73f, 69.45f, 104.18f, 138.9f } },
        { R_StatType.PercentHP, new float[] { 8.75f, 17.49f, 26.23f, 34.98f } },
        { R_StatType.PercentATK, new float[] { 8.75f, 17.49f, 26.23f, 34.98f } },
        { R_StatType.PercentDEF, new float[] { 10.94f, 21.87f, 32.81f, 43.74f } },
        { R_StatType.CRITRate, new float[] { 5.84f, 11.67f, 17.51f, 23.34f } },
        { R_StatType.CRITDamage, new float[] { 11.66f, 23.31f, 34.97f, 46.62f } },
        { R_StatType.CooldownReduction, new float[] { 5.84f, 11.67f, 17.51f, 23.34f } },
    };

    // Always return the first tier for starting values
    public static float GetInitialRoll(R_StatType statType)
    {
        if (SubstatTiers.TryGetValue(statType, out var rolls) && rolls.Length > 0)
            return rolls[0];

        Debug.LogWarning($"No substat roll tiers defined for: {statType}");
        return 0f;
    }

    // Return the next higher tier from the current value, if it exists
    public static float GetNextTierRoll(R_StatType statType, float currentValue)
    {
        if (!SubstatTiers.TryGetValue(statType, out var rolls))
        {
            Debug.LogWarning($"No substat roll tiers defined for: {statType}");
            return currentValue;
        }

        for (int i = 0; i < rolls.Length - 1; i++)
        {
            if (Mathf.Approximately(rolls[i], currentValue))
                return rolls[i + 1];
        }

        return currentValue; // Already at max or unknown tier
    }

    // Return full roll pool (if needed for upgrades or debug)
    public static float[] GetAllTiers(R_StatType statType)
    {
        return SubstatTiers.TryGetValue(statType, out var rolls) ? rolls : null;
    }
}
