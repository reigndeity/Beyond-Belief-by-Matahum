using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private LookAtTarget m_lookAtTarget;
    public CharacterController m_characterController;
    private PlayerInput m_playerInput;
    private PlayerCombat m_playerCombat;
    private PlayerAnimator m_playerAnimator;

    [Header("Movement Speeds")]
    public float walkSpeed = 2f;
    public float jogSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpCooldown = 0.1f;
    public float airControlFactor = 0.4f;
    public float airSpeedDecayRate = 0.98f;
    public bool retainMomentumAfterJump = true;

    private float jumpCooldownTimer = 0f;
    private bool isJumping = false;
    private bool justLanded = false;

    [Tooltip("Delay between pressing jump and the actual jump.")]
    public float jumpStartDelay = 0f;

    private bool isJumpStarting = false;
    private float jumpStartTimer = 0f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float fallMultiplier = 2f;
    public float riseMultiplier = 1.2f;
    public float verticalVelocity = 0f;

    [Header("Landing Settings")]
    public float landingVelocityThreshold = -2f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    public bool isSprinting = false;
    public float CurrentStamina { get => currentStamina; set => currentStamina = value; }
    public float MaxStamina     { get => maxStamina;     set => maxStamina     = value; }


    [Header("Stamina Delay")]
    public float staminaRegenDelay = 2f;
    private float staminaRegenTimer = 0f;
    private bool wasHoldingSprint = false;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashCooldown = 1.5f;
    public float dashStaminaCost = 20f;
    public float dashDuration = 0.2f;
    public bool canDashWhileAirborne = false;
    private float dashCooldownTimer = 0f;
    public bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private Vector3 dashDirection;
    public static event Action OnDashStarted;

    [Header("Movement Visuals")]
    [SerializeField] private VisualEffect dashVFX;
    [SerializeField] private float dashVFXDuration = 1f; // Set this to match VFX duration
    [SerializeField] ParticleSystem sprintTrail;

    [Header("Debug View")]
    [SerializeField] private Vector3 debugMoveDirection;
    [SerializeField] private float debugSpeed;
    [SerializeField] private float debugVerticalVelocity;

    public Vector3 MoveDirection { get; private set; }

    public float Speed => new Vector2(m_characterController.velocity.x, m_characterController.velocity.z).magnitude;

    public bool IsWalking => isWalking;
    public bool IsJumping() => isJumping;
    public bool JustLanded() => justLanded && verticalVelocity < landingVelocityThreshold;
    public float GetVerticalVelocity() => verticalVelocity;

    private float currentMoveSpeed;
    private bool isWalking = false;
    private bool wasGroundedLastFrame = true;

    private Vector3 airMomentum = Vector3.zero;

    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerInput = GetComponent<PlayerInput>();
        m_playerCombat = GetComponent<PlayerCombat>();
        m_playerAnimator = GetComponent<PlayerAnimator>();
        currentStamina = maxStamina;
        currentMoveSpeed = jogSpeed;

        m_lookAtTarget = GetComponent<LookAtTarget>();
    }

    public void HandleMovement()
    {
        if (GetComponent<PlayerSkills>().isUsingNormalSkill || GetComponent<PlayerSkills>().isUsingUltimateSkill)
            return;
        
        HandleMovementMode();

        Vector2 input = m_playerInput.GetMovementInput();
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        Vector3 inputDir = (forward * input.y + right * input.x).normalized;

        if (m_playerCombat.IsAttacking() && !m_playerCombat.CanMoveDuringAttack())
        {
            MoveDirection = Vector3.zero;
            return; // skip rest of movement
        }

        if (m_characterController.isGrounded)
        {
            MoveDirection = inputDir;
            airMomentum = Vector3.zero;
        }
        else
        {
            airMomentum *= airSpeedDecayRate;
            MoveDirection = retainMomentumAfterJump
                ? Vector3.Lerp(airMomentum, inputDir, airControlFactor)
                : Vector3.Lerp(MoveDirection, inputDir, airControlFactor);
        }

        HandleGravity();

        Vector3 velocity = MoveDirection * currentMoveSpeed;
        velocity.y = verticalVelocity;

        m_characterController.Move(velocity * Time.deltaTime);

        if (isDashing && dashDirection != Vector3.zero)
        {
            Quaternion dashRotation = Quaternion.LookRotation(dashDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, dashRotation, 20f * Time.deltaTime);
        }
        else if (MoveDirection != Vector3.zero)
        {
            Quaternion moveRotation = Quaternion.LookRotation(MoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, 10f * Time.deltaTime);
        }

        debugMoveDirection = MoveDirection;
        debugSpeed = Speed;
        debugVerticalVelocity = verticalVelocity;

        justLanded = !wasGroundedLastFrame && m_characterController.isGrounded;
        wasGroundedLastFrame = m_characterController.isGrounded;
    }

    public void HandleJump()
    {
        if (GetComponent<Player>().suppressInputUntilNextFrame) return;
        if (GetComponent<PlayerSkills>().isUsingNormalSkill || GetComponent<PlayerSkills>().isUsingUltimateSkill)
            return;
        if (m_playerCombat.IsAttacking() && !m_playerCombat.CanMoveDuringAttack())
            return;
        
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        if (isJumpStarting)
        {
            jumpStartTimer -= Time.deltaTime;

            if (jumpStartTimer <= 0f && m_characterController.isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = true;
                isJumpStarting = false;
                jumpCooldownTimer = jumpCooldown;

                if (retainMomentumAfterJump)
                    airMomentum = MoveDirection.normalized * currentMoveSpeed;
            }

            return;
        }

        if (Input.GetKeyDown(m_playerInput.jumpKey) && m_characterController.isGrounded &&jumpCooldownTimer <= 0f)
        {
            m_lookAtTarget.DisableLooking();
            if (jumpStartDelay > 0f)
            {
                isJumpStarting = true;
                jumpStartTimer = jumpStartDelay;
                m_playerAnimator.ChangeAnimationState("player_jump");
            }
            else
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = true;
                jumpCooldownTimer = jumpCooldown;

                if (retainMomentumAfterJump)
                    airMomentum = MoveDirection;

                m_playerAnimator.ChangeAnimationState("player_jump");
            }

            return;
        }

        if (m_characterController.isGrounded && verticalVelocity < 0f && !isJumpStarting)
        {
            verticalVelocity = -2f;
            isJumping = false;
            m_lookAtTarget.EnableLooking();
        }
    }

    public void HandleGravity()
    {
        if (!m_characterController.isGrounded)
        {
            verticalVelocity += (verticalVelocity < 0 ? gravity * fallMultiplier : gravity * riseMultiplier) * Time.deltaTime;
        }
    }

    public void ApplyGravityStep()
    {
        if (!m_characterController.isGrounded)
        {
            verticalVelocity += (verticalVelocity < 0 ? gravity * fallMultiplier : gravity * riseMultiplier) * Time.deltaTime;
        }
        else if (verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // force slight downward push to keep grounded
        }

        Vector3 velocity = new Vector3(0, verticalVelocity, 0);
        m_characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMovementMode()
    {
        if (TutorialManager.instance.tutorial_canMovementToggle && Input.GetKeyDown(m_playerInput.toggleKey))
        {
            isWalking = !isWalking;
        }

        bool isHoldingSprint = Input.GetKey(m_playerInput.sprintKey);
        bool isTryingToSprint = isHoldingSprint && MoveDirection.magnitude > 0.1f;
        bool canSprint = currentStamina > 0f;

        if ((wasHoldingSprint && !isHoldingSprint) && staminaRegenTimer <= 0f)
            staminaRegenTimer = staminaRegenDelay;

        if (currentStamina <= 0f && staminaRegenTimer <= 0f)
            staminaRegenTimer = staminaRegenDelay;

        if (!isDashing)
        {
            if (!TutorialManager.instance.tutorial_canSprintAndDash)
            {
                isSprinting = false;
                currentMoveSpeed = isWalking ? walkSpeed : jogSpeed;
            }
            else
            {
                if (isTryingToSprint && canSprint && m_characterController.isGrounded)
                {
                    isSprinting = true;
                    currentMoveSpeed = sprintSpeed;
                    currentStamina -= staminaDrainRate * Time.deltaTime;
                    currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
                    staminaRegenTimer = staminaRegenDelay;
                }
                else
                {
                    isSprinting = false;
                    currentMoveSpeed = isWalking ? walkSpeed : jogSpeed;

                    if (staminaRegenTimer > 0f)
                        staminaRegenTimer -= Time.deltaTime;

                    if (staminaRegenTimer <= 0f && !isHoldingSprint && currentStamina < maxStamina)
                    {
                        currentStamina += staminaRegenRate * Time.deltaTime;
                        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
                    }
                }
            }
        }

        wasHoldingSprint = isHoldingSprint;

        if (isSprinting == false)
        {
            StopSprintTrail();
        }
    }

    public void HandleDash()
    {
        if (!TutorialManager.instance.tutorial_canSprintAndDash) return;
        if (GetComponent<PlayerSkills>().isUsingNormalSkill || GetComponent<PlayerSkills>().isUsingUltimateSkill)
            return;
        
        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(m_playerInput.sprintKey) &&
            dashCooldownTimer <= 0f &&
            currentStamina >= dashStaminaCost &&
            (canDashWhileAirborne || m_characterController.isGrounded))
        {
            Vector2 input = m_playerInput.GetMovementInput();
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = right.y = 0f;

            dashDirection = input.sqrMagnitude > 0.01f
                ? (forward * input.y + right * input.x).normalized
                : transform.forward;

            StartDash();
        }

        if (isDashing)
        {
            m_characterController.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
            dashTimeRemaining -= Time.deltaTime;

            if (dashTimeRemaining <= 0f)
                isDashing = false;
        }
    }

    private void StartDash()
    {
        m_playerCombat.ResetAttackCombo();
        isDashing = true;
        dashTimeRemaining = dashDuration;
        dashCooldownTimer = dashCooldown;

        currentStamina -= dashStaminaCost;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        verticalVelocity = 0f;

        OnDashStarted?.Invoke();
    }

    public bool IsDashing() => isDashing;
    public float GetDashCooldownRemaining() => Mathf.Clamp(dashCooldownTimer, 0f, dashCooldown);

    public void DashParticle()
    {
        if (dashVFX == null) return;

        // Activate and play the effect
        dashVFX.gameObject.SetActive(true);
        dashVFX.Play();

        // Automatically disable after it's done
        StartCoroutine(DisableVFXAfterDuration(dashVFXDuration));
    }
    public void StartSprintTrail()
    {
        sprintTrail.Play();
    }
    public void StopSprintTrail()
    {
        sprintTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private IEnumerator DisableVFXAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        dashVFX.gameObject.SetActive(false);
    }
    public bool IsAboutToJump() => isJumpStarting;


    public void ForceStop()
    {
        MoveDirection = Vector3.zero;
        isSprinting = false;
        isJumping = false;
        isJumpStarting = false;
        dashTimeRemaining = 0f;
        isDashing = false;
        verticalVelocity = -2f;

        StopSprintTrail();
    }

    public void ToggleWalk()
    {
        isWalking = !isWalking;
        currentMoveSpeed = isWalking ? walkSpeed : jogSpeed;
    }

}
