using UnityEngine;

public class VoodooStats : MonoBehaviour
{
    private EnemyStats enemyStats;
    private PlayerStats playerStats;
    public float statMultiplier = 20f;
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        Initialize();
    }

    private void Initialize()
    {
        enemyStats.e_maxHealth = playerStats.p_maxHealth * (statMultiplier / 100);
        enemyStats.e_attack = playerStats.p_attack * (statMultiplier / 100);
        enemyStats.e_defense = playerStats.p_defense * (statMultiplier / 100);
        enemyStats.e_criticalRate = playerStats.p_critRate * 0.5f;
        enemyStats.e_criticalDamage = playerStats.p_criticalDamage * 0.5f;

        enemyStats.e_currentHealth = Mathf.Clamp(enemyStats.e_currentHealth <= 0 ? enemyStats.e_maxHealth : enemyStats.e_currentHealth, 0f, enemyStats.e_maxHealth);
    }
}
