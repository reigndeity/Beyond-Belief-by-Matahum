using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Player m_player;
    [Header("Player Leveling")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public static event Action OnExpChange;

    [Header("Weapon Leveling")]
    public int weaponLevel = 1;
    public int weaponXP = 0;
    public int maxWeaponLevel = 50;

    [Header("Currency")]
    public int currentGoldCoins = 0;

    [Header("Base Constants")]
    public float baseCriticalRate = 5f;    // 5%
    public float baseCriticalDamage = 50f; // 50%
    public float baseCooldownReduction = 0f;
    public float baseStamina = 100f;
    

    [Header("Final Computed Stats")]
    public float p_maxHealth;
    public float p_currentHealth;
    public int p_attack;
    public float p_defense;
    public float p_criticalRate;
    public float p_criticalDamage;
    public float p_stamina;


    [Header("Stats Modifier")]
    public float p_flatHP;
    public float p_hpPrcnt;
    public float p_flatAtk;
    public float p_atkPrcnt;
    public float p_flatDef;
    public float p_defPrcnt;
    public float p_critRate;
    public float p_critDmg;
    public float p_cooldownReduction;


    void Awake()
    {
        m_player = GetComponent<Player>();
    }
    void Start()
    {
        RecalculateStats();
        p_currentHealth = p_maxHealth;
    }

    public void RecalculateStats()
    {
        int level = Mathf.Clamp(currentLevel, 1, 50);

        float baseHP = PlayerStatGrowthTable.GetHP(level);
        float baseATK = PlayerStatGrowthTable.GetATK(level);
        float baseDEF = PlayerStatGrowthTable.GetDEF(level);

        float oldMax = p_maxHealth;

        p_maxHealth = (baseHP + p_flatHP) * (1f + p_hpPrcnt / 100f);
        p_attack = Mathf.RoundToInt((baseATK + p_flatAtk) * (1f + p_atkPrcnt / 100f));
        p_defense = (baseDEF + p_flatDef) * (1f + p_defPrcnt / 100f);

        p_criticalRate = baseCriticalRate + p_critRate;
        p_criticalDamage = baseCriticalDamage + p_critDmg;
        p_stamina = baseStamina;

        // 🔹 Preserve health ratio when max health changes
        if (oldMax > 0)
        {
            float percent = p_currentHealth / oldMax;
            p_currentHealth = p_maxHealth * percent;
        }

        p_attack += Mathf.RoundToInt(m_player.GetWeaponATK());

        p_currentHealth = Mathf.Clamp(p_currentHealth, 0f, p_maxHealth);
    }

    public void ApplyPamanaBonuses(Dictionary<R_PamanaSlotType, R_InventoryItem> equippedPamanas)
    {
        // Reset all modifiers first
        p_flatHP = 0;
        p_hpPrcnt = 0;
        p_flatAtk = 0;
        p_atkPrcnt = 0;
        p_flatDef = 0;
        p_defPrcnt = 0;
        p_critRate = 0;
        p_critDmg = 0;
        p_cooldownReduction = 0;

        foreach (var kvp in equippedPamanas)
        {
            var item = kvp.Value;
            if (item?.itemData?.pamanaData == null)
                continue;

            // Apply main stat
            ApplyStat(item.itemData.pamanaData.mainStatType, item.itemData.pamanaData.mainStatValue);

            // Apply substats
            foreach (var sub in item.itemData.pamanaData.substats)
            {
                ApplyStat(sub.statType, sub.value);
            }
        }

        RecalculateStats();
    }

    private void ApplyStat(R_StatType statType, float value)
    {
        switch (statType)
        {
            case R_StatType.FlatHP: p_flatHP += value; break;
            case R_StatType.PercentHP: p_hpPrcnt += value; break;
            case R_StatType.FlatATK: p_flatAtk += value; break;
            case R_StatType.PercentATK: p_atkPrcnt += value; break;
            case R_StatType.FlatDEF: p_flatDef += value; break;
            case R_StatType.PercentDEF: p_defPrcnt += value; break;
            case R_StatType.CRITRate: p_critRate += value; break;
            case R_StatType.CRITDamage: p_critDmg += value; break;
            case R_StatType.CooldownReduction: p_cooldownReduction += value; break;
        }
    }

    public void NotifyXPChanged()
    {
        OnExpChange?.Invoke();
    }

}

