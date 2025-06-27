using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private PlayerStats m_playerStats;
    private EnemyStats m_enemyStats;
    void Start()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_enemyStats = GetComponent<EnemyStats>();
    }

    public void TakeDamage(float damage)
    {
        bool isCriticalHit = Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        float damageReduction = m_enemyStats.e_defense * 0.66f; // Defense Scaling
        float reducedDamage = damage - damageReduction; // Damage after Defense
        if (isCriticalHit)
        {
            reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f)); // Crit multiplier if critical hit
        }
        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1); // If defense is greater, cap the damage at 1

        m_enemyStats.e_currentHealth -= finalDamage; // Final Damage
        m_enemyStats.e_currentHealth = Mathf.Clamp(m_enemyStats.e_currentHealth, 0f, m_enemyStats.e_maxHealth); // Health cannot go below 0

        if (isCriticalHit) // Damage Pop Up Here
        {
            Debug.Log($"💥 CRITICAL HIT! Enemy took {finalDamage} damage. Current Health: {m_enemyStats.e_currentHealth}");
        }
        else
        {
            Debug.Log($"Enemy took {finalDamage} damage. Current Health: {m_enemyStats.e_currentHealth}");
        }

        if (m_enemyStats.e_currentHealth <= 0f) // Death check
        {
            Debug.Log("Enemy is dead.");
        }
    }
}
