using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PoisonEffect : MonoBehaviour
{
    public float poisonToDamagePercentage = 1f; // % of max HP per tick
    public float poisonDuration = 5f;
    public float tickRate = 1f; // seconds per tick

    private float elapsed;
    private float lastRefreshTime;
    private Coroutine poisonRoutine;

    private IDamageable damageable;
    private PlayerStats playerStats;

    void Awake()
    {
        damageable = GetComponent<IDamageable>();
        playerStats = GetComponent<PlayerStats>();
    }

    public void RestartPoison()
    {
        lastRefreshTime = Time.time; // refresh poison duration

        if (poisonRoutine == null) // start only once
            poisonRoutine = StartCoroutine(Poison());
    }

    private IEnumerator Poison()
    {
        while (true)
        {
            // still within duration? (refreshed if we call RestartPoison)
            if (Time.time - lastRefreshTime <= poisonDuration)
            {
                float damageReduction = playerStats.p_defense * 0.66f;
                float damage = damageReduction + (playerStats.p_maxHealth * (poisonToDamagePercentage / 100f));
                damageable.TakeDamage(damage, false);
            }
            else
            {
                break; // duration ended
            }
            yield return new WaitForSeconds(tickRate);
        }

        Destroy(this); // remove poison effect after it expires
    }
}
