
using UnityEngine;
using UnityEditor;
using System.IO;

public class R_PamanaGeneratorEditor : EditorWindow
{
    private R_ItemRarity rarity = R_ItemRarity.Common;
    private R_PamanaSlotType slot = R_PamanaSlotType.Diwata;
    private R_PamanaSet set = R_PamanaSet.KatiwalaNgAraw;

    private static readonly string basePath = "Assets/Scripts/Rjay/Inventory/GeneratedPamanas";

    [MenuItem("Tools/Pamana Generator")]
    public static void ShowWindow()
    {
        GetWindow<R_PamanaGeneratorEditor>("Pamana Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Pamana Generator", EditorStyles.boldLabel);
        rarity = (R_ItemRarity)EditorGUILayout.EnumPopup("Rarity", rarity);
        slot = (R_PamanaSlotType)EditorGUILayout.EnumPopup("Slot Type", slot);
        set = (R_PamanaSet)EditorGUILayout.EnumPopup("Pamana Set", set);

        if (GUILayout.Button("Generate Pamana"))
        {
            GeneratePamana();
        }
    }

    void GeneratePamana()
    {
        
        EnsureFolderPathExists(basePath);
        // Legacy check below (kept for fallback)
        if (!AssetDatabase.IsValidFolder(basePath))
    
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedPamanas");
        }

        // Generate PamanaData
        var pamanaData = ScriptableObject.CreateInstance<R_PamanaData>();
        pamanaData.slot = slot;
        pamanaData.rarity = rarity;
        pamanaData.SetupByRarity();

        pamanaData.mainStatType = GetValidMainStat(slot);
        pamanaData.mainStatValue = GetMainStatValue(pamanaData.mainStatType, rarity);

        int subCount = GetRandomSubstatCount(rarity);
        for (int i = 0; i < subCount; i++)
        {
            R_StatType subType;
            do {
                subType = GetRandomSubstatType();
            } while (subType == pamanaData.mainStatType || pamanaData.substats.Exists(s => s.statType == subType));

            var sub = new R_PamanaData.Substat {
                statType = subType,
                value = GetSubstatRoll(subType)
            };
            pamanaData.substats.Add(sub);
        }

        string pamanaName = $"Pamana_{rarity}_{slot}_{System.DateTime.Now.Ticks}";
        string pamanaDataPath = $"{basePath}/{pamanaName}_Data.asset";
        AssetDatabase.CreateAsset(pamanaData, pamanaDataPath);

        // Generate ItemData
        var itemData = ScriptableObject.CreateInstance<R_ItemData>();
        itemData.name = pamanaName;
        itemData.itemName = pamanaName;
        itemData.itemType = R_ItemType.Pamana;
        itemData.rarity = rarity;
        itemData.pamanaSlot = slot;
        itemData.set = set;
        itemData.pamanaData = pamanaData;
        itemData.description = $"A generated {rarity} Pamana.";
        itemData.twoPieceBonusDescription = "Increases ATK by 10%.";
        itemData.threePieceBonusDescription = "Increases ATK by 20%.";

        string itemDataPath = $"{basePath}/{pamanaName}_Item.asset";
        AssetDatabase.CreateAsset(itemData, itemDataPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = itemData;

        Debug.Log($"Generated new Pamana: {pamanaName}");
    }

    R_StatType GetValidMainStat(R_PamanaSlotType slot)
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

    float GetMainStatValue(R_StatType statType, R_ItemRarity rarity)
    {
        // Simplified to use rarity scaling. You can customize this.
        switch (statType)
        {
            case R_StatType.FlatATK: return 47 + 13 * (int)rarity;
            case R_StatType.FlatHP: return 717 + 253 * (int)rarity;
            case R_StatType.PercentATK:
            case R_StatType.PercentHP: return 7f + 7f * (int)rarity;
            case R_StatType.PercentDEF: return 8.7f + 8.7f * (int)rarity;
            case R_StatType.CRITRate: return 4.7f + 4.7f * (int)rarity;
            case R_StatType.CRITDamage: return 9.3f + 9.3f * (int)rarity;
            case R_StatType.CooldownReduction: return 4.7f + 4.7f * (int)rarity;
            default: return 0;
        }
    }

    R_StatType GetRandomSubstatType()
    {
        R_StatType[] types = {
            R_StatType.FlatHP, R_StatType.FlatATK, R_StatType.FlatDEF,
            R_StatType.PercentHP, R_StatType.PercentATK, R_StatType.PercentDEF,
            R_StatType.CRITRate, R_StatType.CRITDamage, R_StatType.CooldownReduction
        };
        return types[Random.Range(0, types.Length)];
    }

    float GetSubstatRoll(R_StatType statType)
    {
        float[] tiers = { 0.25f, 0.5f, 0.75f, 1f };
        float tier = tiers[Random.Range(0, tiers.Length)];

        switch (statType)
        {
            case R_StatType.FlatHP: return 1792.5f * tier;
            case R_StatType.FlatATK: return 116.7f * tier;
            case R_StatType.FlatDEF: return 138.9f * tier;
            case R_StatType.PercentHP:
            case R_StatType.PercentATK: return 34.98f * tier;
            case R_StatType.PercentDEF: return 43.74f * tier;
            case R_StatType.CRITRate: return 23.34f * tier;
            case R_StatType.CRITDamage: return 46.62f * tier;
            case R_StatType.CooldownReduction: return 23.34f * tier;
            default: return 0f;
        }
    }

    int GetRandomSubstatCount(R_ItemRarity rarity)
    {
        switch (rarity)
        {
            case R_ItemRarity.Common: return Random.Range(0, 2);
            case R_ItemRarity.Rare: return Random.Range(1, 3);
            case R_ItemRarity.Epic: return Random.Range(2, 4);
            case R_ItemRarity.Legendary: return Random.Range(3, 5);
            default: return 0;
        }
    }


    void EnsureFolderPathExists(string fullPath)
    {
        string[] folders = fullPath.Split('/');
        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = currentPath + "/" + folders[i];
            if (!AssetDatabase.IsValidFolder(nextPath))
                AssetDatabase.CreateFolder(currentPath, folders[i]);

            currentPath = nextPath;
        }
    }
}
