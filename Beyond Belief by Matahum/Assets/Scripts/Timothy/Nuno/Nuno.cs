using UnityEngine;
using BlazeAISpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using FIMSpace.FProceduralAnimation;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Nuno : MonoBehaviour, IDamageable, IDeathHandler
{
    private Player m_player;
    private PlayerStats m_playerStats;
    private EnemyStats nunoStats;
    private Nuno_Animations animator;
    private Rigidbody m_rigidbody;
    public bool isVulnerable = true;

    public event System.Action OnDeath;

    [SerializeField] UI_CanvasGroup hpCanvas;

    void Awake()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        m_player = FindFirstObjectByType<Player>();
        nunoStats = GetComponent<EnemyStats>();
        animator = GetComponent<Nuno_Animations>();
        m_rigidbody = GetComponent<Rigidbody>();
    }
    #region DAMAGE

    public void TakeDamage(float damage, bool hitAnimOn)
    {
        if (isVulnerable && !isDead)
        {
            GetHit();

            bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
            float damageReduction = nunoStats.e_defense * 0.66f; // Defense Scaling
            float reducedDamage = damage - damageReduction;
            if (isCriticalHit)
                reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f));

            int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1);

            // NEW: centralize health changes in EnemyStats
            bool died = nunoStats.ApplyDamage(finalDamage);

            Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
            if (isCriticalHit)
            {
                DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
                Debug.Log($"💥 CRITICAL HIT! Nuno took {finalDamage} damage. Current Health: {nunoStats.e_currentHealth}");
            }
            else
            {
                DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
                Debug.Log($"Nuno took {finalDamage} damage. Current Health: {nunoStats.e_currentHealth}");
            }

            if (died) Death();
        }      
    }

    #endregion
    #region GET HIT
    public void GetHit()
    {
        FindFirstObjectByType<PlayerCamera>().CameraShake(0.1f, 1f);
        HitStop.Instance.TriggerHitStop(0.05f);

        if(Nuno_AttackManager.Instance.isStunned) animator.GetHit();
    }

    #endregion
    #region DEATH
    public void Death()
    {
         _ = Dying();
    }
    public bool isDead = false;
    public bool IsDead() => isDead;
    private async Task Dying()
    {
        isDead = true;
        OnDeath?.Invoke();

        animator.ChangeAnimationState("Nuno_Death");
        hpCanvas.FadeOut(1f);
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q6_Nuno");
        await Task.Delay(2500);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        BB_QuestManager.Instance.ClaimRewardsByID("A1_Q6_NunoAnger");
        await Task.Delay(500);
        BB_QuestManager.Instance.AcceptQuestByID("A1_Q7_LessonFromNuno");
        await Task.Delay(500);
        await GameManager.instance.SavePlayerCoreData();
        await Task.Delay(500);
        Loader.Load(4);
    }
    #endregion
}
