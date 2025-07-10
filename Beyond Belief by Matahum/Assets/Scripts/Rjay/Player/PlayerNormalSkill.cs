using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

public class PlayerNormalSkill : MonoBehaviour
{
    private Rigidbody m_rigidBody;
    private Collider m_collider;
    private PlayerStats m_playerStats;
    [Header("Normal Skill Damage Scaling")]
    public float normalSkillDamagePercent = 0.3f;

    [Header("Hit Cooldown")]
    private Dictionary<Collider, float> hitTimestamps = new Dictionary<Collider, float>();
    public float hitCooldown = 0.3f; // Time between valid hits to the same enemy
    [Header("Hit Impact")]
    public GameObject hitImpactPrefab;

    void Start()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
    }
    public void Initialize(float speed, float lifetime, Vector3 direction)
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody != null)
        {
            m_rigidBody.linearVelocity = direction.normalized * speed;
        }

        Destroy(gameObject, lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        // Check hit cooldown
        if (hitTimestamps.TryGetValue(other, out float lastHitTime))
        {
            if (Time.time - lastHitTime < hitCooldown)
            {
                return; // Recently hit, ignore
            }
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            float damage = m_playerStats.p_attack * normalSkillDamagePercent;
            float finalDamage = m_playerStats.p_attack + damage;
            damageable.TakeDamage(finalDamage);

            hitTimestamps[other] = Time.time; // Record this hit
            SpawnHitImpact(other.ClosestPoint(transform.position));
        }
    }
    private void SpawnHitImpact(Vector3 position)
    {
        if (hitImpactPrefab == null) return;

        GameObject vfx = Instantiate(hitImpactPrefab, position, Quaternion.identity);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(vfx, 1.5f); // fallback if no particle system
        }
    }
}
