using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using System.Collections;

public class CandlePillar : MonoBehaviour, IDamageable, IDeathHandler
{
    private PlayerStats playerStats;
    [HideInInspector] public EnemyStats enemyStats;
    [HideInInspector] public EnemyStats candleStats;
    public GameObject candlePulse;
    public float candleHealth;
    public float pillarHeight;
    public float risingDuration;
    public bool isDead = false;

    public event System.Action OnDeath;

    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        candleStats = GetComponent<EnemyStats>();

        candleStats.e_maxHealth = candleHealth;
        candleStats.RecalculateStats();

        StartCoroutine(StartRising());
    }
    public IEnumerator StartRising()
    {
        yield return new WaitForSeconds(1f);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * pillarHeight;

        float elapsed = 0f;
        // Scale in
        while (elapsed < risingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / risingDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    public void SpawnPulse()
    {
        CandlePulseExpansion candlePulseExpansion = GetComponentInChildren<CandlePulseExpansion>();
        if (candlePulseExpansion == null)
        {
            Vector3 offset = new Vector3(0, -1f, 0);
            candlePulseExpansion = Instantiate(candlePulse, transform.position + offset, Quaternion.identity, transform).GetComponent<CandlePulseExpansion>();
            candlePulseExpansion.enemyStats = enemyStats;
            candlePulseExpansion.PrewarmPool();
        }

        candlePulseExpansion.StartExpanding();
    }

    public bool IsDead() => candleStats.e_currentHealth <= 0;

    public void TakeDamage(float damage, bool hitAnimOn = false)
    {
        candleStats.e_currentHealth -= damage;

        PopUpDamage(damage);

        if (IsDead())
        {
            candleStats.e_currentHealth = 0f;
            Death();
        }
    }

    private void PopUpDamage(float damage)
    {
        bool isCriticalHit = UnityEngine.Random.value <= (playerStats.p_criticalRate / 100f); // Crit Check
        int finalDamage = Mathf.Max(Mathf.FloorToInt(damage), 1);
        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        if (isCriticalHit)
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
        }
        else
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
        }
    }

    public void Death()
    {
        isDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
