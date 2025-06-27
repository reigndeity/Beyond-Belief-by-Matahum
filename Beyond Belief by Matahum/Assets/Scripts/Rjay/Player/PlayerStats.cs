using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Player Stats")]
    public float baseHealth = 100f;
    public float baseAttack = 15;
    public float baseDefense = 35;
    public float baseCriticalRate = 5f; // this means 5%
    public float baseCriticalDamage = 50f; // this means 50%
    public float baseLevel = 1;

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

    void Start()
    {
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        p_maxHealth = (baseHealth + p_flatHP) * (1f + p_hpPrcnt / 100f);
        p_attack = Mathf.RoundToInt((baseAttack + p_flatAtk) * (1f + p_atkPrcnt / 100f));
        p_defense = (baseDefense + p_flatDef) * (1f + p_defPrcnt / 100f);
        p_criticalRate = baseCriticalRate + p_critRate;
        p_criticalDamage = baseCriticalDamage + p_critDmg;
    }
}
