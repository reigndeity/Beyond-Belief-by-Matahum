using UnityEngine;

public class PotionBottle : MonoBehaviour
{
    public float bulletDamage;
    public GameObject poisonArea;
    public GameObject explosionVFX;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.TakeDamage(bulletDamage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Vector3 offset = new Vector3(0, 0.1f, 0);
            Instantiate(poisonArea, transform.position + offset, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
