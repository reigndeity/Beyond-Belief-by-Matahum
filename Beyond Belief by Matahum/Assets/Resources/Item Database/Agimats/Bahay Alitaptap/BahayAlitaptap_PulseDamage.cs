using UnityEngine;

public class BahayAlitaptap_PulseDamage : MonoBehaviour
{
    public float pulseDamage;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag != "Player")
        {
            float finalDamage = pulseDamage;

            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
            {
                finalDamage += stats.e_defense * 0.66f;
            }

            damageable.TakeDamage(finalDamage);
        }
    }
}
