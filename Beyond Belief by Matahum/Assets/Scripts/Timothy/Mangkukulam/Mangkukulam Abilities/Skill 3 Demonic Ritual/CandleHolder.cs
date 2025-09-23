using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CandleHolder : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public Mangkukulam mangkukulam;
    [HideInInspector] public EnemyStats stats;

    [HideInInspector] public float candleHealth;
    [HideInInspector] public float candleToHealthRatio;

    [Header("Candle Properties")]
    public int candleCount = 4;
    public float radius = 10f;
    public GameObject candlePrefab;
    public List<CandlePillar> candles = new List<CandlePillar>();
    public float spawnInterval;

    [Header("Healing / Damage Phases")]
    public float healPercentPerCandle = 1f; // heal % per candle per second
    public bool isHealing = true;
    public GameObject damageAuraVFX;
    public GameObject healingAuraVFX;

    private Coroutine phaseRoutine;

    public void Initialize(Mangkukulam mangkukulam, EnemyStats stats, float candleToHealthRatio)
    {
        this.mangkukulam = mangkukulam;
        this.stats = stats;
        this.candleToHealthRatio = candleToHealthRatio;

        damageAuraVFX.SetActive(false);
        healingAuraVFX.SetActive(false);

        candleHealth = stats.e_maxHealth * (candleToHealthRatio / 100);
    }

    public void SpawnCandles()
    {
        float delay = 0;
        for (int i = 0; i < candleCount; i++)
        {
            delay += spawnInterval;
            StartCoroutine(DelaySpawn(delay, i));
        }

        // Start phase loop
        phaseRoutine = StartCoroutine(PhaseLoop(delay + 1));
    }

    IEnumerator DelaySpawn(float delay, int index)
    {
        yield return new WaitForSeconds(delay);

        float angle = index * Mathf.PI * 2f / candleCount;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 basePos = transform.position + offset;

        if (NavMesh.SamplePosition(basePos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            Vector3 navMeshOffset = new Vector3(0, 0.11f, 0);
            CandlePillar candleObj = Instantiate(
                candlePrefab,
                hit.position - navMeshOffset,
                transform.parent.rotation,
                transform
            ).GetComponent<CandlePillar>();

            candles.Add(candleObj);
            candleObj.enemyStats = stats;
            candleObj.candleHealth = candleHealth;
            candleObj.OnDeath += () => OnCandleDestroyed(candleObj);
        }
    }

    private IEnumerator PhaseLoop(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            // --- Healing phase (5s) ---
            isHealing = true;

            damageAuraVFX.SetActive(!isHealing);
            healingAuraVFX.SetActive(isHealing);

            float healDuration = 5f;
            float healTimer = 0f;

            while (healTimer < healDuration && candles.Count > 0)
            {
                HealOncePerSecond();
                yield return new WaitForSeconds(1f);
                healTimer += 1f;
            }

            // --- Damage phase (10s) ---
            isHealing = false;

            damageAuraVFX.SetActive(!isHealing);
            healingAuraVFX.SetActive(isHealing);

            foreach (var candle in candles)
            {
                candle.SpawnPulse(); // activate candle attack
            }

            yield return new WaitForSeconds(10f);
        }
    }

    private void HealOncePerSecond()
    {
        int candleCountAlive = candles.Count;
        if (candleCountAlive > 0 && mangkukulam != null && stats != null)
        {
            float healAmount = stats.e_maxHealth * (healPercentPerCandle / 100f) * candleCountAlive;
            mangkukulam.Heal(healAmount);
        }
    }

    public void OnCandleDestroyed(CandlePillar candlePillar)
    {
        candles.Remove(candlePillar);

        if (IsAllCandlesDestroyed())
        {
            if (phaseRoutine != null)
                StopCoroutine(phaseRoutine);

            Destroy(gameObject);
        }
    }

    public bool IsAllCandlesDestroyed()
    {
        return candles.Count == 0;
    }

    private void OnDestroy()
    {
        if (phaseRoutine != null)
            StopCoroutine(phaseRoutine);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (candleCount > 0)
        {
            for (int i = 0; i < candleCount; i++)
            {
                float angle = i * Mathf.PI * 2f / candleCount;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                Gizmos.DrawSphere(transform.position + pos, 0.1f);
            }
        }
    }
}
