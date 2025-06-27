using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    public float e_baseMaxHealth = 100f;
    public int e_baseAttack = 10;
    public float e_baseDefense = 5f;
    public float e_baseCriticalRate = 5f;       // In percent
    public float e_baseCriticalDamage = 50f;    // In percent
    public float e_level = 1;

    [Header("Computed Enemy Stats")]
    public float e_currentHealth;
    public float e_maxHealth;
    public int e_attack;
    public float e_defense;
    public float e_criticalRate;
    public float e_criticalDamage;

    void Start()
    {
        RecalculateStats();
    }
    public void RecalculateStats()
    {
        e_maxHealth = e_baseMaxHealth;
        e_attack = e_baseAttack;
        e_defense = e_baseDefense;
        e_criticalRate = e_baseCriticalRate;
        e_criticalDamage = e_baseCriticalDamage;
        e_currentHealth = e_maxHealth;
    }
}
