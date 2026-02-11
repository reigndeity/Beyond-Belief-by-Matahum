using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Mangkukulam : MonoBehaviour, IDamageable, IDeathHandler
{
    public static Mangkukulam instance;

    [Header("Mangkukulam State")]
    public bool isStunned;
    public bool isDead;
    public bool isVulnerable;
    public bool isBattleStart = false;
    public bool isMoving;
    public bool isFlying;

    public GameObject StunVFX;

    [Header("References")]
    private EnemyStats stats;
    private PlayerStats m_playerStats;
    private PlayerMovement playerMovement;
    private Mangkukulam_AnimationManager animator;
    [HideInInspector] public Boss_UICanvas uiCanvas;

    public event System.Action OnDeath;
    public event System.Action OnHit;

    [SerializeField] UI_CanvasGroup hpCanvas;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        animator = GetComponent<Mangkukulam_AnimationManager>();
        uiCanvas = GetComponent<Boss_UICanvas>();

        uiCanvas.currentCastingSkillTxt.text = " ";
    }

    private void Update()
    {
        if (isStunned)
        {
            uiCanvas.currentCastingSkillTxt.text = "Mangkukulam is stunned!";
            if(StunVFX != null) StunVFX.SetActive(true);
        }
        else if (StunVFX != null) StunVFX.SetActive(false);

    }

    public void BattleStart()
    {
        isBattleStart = true;
    }

    #region DAMAGE AND DEATH
    public void TakeDamage(float damage, bool hitAnimOn)
    {
        if (isVulnerable && !isDead)
        {
            GetHit();
            OnHit?.Invoke();

            bool isCriticalHit = Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
            float damageReduction = stats.e_defense * 0.66f; // Defense Scaling
            float reducedDamage = damage - damageReduction;
            if (isCriticalHit)
                reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f));

            int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1);

            // NEW: centralize health changes in EnemyStats
            bool died = stats.ApplyDamage(finalDamage);

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
    }
    private bool CanGetHitAnimation = true;
    public void GetHit()
    {
        FindFirstObjectByType<PlayerCamera>().CameraShake(0.1f, 1f);
        HitStop.Instance.TriggerHitStop(0.05f);

        if (isStunned && CanGetHitAnimation)
        {
            StartCoroutine(GetHitCooldown());
            animator.GetHit();
        }
    }
    IEnumerator GetHitCooldown()
    {
        CanGetHitAnimation = false;
        yield return new WaitForSeconds(1);
        CanGetHitAnimation = true;
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || isDead)
            return;

        float maxHP = stats.e_maxHealth;
        float oldHP = stats.e_currentHealth;

        stats.e_currentHealth = Mathf.Min(oldHP + amount, maxHP);

        int displayAmount = Mathf.Max(1, Mathf.FloorToInt(amount));

        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, displayAmount.ToString(), Color.green);
    }

    public void Death()
    {
        _ = Dying();
        isDead = true;
        OnDeath?.Invoke();
    }
    public bool IsDead() => isDead;
    private async Task Dying()
    {
        isDead = true;
        OnDeath?.Invoke();

        animator.ChangeAnimationState("Mangkukulam_Skill_3_Demonic Ritual");
        hpCanvas.FadeOut(1f);
        await Task.Delay(2500);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await Task.Delay(500);
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A2_Q9_FightMangkukulam");
        await Task.Delay(100);
        BB_QuestManager.Instance.ClaimRewardsByID("A2_Q9_DefeatMangkukulam");
        await Task.Delay(100);
        BB_QuestManager.Instance.AcceptQuestByID("A2_Q10_HowDoIgetHome");
        await Task.Delay(500);
        await GameManager.instance.SavePlayerCoreData();
        await Task.Delay(500);
        Loader.Load(15);
    }

    #endregion
}
