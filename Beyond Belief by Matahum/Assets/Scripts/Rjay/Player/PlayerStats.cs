using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Leveling")]
    public int currentLevel = 1;
    public int currentExp = 0;

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

    void Start()
    {
        RecalculateStats();
        p_currentHealth = p_maxHealth;
    }

    public void GainXP(int amount)
    {
        currentExp += amount;
        while (currentLevel < 50 && currentExp >= PlayerLevelTable.GetXPRequiredForLevel(currentLevel))
        {
            currentExp -= PlayerLevelTable.GetXPRequiredForLevel(currentLevel);
            currentLevel++;
            Debug.Log($"🔼 Level up! New level: {currentLevel}");
            RecalculateStats();
        }
    }

    public void RecalculateStats()
    {
        int level = Mathf.Clamp(currentLevel, 1, 50);

        float baseHP = PlayerStatGrowthTable.GetHP(level);
        float baseATK = PlayerStatGrowthTable.GetATK(level);
        float baseDEF = PlayerStatGrowthTable.GetDEF(level);

        p_maxHealth = (baseHP + p_flatHP) * (1f + p_hpPrcnt / 100f);
        p_attack = Mathf.RoundToInt((baseATK + p_flatAtk) * (1f + p_atkPrcnt / 100f));
        p_defense = (baseDEF + p_flatDef) * (1f + p_defPrcnt / 100f);

        p_criticalRate = baseCriticalRate + p_critRate;
        p_criticalDamage = baseCriticalDamage + p_critDmg;

        p_cooldownReduction = baseCooldownReduction;

        p_stamina = baseStamina;
    }
}

