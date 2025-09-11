using UnityEngine;

public class BahayAlitaptap_PulseDamage : MonoBehaviour
{
    public float pulseDamage;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag != "Player")
        {
            damageable.TakeDamage(pulseDamage);
        }
    }
}
