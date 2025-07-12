using System.Collections;
using UnityEngine;
public enum PlayerState 
{
    IDLE,
    MOVING,
    JUMPING,
    FALLING,
    LANDING,
    COMBAT,
    DASHING
}
public class Player : MonoBehaviour, IDamageable
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;
    private PlayerCombat m_playerCombat;
    private PlayerSkills m_playerSkills;
    private PlayerStats m_playerStats;
    public PlayerState currentState;
    private PlayerMinimap m_playerMinimap;
    private PlayerCamera m_playerCamera;


    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
        m_playerCombat = GetComponent<PlayerCombat>();
        m_playerSkills = GetComponent<PlayerSkills>();
        m_playerStats = GetComponent<PlayerStats>();
        m_playerMinimap = GetComponentInChildren<PlayerMinimap>();
        m_playerCamera = FindFirstObjectByType<PlayerCamera>();
    }

    void Update()
    {   

        // Player Camera
        m_playerCamera.HandleMouseLock();
        if (Cursor.lockState == CursorLockMode.Locked)
            m_playerCamera.HandleRotation();
        if (!m_playerMinimap.IsMapOpen())
        {
            m_playerCamera.HandleZoom();
            m_playerMinimap.projectedViewIcon.enabled = true; // show when map is closed
        }
        else
        {
            m_playerMinimap.projectedViewIcon.enabled = false; // hide when map is open
        }

        // Map
        m_playerMinimap.ProjectionRotation();
        m_playerMinimap.HandleMapToggle();
        m_playerMinimap.ZoomControl();

        if (!m_playerMinimap.IsMapOpen())
        {
            m_playerMovement.HandleMovement();
            m_playerMovement.HandleDash();
            m_playerMovement.HandleJump();
            m_playerCombat.HandleAttack();
            m_playerSkills.HandleSkills();
            HandleGrassInteraction();
        }

        float currentSpeed = m_playerMovement.Speed;
        m_playerAnimator.lastFrameSpeed = currentSpeed;
        m_playerAnimator.HandleAnimations();

        // --- Determine current player state ---
        if (m_playerMovement.IsDashing())
        {
            currentState = PlayerState.DASHING;
        }
        else if (m_playerMovement.IsJumping())
        {
            currentState = PlayerState.JUMPING;
        }
        else if (m_playerMovement.GetVerticalVelocity() < -1f && !m_playerMovement.GetComponent<CharacterController>().isGrounded)
        {
            currentState = PlayerState.FALLING;
        }
        else if (m_playerMovement.JustLanded())
        {
            currentState = PlayerState.LANDING;
        }
        else if (m_playerCombat.IsAttacking())
        {
            currentState = PlayerState.COMBAT;
        }
        else if (currentSpeed > 0.1f)
        {
            currentState = PlayerState.MOVING;
        }
        else
        {
            currentState = PlayerState.IDLE;
        }
    }


    public void TakeDamage(float damage)
    {
        m_playerAnimator.GetHit();
        bool isCriticalHit = Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        float damageReduction = m_playerStats.p_defense * 0.66f; // Defense Scaling
        float reducedDamage = damage - damageReduction; // Damage after Defense
        if (isCriticalHit)
        {
            reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f)); // Crit multiplier if critical hit
        }
        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1); // If defense is greater, cap the damage at 1

        m_playerStats.p_currentHealth -= finalDamage; // Final Damage
        m_playerStats.p_currentHealth = Mathf.Clamp(m_playerStats.p_currentHealth, 0f, m_playerStats.p_maxHealth); // Health cannot go below 0

        if (isCriticalHit) // Damage Pop Up Here
        {
            Debug.Log($"💥 CRITICAL HIT! Player took {finalDamage} damage. Current Health: {m_playerStats.p_currentHealth}");
        }
        else
        {
            Debug.Log($"Player took {finalDamage} damage. Current Health: {m_playerStats.p_currentHealth}");
        }

        if (m_playerStats.p_currentHealth <= 0f) // Death check
        {
            Debug.Log("Player is dead.");
        }
    }

    public bool isDead = false;
    public bool IsDead() => isDead;

    void HandleGrassInteraction() => Shader.SetGlobalVector("_Player", transform.position + Vector3.up * 0.1f);
}
