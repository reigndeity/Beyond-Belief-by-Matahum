using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private PlayerAnimator m_playerAnimator;
    private PlayerCombat m_playerCombat;

    [Header("Normal Skill")]
    public float normalSkillCooldown = 3f;
    private float normalSkillTimer = 0f;
    public bool isUsingNormalSkill = false;

    [Header("Ultimate Skill")]
    [SerializeField] Animator weaponAnimator;
    public float ultimateSkillCooldown = 10f;
    private float ultimateSkillTimer = 0f;
    public bool isUsingUltimateSkill = false;

    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerCombat = GetComponent<PlayerCombat>();
    }

    public void HandleSkills()
    {
        if (normalSkillTimer > 0f) normalSkillTimer -= Time.deltaTime;
        if (ultimateSkillTimer > 0f) ultimateSkillTimer -= Time.deltaTime;

        // Prevent using skills mid-air
        bool isGrounded = GetComponent<CharacterController>().isGrounded;
        if (!isGrounded) return;

        if (!isUsingUltimateSkill && !isUsingNormalSkill)
        {
            if (Input.GetKeyDown(m_playerInput.normalSkillKey) && normalSkillTimer <= 0f)
            {
                ActivateNormalSkill();
            }

            if (Input.GetKeyDown(m_playerInput.ultimateSkillKey) && ultimateSkillTimer <= 0f)
            {
                ActivateUltimateSkill();
            }
        }
    }


    void ActivateNormalSkill()
    {
        isUsingNormalSkill = true;
        m_playerAnimator.ChangeAnimationState("player_normalSkill");
        normalSkillTimer = normalSkillCooldown;
    }

    void ActivateUltimateSkill()
    {
        isUsingUltimateSkill = true;
        m_playerAnimator.ChangeAnimationState("player_ultimateSkill");
        weaponAnimator.SetTrigger("isUltimate");
        ultimateSkillTimer = ultimateSkillCooldown;
    }
    public float GetNormalSkillCooldownRemaining() => normalSkillTimer;
    public float GetUltimateSkillCooldownRemaining() => ultimateSkillTimer;

    public float GetNormalSkillCooldown() => normalSkillCooldown;
    public float GetUltimateSkillCooldown() => ultimateSkillCooldown;

    // ANIMATOR EVENT SYSTEM
    public void EndNormalSkill()
    {
        isUsingNormalSkill = false;
    }

    public void EndUltimateSkill()
    {
        isUsingUltimateSkill = false;
    }

}
