using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Haligi : MonoBehaviour, IDamageable
{
    [Header("Health & Defense")]
    [SerializeField] private float maxHealth = 1000;
    [SerializeField] private float currentHealth = 10f;
    [SerializeField] private float defense = 50f;                  // Defense value (scaled below)

    [Header("Critical")]
    [Range(0f, 100f)] public float criticalRate = 0f;             // In percent
    [Range(0f, 1000f)] public float criticalDamage = 50f;         // Extra percent (e.g., 50 => 1.5x)

    [Header("Flags")]
    public bool isInvulnerable = false;
    private bool isDead = false;

    [Header("FX")]
    [SerializeField] private Animator animator;                    // Optional; can be null

    [Header("Health UI")]
    [SerializeField] private Image healthFillDelayed;
    [SerializeField] private Image healthFillInstant;
    [SerializeField] private float healthFillDelaySpeed = 2f;

    [Header("Quest Mission")]
    public string missionID;
    public GameObject fragment;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth <= 0 ? maxHealth : currentHealth, 0f, maxHealth);
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(float damage, bool hitAnimOn = true)
    {
        if (isInvulnerable || isDead) return;

        // Crit check
        bool isCriticalHit = Random.value <= (criticalRate / 100f);

        // Defense scaling (same 0.66 factor you used)
        float damageReduction = defense * 0.66f;
        float reducedDamage = damage - damageReduction;

        if (isCriticalHit)
        {
            // criticalDamage 50 => +50% => x1.5
            reducedDamage *= (1f + (criticalDamage / 100f));
        }

        // Ensure at least 1 damage after reductions and floor to int for clean UI display
        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1);

        // Apply and clamp health
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Random small offset for popup so multiple hits don’t overlap perfectly
        Vector3 popUpRandomness = new Vector3(
            Random.Range(0f, 0.25f),
            Random.Range(0f, 0.25f),
            Random.Range(0f, 0.25f)
        );

        string dmgText = finalDamage.ToString();

        if (isCriticalHit)
        {
            DamagePopUpGenerator.instance?.CreatePopUp(transform.position + popUpRandomness, dmgText, Color.red);
            Debug.Log($"💥 CRIT on Haligi! Took {finalDamage}. HP: {currentHealth}/{maxHealth}");
        }
        else
        {
            DamagePopUpGenerator.instance?.CreatePopUp(transform.position + popUpRandomness, dmgText, Color.white);
            Debug.Log($"Haligi took {finalDamage}. HP: {currentHealth}/{maxHealth}");
        }

        // Death
        if (currentHealth <= 0f && !isDead)
        {
            HandleDeath();
            return;
        }

        // Optional hit anim
        if (hitAnimOn && animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        fragment.SetActive(true);
        Destroy(gameObject);
    }

    private void UpdateHealthUI()
    {
        float current = currentHealth;
        float max = maxHealth;

        float targetFill = current / max;

        healthFillInstant.fillAmount = targetFill;

        if (healthFillDelayed.fillAmount > targetFill)
        {
            healthFillDelayed.fillAmount = Mathf.Lerp(healthFillDelayed.fillAmount, targetFill, Time.deltaTime * healthFillDelaySpeed);
        }
        else
        {
            healthFillDelayed.fillAmount = targetFill;
        }
    }
}