using UnityEngine;

public class RockShield_Shield : MonoBehaviour, IDamageable
{
    public float shieldHealth;
    [HideInInspector] public RockShield_ShieldHolder shieldHolder;

    public bool IsDead() => shieldHealth <= 0;

    public void Init(RockShield_ShieldHolder holder, float health)
    {
        shieldHolder = holder;
        shieldHealth = health;
        gameObject.SetActive(true);
    }

    public void TakeDamage(float damage)
    {
        shieldHealth -= damage;

        if (IsDead())
        {
            gameObject.SetActive(false);
            shieldHealth = 0f;
            shieldHolder.OnShieldDestroyed();
        }
    }
}
