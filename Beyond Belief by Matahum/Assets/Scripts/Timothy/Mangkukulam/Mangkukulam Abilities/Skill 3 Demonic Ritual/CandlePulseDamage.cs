using UnityEngine;


public class CandlePulseDamage : MonoBehaviour
{
    [HideInInspector] public CandlePulseExpansion candlePulseExpansion;
    [HideInInspector] public float damage;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.CompareTag("Player"))
        {
            if (!candlePulseExpansion.canDamage) return;

            damageable.TakeDamage(damage,true);
            candlePulseExpansion.canDamage = false;
        }
    }
}
