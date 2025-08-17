using System.Collections;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private PlayerAnimator m_playerAnimator;
    private PlayerCombat m_playerCombat;
    private PlayerWeapon m_playerWeapon;

    [Header("Normal Skill")]
    public float normalSkillCooldown = 3f;
    private float normalSkillTimer = 0f;
    public bool isUsingNormalSkill = false;
    [SerializeField] GameObject normalSkill;
    [SerializeField] float normalSkillLifetime;
    [SerializeField] float normalSkillSpeed;
    [SerializeField] private Transform normalSkillSpawnPoint;

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
        m_playerWeapon = GetComponentInChildren<PlayerWeapon>();
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
        if (!TutorialManager.instance.tutorial_canNormalSkill) return;

        isUsingNormalSkill = true;
        m_playerAnimator.ChangeAnimationState("player_normalSkill");
        normalSkillTimer = normalSkillCooldown;

        Transform enemy = m_playerCombat.GetNearestEnemy();
        m_playerCombat.FaceTarget(enemy);
    }


    void ActivateUltimateSkill()
    {
        if (!TutorialManager.instance.tutorial_canUltimateSkill) return;

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
    public void StartNormalSkill()
    {
        m_playerCombat.isAttacking = false;
        m_playerCombat.canMoveDuringAttack = true;
        m_playerCombat.ShowWeapon();
    }
    void SpawnNormalSkill()
    {
        Vector3 forwardDirection = normalSkillSpawnPoint.forward;
        Quaternion directionRotation = Quaternion.LookRotation(forwardDirection);
        Quaternion prefabOffset = Quaternion.Euler(45f, 90f, 0f);
        GameObject skillGO = Instantiate(normalSkill, normalSkillSpawnPoint.position, directionRotation * prefabOffset);

        PlayerNormalSkill skillScript = skillGO.GetComponent<PlayerNormalSkill>();
        if (skillScript != null)
        {
            skillScript.Initialize(normalSkillSpeed, normalSkillLifetime, forwardDirection);
        }
    }

    public void EndNormalSkill()
    {
        isUsingNormalSkill = false;
        m_playerCombat.HideWeapon();
        m_playerCombat.ShowWeaponParticle();
    }
    public void StartUltimateSkill()
    {
        m_playerCombat.isAttacking = false;
        m_playerCombat.canMoveDuringAttack = true;
        m_playerCombat.ShowWeapon();
        
        StartCoroutine(UltimateBlade());
    }

    IEnumerator UltimateBlade()
    {
        m_playerWeapon.SetToUltimateBlade();
        yield return new WaitForSeconds(0.1f);
        m_playerWeapon.UltimateBlade(400, 3.8f, 0.6f);
    }

    public void EndUltimateSkill()
    {
        m_playerWeapon.SetToDefaultBlade();
        isUsingUltimateSkill = false;
        m_playerCombat.HideWeapon();
        m_playerCombat.ShowWeaponParticle();
    }

    public void ForceStopSkills()
    {
        if (isUsingNormalSkill)
        {
            EndNormalSkill(); // your existing cleanup function
        }

        if (isUsingUltimateSkill)
        {
            EndUltimateSkill(); // your existing cleanup function
        }
    }

}
