using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Definition & Level")]
    public EnemyDefinition definition;
    [Range(1, 50)] public int e_level = 1;

    [Header("Computed Enemy Stats")]
    public float e_currentHealth;
    public float e_maxHealth;
    public float e_attack;
    public float e_defense;
    public float e_criticalRate;    // %
    public float e_criticalDamage;  // %

    void Start() { RecalculateStats(); }

    public void RecalculateStats()
    {
        if (definition != null)
        {
            e_maxHealth      = StatGrowthUtil.Eval(definition.hpL1,  definition.hpL50,  e_level, definition.curvePowHP,  definition.growthMode);
            e_attack         = StatGrowthUtil.Eval(definition.atkL1, definition.atkL50, e_level, definition.curvePowATK, definition.growthMode);
            e_defense        = StatGrowthUtil.Eval(definition.defL1, definition.defL50, e_level, definition.curvePowDEF, definition.growthMode);
            e_criticalRate   = definition.critRate;
            e_criticalDamage = definition.critDamage;
        }
        // else: keep serialized values if no definition assigned (non-breaking fallback)

        e_currentHealth = Mathf.Clamp(e_currentHealth <= 0 ? e_maxHealth : e_currentHealth, 0f, e_maxHealth);
    }

    public void SetLevel(int newLevel)
    {
        e_level = Mathf.Clamp(newLevel, 1, 50);
        RecalculateStats();
    }

    /// <summary>Returns true if the enemy died.</summary>
    public bool ApplyDamage(float amount)
    {
        e_currentHealth = Mathf.Clamp(e_currentHealth - amount, 0f, e_maxHealth);
        return e_currentHealth <= 0f;
    }
}
