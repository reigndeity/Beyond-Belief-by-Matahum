using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerMovement m_playerMovement;
    private PlayerSkills m_playerSkills;
    private PlayerStats m_playerStats;

    [Header("Health UI Properties")]
    [SerializeField] private Image healthFillDelayed;
    [SerializeField] private Image healthFillInstant;
    [SerializeField] private float healthFillDelaySpeed = 2f;
    [SerializeField] private TextMeshProUGUI healthAmountTxt;

    [Header("Stamina and Dash UI Properties")]
    [SerializeField] private Image staminaFillDelayed;
    [SerializeField] private Image staminaFillInstant;
    [SerializeField] private float staminaFillDelaySpeed = 2f;
    [SerializeField] private PlayerStaminaCanvas m_playerStaminaCanvas;
    private float lastStaminaUseTime;
    private bool isStaminaVisible = false;
    [SerializeField] private Image dashFill;

    [Header("Normal Skill UI Properties")]
    [SerializeField] private Image normalSkillBackground;
    [SerializeField] private Image normalSkillForeground;
    [SerializeField] private Image normalSkillCooldownOutlineFill;
    [SerializeField] private TextMeshProUGUI normalSkillCooldownTxt;

    [Header("Ultimate Skill UI Properties")]
    [SerializeField] private Image ultimateSkillBackground;
    [SerializeField] private Image ultimateSkillForeground;
    [SerializeField] private Image ultimateSkillCooldownOutlineFill;
    [SerializeField] private TextMeshProUGUI ultimateSkillCooldownTxt;

    void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerSkills = GetComponent<PlayerSkills>();
        m_playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        UIStaminaUpdate();
        UIDashCooldownUpdate();
        UINormalSkillCooldownUpdate();
        UIUltimateSkillCooldownUpdate();
        UIHealthUpdate();
    }

    void UIStaminaUpdate()
    {
        float current = m_playerMovement.currentStamina;
        float max = m_playerMovement.maxStamina;
        float percent = current / max;

        staminaFillInstant.fillAmount = percent;

        if (staminaFillDelayed.fillAmount > percent)
        {
            staminaFillDelayed.fillAmount = Mathf.Lerp(staminaFillDelayed.fillAmount, percent, Time.deltaTime * staminaFillDelaySpeed);
        }
        else
        {
            staminaFillDelayed.fillAmount = percent;
        }

        if (current < max)
        {
            lastStaminaUseTime = Time.time;

            if (!isStaminaVisible)
            {
                m_playerStaminaCanvas.FadeIn(0.5f);
                isStaminaVisible = true;
            }
        }
        else
        {
            if (isStaminaVisible && Time.time - lastStaminaUseTime >= 1)
            {
                m_playerStaminaCanvas.FadeOut(0.5f);
                isStaminaVisible = false;
            }
        }
    }

    void UIDashCooldownUpdate()
    {
        float dashPercent = 1f - (m_playerMovement.GetDashCooldownRemaining() / m_playerMovement.dashCooldown);
        dashFill.fillAmount = dashPercent;
    }

    void UINormalSkillCooldownUpdate()
    {
        float remaining = m_playerSkills.GetNormalSkillCooldownRemaining();
        float total = m_playerSkills.GetNormalSkillCooldown();

        float fill = remaining / total; // Now drains 1 -> 0
        normalSkillCooldownOutlineFill.fillAmount = fill;

        Color bgAlpha = normalSkillBackground.color;
        Color fgAlpha = normalSkillForeground.color;

        if (fill <= 0f)
        {
            normalSkillCooldownOutlineFill.enabled = false;
            normalSkillCooldownTxt.enabled = false;

            bgAlpha.a = 0.5f;
            fgAlpha.a = 0.5f;
            normalSkillBackground.color = bgAlpha;
            normalSkillForeground.color = fgAlpha;
        }
        else
        {
            normalSkillCooldownOutlineFill.enabled = true;
            normalSkillCooldownTxt.enabled = true;
            normalSkillCooldownTxt.text = remaining.ToString("F1");

            bgAlpha.a = 0.25f;
            fgAlpha.a = 0.25f;
            normalSkillBackground.color = bgAlpha;
            normalSkillForeground.color = fgAlpha;
        }
    }

    void UIUltimateSkillCooldownUpdate()
    {
        float remaining = m_playerSkills.GetUltimateSkillCooldownRemaining();
        float total = m_playerSkills.GetUltimateSkillCooldown();

        float fill = remaining / total; // Now drains 1 -> 0
        ultimateSkillCooldownOutlineFill.fillAmount = fill;

        Color bgAlpha = ultimateSkillBackground.color;
        Color fgAlpha = ultimateSkillForeground.color;

        if (fill <= 0f)
        {
            ultimateSkillCooldownOutlineFill.enabled = false;
            ultimateSkillCooldownTxt.enabled = false;
            
            bgAlpha.a = 0.5f;
            fgAlpha.a = 0.5f;
            ultimateSkillBackground.color = bgAlpha;
            ultimateSkillForeground.color = fgAlpha;
        }
        else
        {
            ultimateSkillCooldownOutlineFill.enabled = true;
            ultimateSkillCooldownTxt.enabled = true;
            ultimateSkillCooldownTxt.text = remaining.ToString("F1");

            bgAlpha.a = 0.25f;
            fgAlpha.a = 0.25f;
            ultimateSkillBackground.color = bgAlpha;
            ultimateSkillForeground.color = fgAlpha;
        }
    }

    void UIHealthUpdate()
    {
        float current = m_playerStats.p_currentHealth;
        float max = m_playerStats.p_maxHealth;

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

        healthAmountTxt.text = $"{current:F0} / {max:F0}";
    }
}
