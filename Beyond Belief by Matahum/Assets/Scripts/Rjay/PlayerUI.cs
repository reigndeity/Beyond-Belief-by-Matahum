using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerMovement m_playerMovement;

    [Header("UI Properties")]
    [SerializeField] private Image staminaFill;
    [SerializeField] private Image dashFill;

    void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        UIStaminaUpdate();
        UIDashCooldownUpdate();
    }

    void UIStaminaUpdate()
    {
        float percent = m_playerMovement.currentStamina / m_playerMovement.maxStamina;
        staminaFill.fillAmount = percent;
    }
    void UIDashCooldownUpdate()
    {
        float dashPercent = 1f - (m_playerMovement.GetDashCooldownRemaining() / m_playerMovement.dashCooldown);
        dashFill.fillAmount = dashPercent;
    }
}
