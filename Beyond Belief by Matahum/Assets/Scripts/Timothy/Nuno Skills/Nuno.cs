using UnityEngine;
using BlazeAISpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using FIMSpace.FProceduralAnimation;

public class Nuno : MonoBehaviour, IDamageable
{
    private Player m_player;
    private PlayerStats m_playerStats;
    private EnemyStats m_enemyStats;
    private Rigidbody m_rigidbody;

    public event System.Action OnDeath;

    void Awake()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_player = FindFirstObjectByType<Player>();
        m_enemyStats = GetComponent<EnemyStats>();
        m_rigidbody = GetComponent<Rigidbody>();
    }
    #region DAMAGE

    public void TakeDamage(float damage)
    {
        GetHit();

        bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        float damageReduction = m_enemyStats.e_defense * 0.66f; // Defense Scaling
        float reducedDamage = damage - damageReduction;
        if (isCriticalHit)
            reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f));

        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1);

        // NEW: centralize health changes in EnemyStats
        bool died = m_enemyStats.ApplyDamage(finalDamage);

        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        if (isCriticalHit)
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
            Debug.Log($"💥 CRITICAL HIT! Enemy took {finalDamage} damage. Current Health: {m_enemyStats.e_currentHealth}");
        }
        else
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
            Debug.Log($"Enemy took {finalDamage} damage. Current Health: {m_enemyStats.e_currentHealth}");
        }

        if (died) Death();
    }

    #endregion
    #region GET HIT
    public void GetHit()
    {
        FindFirstObjectByType<PlayerCamera>().CameraShake(0.1f, 1f);
        HitStop.Instance.TriggerHitStop(0.05f);
        FacePlayer();
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

    #endregion
    #region DEATH
    public void Death()
    {
        StartCoroutine(Dying());
    }
    public bool isDead = false;
    public bool IsDead() => isDead;
    IEnumerator Dying()
    {
        isDead = true;
        OnDeath.Invoke();
        yield return new WaitForSeconds(0.5f);
    }
    #endregion
}
