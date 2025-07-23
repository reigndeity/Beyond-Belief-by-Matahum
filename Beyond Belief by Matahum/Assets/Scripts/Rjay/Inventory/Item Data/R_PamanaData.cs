using UnityEngine;
using System.Collections.Generic;

public enum R_StatType
{
    CRITRate,
    CRITDamage,
    FlatATK,
    FlatDEF,
    FlatHP,
    PercentATK,
    PercentDEF,
    PercentHP,
    CooldownReduction
}

[CreateAssetMenu(menuName = "Inventory/Pamana Data", fileName = "New Pamana Data")]
public class R_PamanaData : ScriptableObject
{
    public R_PamanaSet set;
    public R_PamanaSlotType slot;
    public R_ItemRarity rarity;

    public int currentLevel = 0;
    public int currentExp = 0;

    public R_StatType mainStatType;
    public float mainStatValue;

    [System.Serializable]
    public class Substat
    {
        public R_StatType statType;
        public float value;
    }

    public List<Substat> substats = new();

    public int maxLevel;
    public int maxExp;


    public void SetupByRarity()
    {
        switch (rarity)
        {
            case R_ItemRarity.Common:
                maxLevel = 5;
                maxExp = 11000;
                break;
            case R_ItemRarity.Rare:
                maxLevel = 10;
                maxExp = 35000;
                break;
            case R_ItemRarity.Epic:
                maxLevel = 15;
                maxExp = 95000;
                break;
            case R_ItemRarity.Legendary:
                maxLevel = 20;
                maxExp = 270475;
                break;
        }
    }

    public void SimulateUpgradeToLevel(int targetLevel)
    {
        int[] upgradeMilestones = { 5, 10, 15, 20 };
        int maxSubstatsAllowed = rarity switch
        {
            R_ItemRarity.Common => 1,
            R_ItemRarity.Rare => 2,
            R_ItemRarity.Epic => 3,
            R_ItemRarity.Legendary => 4,
            _ => 0
        };

        int upgradesToPerform = 0;
        foreach (int milestone in upgradeMilestones)
        {
            if (targetLevel >= milestone)
                upgradesToPerform++;
        }

        for (int i = 0; i < upgradesToPerform; i++)
        {
            if (substats.Count < maxSubstatsAllowed)
            {
                // Add new substat (Tier 1)
                R_StatType newType;
                do {
                    newType = GetRandomSubstatType();
                } while (newType == mainStatType || substats.Exists(s => s.statType == newType));

                var newSub = new Substat
                {
                    statType = newType,
                    value = R_SubstatRollTable.GetInitialRoll(newType)
                };
                substats.Add(newSub);
            }
            else
            {
                // Upgrade existing substat
                int index = UnityEngine.Random.Range(0, substats.Count);
                var sub = substats[index];
                sub.value = R_SubstatRollTable.GetNextTierRoll(sub.statType, sub.value);
            }
        }
    }

    private R_StatType GetRandomSubstatType()
    {
        R_StatType[] all = {
            R_StatType.FlatHP, R_StatType.FlatATK, R_StatType.FlatDEF,
            R_StatType.PercentHP, R_StatType.PercentATK, R_StatType.PercentDEF,
            R_StatType.CRITRate, R_StatType.CRITDamage, R_StatType.CooldownReduction
        };

        return all[UnityEngine.Random.Range(0, all.Length)];
    }

}