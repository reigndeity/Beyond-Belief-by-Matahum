using UnityEngine;
using System.Collections;

public class NgipinNgKalabaw_Knockback : MonoBehaviour
{
    public float damage;
    public float knockbackDistance = 5f;   // adjustable distance
    public float knockbackDuration = 0.5f; // always 0.5s but adjustable if needed
    public bool canDamage;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && canDamage && other.gameObject.tag != "Player")
        {
            float finalDamage = damage;

            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
            {
                finalDamage += stats.e_defense * 0.66f;
            }

            damageable.TakeDamage(finalDamage);

            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null) enemy.PushBackward(knockbackDistance);
        }
    }
}
