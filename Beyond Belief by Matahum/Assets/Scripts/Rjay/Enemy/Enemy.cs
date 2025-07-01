using UnityEngine;
using BlazeAISpace;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    private Player m_player;
    private PlayerStats m_playerStats;
    private EnemyStats m_enemyStats;
    private BlazeAI m_blazeAI;
    private Rigidbody m_rigidbody;
    void Start()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_player = FindFirstObjectByType<Player>();
        m_enemyStats = GetComponent<EnemyStats>();
        m_blazeAI = GetComponent<BlazeAI>();
        m_rigidbody = GetComponent<Rigidbody>();
    }
    public void TakeDamage(float damage)
    {
        GetHit();
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

    public void GetHit()
    {
        m_blazeAI.Hit();
        FacePlayer();
    }
    public void PushBackward(float distance)
    {
        StartCoroutine(SmoothPushRoutine(distance, 0.15f));
    }
    private IEnumerator SmoothPushRoutine(float totalDistance, float duration)
    {
        float elapsed = 0f;
        Vector3 direction = -transform.forward;
        direction.y = 0f;
        direction.Normalize();

        while (elapsed < duration)
        {
            float step = (totalDistance / duration) * Time.deltaTime;
            Vector3 push = direction * step;

            m_rigidbody.MovePosition(m_rigidbody.position + push);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    private void FacePlayer()
    {
        Vector3 dirToPlayer = m_player.transform.position - transform.position;
        dirToPlayer.y = 0f;

        if (dirToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer.normalized);
            transform.rotation = lookRotation;
        }
    }
}
