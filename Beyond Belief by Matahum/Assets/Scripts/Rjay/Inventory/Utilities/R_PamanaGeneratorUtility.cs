using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class R_PamanaGeneratorUtility
{
    public static R_PamanaData GenerateFrom(R_ItemData itemData)
    {
        if (itemData == null || itemData.itemType != R_ItemType.Pamana)
        {
            Debug.LogError("Invalid itemData passed to Pamana generator.");
            return null;
        }

        R_PamanaData pamanaData = ScriptableObject.CreateInstance<R_PamanaData>();
        pamanaData.slot = itemData.pamanaSlot;
        pamanaData.rarity = itemData.rarity;
        pamanaData.SetupByRarity();

        pamanaData.mainStatType = GetValidMainStat(itemData.pamanaSlot);

        // ❌ Old leveling-based curve system (keep for later upgrades)
        // pamanaData.mainStatValue = R_MainStatCurveTable.GetMainStatValue(pamanaData.mainStatType, 0);

        // ✅ New rarity-based roll from real curve table
        pamanaData.mainStatValue = GetMainStatRoll(pamanaData.slot, pamanaData.rarity, pamanaData.mainStatType);

        int subCount = GetRandomSubstatCount(itemData.rarity);
        for (int i = 0; i < subCount; i++)
        {
            R_StatType subType;
            do
            {
                subType = GetRandomSubstatType();
            }
            while (subType == pamanaData.mainStatType || pamanaData.substats.Exists(s => s.statType == subType));

            var sub = new R_PamanaData.Substat
            {
                statType = subType,
                value = GetSubstatRoll(subType)
            };
            pamanaData.substats.Add(sub);
        }

#if UNITY_EDITOR
        string path = "Assets/Scripts/Rjay/Inventory/Item Data/Item Database/Pamana Datas";
        EnsureFolderPathExists(path);

        string assetName = $"PamanaStats_{itemData.name}_{System.DateTime.Now.Ticks}";
        string fullPath = $"{path}/{assetName}.asset";
        AssetDatabase.CreateAsset(pamanaData, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif

        return pamanaData;
    }

    private static R_StatType GetValidMainStat(R_PamanaSlotType slot)
    {
        if (slot == R_PamanaSlotType.Diwata)
            return Random.value > 0.5f ? R_StatType.CRITRate : R_StatType.CRITDamage;
        else if (slot == R_PamanaSlotType.Lihim)
            return Random.value > 0.5f ? R_StatType.FlatATK : R_StatType.FlatHP;
        else
        {
            R_StatType[] valid = { R_StatType.PercentATK, R_StatType.PercentDEF, R_StatType.PercentHP, R_StatType.CooldownReduction };
            return valid[Random.Range(0, valid.Length)];
        }
    }

    private static float GetMainStatRoll(R_PamanaSlotType slot, R_ItemRarity rarity, R_StatType statType)
    {
        if (!R_MainStatCurveTable.MainStatCurves.TryGetValue(statType, out var curve))
        {
            Debug.LogWarning($"No curve defined for {statType}");
            return 0f;
        }

        // Map rarity → index ranges
        (int minIndex, int maxIndex) = rarity switch
        {
            R_ItemRarity.Common    => (0, 1),
            R_ItemRarity.Rare      => (2, 3),
            R_ItemRarity.Epic      => (4, 5),
            R_ItemRarity.Legendary => (6, 7),
            _ => (0, 0)
        };

        float min = curve[minIndex];
        float max = curve[maxIndex];
        float roll = Random.Range(min, max);

        // ✅ Always clamp to 1 decimal place
        return (float)System.Math.Round(roll, 1);
    }

    private static R_StatType GetRandomSubstatType()
    {
        R_StatType[] types = {
            R_StatType.FlatHP, R_StatType.FlatATK, R_StatType.FlatDEF,
            R_StatType.PercentHP, R_StatType.PercentATK, R_StatType.PercentDEF,
            R_StatType.CRITRate, R_StatType.CRITDamage, R_StatType.CooldownReduction
        };
        return types[Random.Range(0, types.Length)];
    }

    private static float GetSubstatRoll(R_StatType statType)
    {
        return R_SubstatRollTable.GetInitialRoll(statType);
    }

    private static int GetRandomSubstatCount(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(0, 2),
            R_ItemRarity.Rare => Random.Range(1, 3),
            R_ItemRarity.Epic => Random.Range(2, 4),
            R_ItemRarity.Legendary => Random.Range(3, 5),
            _ => 0,
        };
    }

#if UNITY_EDITOR
    private static void EnsureFolderPathExists(string fullPath)
    {
        string[] folders = fullPath.Split('/');
        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = $"{currentPath}/{folders[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
                AssetDatabase.CreateFolder(currentPath, folders[i]);

            currentPath = nextPath;
        }
    }
#endif
}
