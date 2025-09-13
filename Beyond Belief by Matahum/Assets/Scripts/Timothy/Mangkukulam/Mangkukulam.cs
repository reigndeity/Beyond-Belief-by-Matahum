using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mangkukulam : MonoBehaviour, IDamageable, IDeathHandler
{
    public static Mangkukulam instance;

    [Header("Mangkukulam State")]
    public bool isStunned;
    public bool isDead;
    public bool isVulnerable;
    public bool isBattleStart = false;

    [Header("References")]
    private EnemyStats stats;
    private PlayerStats m_playerStats;

    public event System.Action OnDeath; 

    void Awake()
    {
        instance = this;
    }

    #region DAMAGE AND DEATH
    public void TakeDamage(float damage, bool hitAnimOn)
    {
        if (isVulnerable && !isDead)
        {
            //GetHit();

            bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
            float damageReduction = stats.e_defense * 0.66f; // Defense Scaling
            float reducedDamage = damage - damageReduction;
            if (isCriticalHit)
                reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f));

            int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1);

            // NEW: centralize health changes in EnemyStats
            bool died = stats.ApplyDamage(finalDamage);

            Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
            if (isCriticalHit)
            {
                DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
                Debug.Log($"💥 CRITICAL HIT! Mangkukulam took {finalDamage} damage. Current Health: {stats.e_currentHealth}");
            }
            else
            {
                DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
                Debug.Log($"Mangkukulam took {finalDamage} damage. Current Health: {stats.e_currentHealth}");
            }

            if (died) Death();
        }
    }

    public bool IsDead()
    {
        throw new System.NotImplementedException();
    }

    public void Death()
    {
        OnDeath?.Invoke();
    }

    #endregion
}
