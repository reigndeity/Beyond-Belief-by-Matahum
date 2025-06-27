using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private PlayerStats m_playerStats;
    public Collider weaponCollider;
    public float m_scalingAmount;

    void Start()
    {
        m_playerStats = GetComponentInParent<PlayerStats>();
        weaponCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            FindFirstObjectByType<PlayerCamera>().CameraShake(0.1f, 1f);

            float damage = m_playerStats.p_attack * m_scalingAmount;
            Debug.Log(m_scalingAmount);
            damageable.TakeDamage(damage);
        }
    }
}
