using UnityEngine;
using System.Collections;

public class PoisonEffect : MonoBehaviour
{
    public float poisonToDamagePercentage = 1f; // % of max HP per tick
    public float poisonDuration = 5f;
    public float tickRate = 1f; // seconds per tick
    public bool fromPlayer; // true = applied on player, false = applied on enemy

    private float lastRefreshTime;
    private Coroutine poisonRoutine;

    private IDamageable damageable;
    private PlayerStats playerStats;
    private EnemyStats enemyStats;

    public void Initialize(bool isPlayerTarget)
    {
        fromPlayer = isPlayerTarget;

        if (!fromPlayer)
        {
            playerStats = GetComponentInParent<PlayerStats>();
            damageable = playerStats?.GetComponent<IDamageable>();
        }
        else if(fromPlayer)
        {
            enemyStats = GetComponentInParent<EnemyStats>();
            damageable = enemyStats?.GetComponent<IDamageable>();
        }
    }

    public void RestartPoison()
    {
        lastRefreshTime = Time.time; // refresh poison duration

        if (poisonRoutine == null) // start only once
            poisonRoutine = StartCoroutine(Poison());
    }

    private IEnumerator Poison()
    {
        while (Time.time - lastRefreshTime <= poisonDuration)
        {
            if (damageable != null)
            {
                float dmg = CalculatePoisonDamage();
                damageable.TakeDamage(dmg, false);
            }

            yield return new WaitForSeconds(tickRate);
        }

        // OPTIONAL: fade out poison VFX only, not the whole character
        // Example: if this script is on a poison effect prefab
        float t = 0;
        Vector3 startScale = transform.localScale;
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject); // destroy poison effect object (not the player/enemy)
    }

    private float CalculatePoisonDamage()
    {
        float damage = 0f;

        if (!fromPlayer && playerStats != null)
        {
            float baseDamage = playerStats.p_maxHealth * (poisonToDamagePercentage / 100f);
            float reduction = playerStats.p_defense * 0.66f;
            damage = Mathf.Max(0, baseDamage + reduction);
        }
        else if (fromPlayer && enemyStats != null)
        {
            float baseDamage = enemyStats.e_maxHealth * (poisonToDamagePercentage / 100f);
            float reduction = enemyStats.e_defense * 0.66f;
            damage = Mathf.Max(0, baseDamage + reduction);
        }

        return damage;
    }
}
