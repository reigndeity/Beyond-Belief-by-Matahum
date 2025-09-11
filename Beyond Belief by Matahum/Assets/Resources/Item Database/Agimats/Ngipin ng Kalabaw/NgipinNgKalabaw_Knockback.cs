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
            damageable.TakeDamage(damage);

            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null) enemy.PushBackward(knockbackDistance);
        }
    }
}
