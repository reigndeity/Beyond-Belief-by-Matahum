using System;
using UnityEngine;

public class Temp_PlayerStats : MonoBehaviour
{
    public event Action OnStatsUpdated;
    [Header("Base Player Stats")]
    public float baseHealth = 100f;
    public float baseAttack = 15;
    public float baseDefense = 57;
    public float baseCriticalRate = 5f;
    public float baseCriticalDmage = 50f;

    [Header("Computed Player Stats")]
    public float p_maxHealth;
    public float p_currentHealth;
    public int p_attack;
    public float p_defense;
    public float p_criticalRate;
    public float p_criticalDamage;
    public float p_moveSpeed;
    public float p_level;

    [Header("Stats Modifier")]
    public float p_flatHP;
    public float p_hpPrcnt;
    public float p_flatAtk;
    public float p_atkPrcnt;
    public float p_flatDef;
    public float p_defPrcnt;
    public float p_critRate;
    public float p_critDmg;

    private void Start()
    {
        p_currentHealth = p_maxHealth;
    }
    private void Update()
    {
        StatCalculations();
    }
    public void StatCalculations()
    {
        float maxLevel = 50f;
        /*float level = PlayerLevel.Instance.currentLevel; // make sure this is updated properly

        baseHealth = InterpolateStat(911.79f, 5766.03f, level, maxLevel);
        baseAttack = InterpolateStat(17.81f, 112.62f, level, maxLevel);
        baseDefense = InterpolateStat(57.23f, 139.93f, level, maxLevel);*/

        p_maxHealth = (baseHealth + p_flatHP) * (1f + (p_hpPrcnt / 100));
        p_attack = Mathf.RoundToInt((baseAttack + p_flatAtk) * (1 + (p_atkPrcnt / 100)));
        p_defense = (baseDefense + p_flatDef) * (1f + (p_defPrcnt / 100));
        p_criticalRate = baseCriticalRate + p_critRate;
        p_criticalDamage = baseCriticalDmage + p_critDmg;

        OnStatsUpdated?.Invoke(); // Notify other systems when stats update
    }

    public void ApplyStat(PamanaStat stat, float value)
    {
        switch (stat.statType)
        {
            case StatType.CritRate: p_critRate += stat.value; break;
            case StatType.CritDmg: p_critDmg += stat.value; break;
            case StatType.AtkPrcnt: p_atkPrcnt += stat.value; break;
            case StatType.FlatAtk: p_flatAtk += stat.value; break;
            case StatType.HpPrcnt: p_hpPrcnt += stat.value; break;
            case StatType.FlatHp: p_flatHP += stat.value; break;
            case StatType.DefPrcnt: p_defPrcnt += stat.value; break;
            case StatType.FlatDef: p_flatDef += stat.value; break;
        }
        StatCalculations();
    }

    public void RemoveStat(PamanaStat stat, float value)
    {
        switch (stat.statType)
        {
            case StatType.CritRate: p_critRate -= stat.value; break;
            case StatType.CritDmg: p_critDmg -= stat.value; break;
            case StatType.AtkPrcnt: p_atkPrcnt -= stat.value; break;
            case StatType.FlatAtk: p_flatAtk -= stat.value; break;
            case StatType.HpPrcnt: p_hpPrcnt -= stat.value; break;
            case StatType.FlatHp: p_flatHP -= stat.value; break;
            case StatType.DefPrcnt: p_defPrcnt -= stat.value; break;
            case StatType.FlatDef: p_flatDef -= stat.value; break;
        }
        StatCalculations();
    }
    float InterpolateStat(float min, float max, float level, float maxLevel)
    {
        return Mathf.Lerp(min, max, level / maxLevel);
    }
}
