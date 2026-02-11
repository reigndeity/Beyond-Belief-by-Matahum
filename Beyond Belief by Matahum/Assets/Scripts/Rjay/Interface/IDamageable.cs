using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, bool hitAnimOn = true);
    bool IsDead();
}
