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
}
