using UnityEngine;

public class RockShield_Shield : MonoBehaviour, IDamageable
{
    public float shieldHealth;
    [HideInInspector] public RockShield_ShieldHolder shieldHolder;
    private FractureObject vfx;

    private PlayerStats m_playerStats;

    void Start()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        vfx = GetComponentInParent<FractureObject>();
    }
    public bool IsDead() => shieldHealth <= 0;

    public void Init(RockShield_ShieldHolder holder, float health)
    {
        gameObject.SetActive(true);
        shieldHolder = holder;
        shieldHealth = health;
        
    }

    public void TakeDamage(float damage)
    {
        shieldHealth -= damage;

        PopUpDamage(damage);

        if (IsDead())
        {
            vfx.Explode();
            shieldHealth = 0f;
            shieldHolder.OnShieldDestroyed();
        }
    }

    private void PopUpDamage(float damage)
    {
        bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
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
}
