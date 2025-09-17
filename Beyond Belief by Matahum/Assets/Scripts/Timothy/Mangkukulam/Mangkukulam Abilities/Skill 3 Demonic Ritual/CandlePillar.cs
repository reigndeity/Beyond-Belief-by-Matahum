using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using System.Collections;

public class CandlePillar : MonoBehaviour, IDamageable, IDeathHandler
{
    private PlayerStats playerStats;
    [HideInInspector] public EnemyStats enemyStats;
    public GameObject candlePulse;
    public float pillarHeight;
    public float risingDuration;
    public float candleHealth;
    public bool isDead = false;

    public event System.Action OnDeath;

    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();

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
        Vector3 offset = new Vector3(0, -1f, 0);
        CandlePulseExpansion candlePulseExpansion = Instantiate(candlePulse, transform.position + offset, Quaternion.identity, transform).GetComponent<CandlePulseExpansion>();
        candlePulseExpansion.enemyStats = enemyStats;
    }

    public bool IsDead() => candleHealth <= 0;

    public void TakeDamage(float damage, bool hitAnimOn = false)
    {
        candleHealth -= damage;

        PopUpDamage(damage);

        if (IsDead())
        {
            candleHealth = 0f;
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
