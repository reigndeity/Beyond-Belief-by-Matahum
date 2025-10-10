using UnityEngine;
using System.Collections.Generic;

public class MutyaNgSampalok_VineDamage : MonoBehaviour
{
    public float damage;
    public GameObject poisonEffectObj;
    private HashSet<IDamageable> damagedEnemies = new HashSet<IDamageable>(); // faster lookup than List

    public VineCollider[] colliders;

    private void Start()
    {
        foreach (var vine in colliders)
        {
            vine.mainVine = this;
        }
    }
    public void HandleTrigger(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damagedEnemies.Add(damageable)) // only true the first time we add it
            {
                float finalDamage = damage;

                EnemyStats stats = other.GetComponent<EnemyStats>();
                if (stats != null)
                {
                    finalDamage += stats.e_defense * 0.66f;
                }

                damageable.TakeDamage(finalDamage);

                PoisonEffect poison = other.GetComponentInChildren<PoisonEffect>();
                if (poison == null)
                {
                    Vector3 offset = new Vector3(0, 1, 0);
                    poison = Instantiate(poisonEffectObj, other.transform.position + offset, Quaternion.identity, other.transform).GetComponent<PoisonEffect>();
                }

                // Refresh poison
                poison.RestartPoison();
                poison.Initialize(true);
            }
        }
    }

    void EnableColliders()
    {
        foreach (var vine in colliders)
        {
            vine.collider.enabled = true;
        }
    }

    void DisableColliders()
    {
        foreach (var vine in colliders)
        {
            vine.collider.enabled = false;
        }
    }
    void DestroyObject()
    {
        Destroy(transform.parent.gameObject);
    }
}
