using UnityEngine;
using BlazeAISpace;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    private Player m_player;
    private PlayerStats m_playerStats;
    private EnemyStats m_enemyStats;
    private BlazeAI m_blazeAI;
    private Rigidbody m_rigidbody;
    private EnemyUI m_enemyUI;

    void Awake()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_player = FindFirstObjectByType<Player>();
        m_enemyStats = GetComponent<EnemyStats>();
        m_blazeAI = GetComponent<BlazeAI>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_enemyUI = GetComponent<EnemyUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ShowCanvas();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            HideCanvas();
        }
    }
    public void TakeDamage(float damage)
    {
        GetHit();
        bool isCriticalHit = Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        float damageReduction = m_enemyStats.e_defense * 0.66f; // Defense Scaling
        float reducedDamage = damage - damageReduction; // Damage after Defense
        if (isCriticalHit)
        {
            reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f)); // Crit multiplier if critical hit
        }
        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1); // If defense is greater, cap the damage at 1

        m_enemyStats.e_currentHealth -= finalDamage; // Final Damage
        m_enemyStats.e_currentHealth = Mathf.Clamp(m_enemyStats.e_currentHealth, 0f, m_enemyStats.e_maxHealth); // Health cannot go below 0

        if (isCriticalHit) // Damage Pop Up Here
        {
            Debug.Log($"💥 CRITICAL HIT! Enemy took {finalDamage} damage. Current Health: {m_enemyStats.e_currentHealth}");
        }
        else
        {
            Debug.Log($"Enemy took {finalDamage} damage. Current Health: {m_enemyStats.e_currentHealth}");
        }

        if (m_enemyStats.e_currentHealth <= 0f) // Death check
        {
            Debug.Log("Enemy is dead.");
        }
    }
    public void GetHit()
    {
        m_blazeAI.Hit();
        PushBackward(0.5f);
        FindFirstObjectByType<PlayerCamera>().CameraShake(0.1f, 1f);
        HitStop.Instance.TriggerHitStop(0.05f);
        FacePlayer();
        m_enemyUI.HealthBarFadeIn(0.15f);
    }
    public void PushBackward(float distance)
    {
        StartCoroutine(SmoothPushRoutine(distance, 0.15f));
    }
    private IEnumerator SmoothPushRoutine(float totalDistance, float duration)
    {
        float elapsed = 0f;
        Vector3 direction = -transform.forward;
        direction.y = 0f;
        direction.Normalize();

        while (elapsed < duration)
        {
            float step = (totalDistance / duration) * Time.deltaTime;
            Vector3 push = direction * step;

            m_rigidbody.MovePosition(m_rigidbody.position + push);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    private void FacePlayer()
    {
        Vector3 dirToPlayer = m_player.transform.position - transform.position;
        dirToPlayer.y = 0f;

        if (dirToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer.normalized);
            transform.rotation = lookRotation;
        }
    }

    public void ShowCanvas()
    {
        StartCoroutine(Surprise());
    }
    public void HideCanvas()
    {
        m_enemyUI.AlertFadeOut(1.0f);
        m_enemyUI.HealthBarFadeOut(0.25f);
    }
    IEnumerator Surprise()
    {
        m_enemyUI.AlertFadeIn(0.25f);
        m_enemyUI.ShowAlert();
        yield return new WaitForSeconds(0.5f);
        m_enemyUI.AlertFadeOut(0.5f);
        yield return new WaitForSeconds(0.5f);
        m_enemyUI.HealthBarFadeIn(0.25f);
        //yield return new WaitForSeconds(1.833f);
        
    }
}
