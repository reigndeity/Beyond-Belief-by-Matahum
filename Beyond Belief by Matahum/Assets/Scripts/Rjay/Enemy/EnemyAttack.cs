using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyStats m_enemyStats;
    [SerializeField] Collider m_Collider;
    private int enemyLayer; // layer index

    void Awake()
    {
        m_enemyStats = GetComponentInParent<EnemyStats>();
        m_Collider = GetComponent<Collider>();
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Collider.enabled) return;

        // FIX: compare layer index, not bitmask; also don't hit self
        if (other.gameObject.layer == enemyLayer) return;
        if (other.transform.root == transform.root) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(m_enemyStats.e_attack);
    }

    public void EnableAttackCollider()  => m_Collider.enabled = true;
    public void DisableAttackCollider() => m_Collider.enabled = false;
}
