using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MutyaNgSampalok_VineDamage : MonoBehaviour
{
    public float damage;
    private HashSet<IDamageable> damagedEnemies = new HashSet<IDamageable>(); // faster lookup than List

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damagedEnemies.Add(damageable)) // only true the first time we add it
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
