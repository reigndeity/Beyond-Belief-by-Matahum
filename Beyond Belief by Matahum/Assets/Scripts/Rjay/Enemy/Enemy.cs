using UnityEngine;
using BlazeAISpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using FIMSpace.FProceduralAnimation;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour, IDamageable, IDeathHandler
{
    private Player m_player;
    private PlayerStats m_playerStats;
    private EnemyStats m_enemyStats;
    private BlazeAI m_blazeAI;
    private Rigidbody m_rigidbody;
    private EnemyUI m_enemyUI;
    private NavMeshAgent m_navMeshAgent;
    private LegsAnimator m_legsAnimator;
    private EnemyAttack m_enemyAttack;
    public CharacterAudios charAudios;
    public int charAudioIndex;
    public event System.Action OnDeath;
    private bool isChasingPlayer = false;

    void Awake()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_player = FindFirstObjectByType<Player>();
        m_enemyStats = GetComponent<EnemyStats>();
        m_blazeAI = GetComponent<BlazeAI>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_enemyUI = GetComponent<EnemyUI>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_legsAnimator = GetComponent<LegsAnimator>();
        m_enemyAttack = GetComponentInChildren<EnemyAttack>();
    }

    void Start()
    {
        DisableAllRagdollParts();
        if (charAudios == null)
            charAudios = GetComponent<CharacterAudios>();

        if (m_blazeAI != null && m_blazeAI.vision != null)
        {
            // these UnityEvents are built-in inside the BlazeAI.vision system
            m_blazeAI.vision.enemyEnterEvent.AddListener(OnEnemyDetected);
            m_blazeAI.vision.enemyLeaveEvent.AddListener(OnEnemyLost);
        }
    }
    #region DAMAGE

    public void TakeDamage(float damage, bool hitAnimOn)
    {
        if (hitAnimOn)
            GetHit();

        bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        float damageReduction = m_enemyStats.e_defense * 0.66f; // Defense Scaling
        float reducedDamage = damage - damageReduction;
        if (isCriticalHit)
            reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f));

        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1);

        // NEW: centralize health changes in EnemyStats
        bool died = m_enemyStats.ApplyDamage(finalDamage);

        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        if (isCriticalHit)
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
        }
        else
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
        }

        if (died) Death();
    }

    #endregion
    #region GET HIT
    public void GetHit()
    {
        m_navMeshAgent.enabled = false;
        m_blazeAI.Hit();
        PushBackward(1f);
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
        m_navMeshAgent.enabled = true;
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

    #endregion
    #region ALERT BEHAVIOR
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
    #endregion
    #region DEATH
    void IgnorePlayerLayer()
    {
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject != gameObject) // skip main root
                rb.excludeLayers = playerLayerMask;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col.gameObject != gameObject) // skip main capsule
                col.excludeLayers = playerLayerMask;
        }
    }
    void DisableAllRagdollParts()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject != gameObject) // skip main root
                rb.isKinematic = true;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col.gameObject != gameObject) // skip main capsule
                col.enabled = false;
        }
    }
    void EnableAllRagdollParts()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject != gameObject) // skip main root
                rb.isKinematic = false;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col.gameObject != gameObject) // skip main capsule
                col.enabled = true;
        }
    }
    public void Death()
    {
        StartCoroutine(Dying());
    }
    public bool isDead = false;
    public bool IsDead() => isDead;
    IEnumerator Dying()
    {
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        HideCanvas();
        m_legsAnimator.enabled = false;
        EnableAllRagdollParts();
        m_blazeAI.Death();
        IgnorePlayerLayer();
        charAudios.SFX(charAudioIndex);
        OnDeath?.Invoke();
        yield return new WaitForSeconds(0.5f);
    }
    #endregion

    public void EnableAttack() => m_enemyAttack.EnableAttackCollider();
    public void DisableAttack() => m_enemyAttack.DisableAttackCollider();
    #region DANGER MUSIC HANDLER
    private void OnEnemyDetected()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName != "OpenWorldScene") 
            return; // ignore other scenes

        if (isDead) return;
        if (isChasingPlayer) return;

        isChasingPlayer = true;
        m_player.EnterDanger();
    }

    private void OnEnemyLost()
    {
        if (!isChasingPlayer) return;

        isChasingPlayer = false;
        m_player.ExitDanger();
    }

    private void OnDestroy()
    {
        if (m_blazeAI != null && m_blazeAI.vision != null)
        {
            m_blazeAI.vision.enemyEnterEvent.RemoveListener(OnEnemyDetected);
            m_blazeAI.vision.enemyLeaveEvent.RemoveListener(OnEnemyLost);
        }

        if (m_player != null && isChasingPlayer)
            m_player.ExitDanger();
    }

    #endregion

}
