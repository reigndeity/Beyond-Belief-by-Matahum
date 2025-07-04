using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyStats m_enemyStats;
    [SerializeField] Collider m_Collider;
    void Awake()
    {
        m_enemyStats = GetComponentInParent<EnemyStats>();
        m_Collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        if (other.gameObject.layer == enemyLayerMask) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            float damage = m_enemyStats.e_attack;
            damageable.TakeDamage(damage);
        }
    }

    public void EnableAttackCollider()
    {
        m_Collider.enabled = true;
    }
    public void DisableAttackCollider()
    {
        m_Collider.enabled = false;
    }

}
