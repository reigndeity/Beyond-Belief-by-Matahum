using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController m_characterController;
    private PlayerInput m_playerInput;

    [Header("Movement Speeds")]
    public float walkSpeed = 2f;
    public float jogSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpCooldown = 0.1f;
    public float airControlFactor = 0.4f; // 0 = no air control, 1 = full air control
    public float airSpeedDecayRate = 0.98f;
    public bool retainMomentumAfterJump = true;

    private float jumpCooldownTimer = 0f;
    private bool isJumping = false;
    private bool justLanded = false;
    [Tooltip("Delay between pressing jump and the actual jump.")]
    public float jumpStartDelay = 0f; // e.g., 0.15f for 150ms delay

    private bool isJumpStarting = false;
    private float jumpStartTimer = 0f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float fallMultiplier = 2f;
    public float riseMultiplier = 1.2f;
    private float verticalVelocity = 0f;

    [Header("Landing Settings")]
    public float landingVelocityThreshold = -3f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    public bool isSprinting = false;

    [Header("Stamina Delay")]
    public float staminaRegenDelay = 2f;
    private float staminaRegenTimer = 0f;
    private bool wasHoldingSprint = false;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashCooldown = 1.5f;
    public float dashStaminaCost = 20f;
    public float dashDuration = 0.2f;
    private float dashCooldownTimer = 0f;
    public bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private Vector3 dashDirection;
    public static event Action OnDashStarted;

    [Header("Debug View")]
    [SerializeField] private Vector3 debugMoveDirection;
    [SerializeField] private float debugSpeed;
    [SerializeField] private float debugVerticalVelocity;

    public Vector3 MoveDirection { get; private set; }
    public float Speed => new Vector2(MoveDirection.x, MoveDirection.z).magnitude;
    public bool IsWalking => isWalking;
    public bool IsJumping() => isJumping;
    public bool JustLanded() => justLanded && verticalVelocity < landingVelocityThreshold;
    public float GetVerticalVelocity() => verticalVelocity;

    private float currentMoveSpeed;
    private bool isWalking = false;
    private bool wasGroundedLastFrame = true;

    private Vector3 airMomentum = Vector3.zero;

    void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerInput = GetComponent<PlayerInput>();
        currentStamina = maxStamina;
        currentMoveSpeed = jogSpeed;
    }

    public void HandleMovement()
    {
        HandleMovementMode();
        HandleJump();

        if (isDashing)
        {
            m_characterController.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
            dashTimeRemaining -= Time.deltaTime;

            if (dashTimeRemaining <= 0f)
                isDashing = false;

            return;
        }

        Vector2 input = m_playerInput.GetMovementInput();
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        Vector3 inputDir = (forward * input.y + right * input.x).normalized;

        if (m_characterController.isGrounded)
        {
            MoveDirection = inputDir;
            airMomentum = Vector3.zero;
        }
        else
        {
            // Air movement control
            airMomentum *= airSpeedDecayRate;
            if (retainMomentumAfterJump)
            {
                MoveDirection = Vector3.Lerp(airMomentum, inputDir, airControlFactor);
            }
            else
            {
                MoveDirection = Vector3.Lerp(MoveDirection, inputDir, airControlFactor);
            }
        }

        HandleGravity();

        Vector3 velocity = MoveDirection * currentMoveSpeed;
        velocity.y = verticalVelocity;

        m_characterController.Move(velocity * Time.deltaTime);

        if (MoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        debugMoveDirection = MoveDirection;
        debugSpeed = Speed;
        debugVerticalVelocity = verticalVelocity;

        justLanded = !wasGroundedLastFrame && m_characterController.isGrounded;
        wasGroundedLastFrame = m_characterController.isGrounded;
    }

    private void HandleJump()
    {
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        // Still waiting to launch the jump after jump start
        if (isJumpStarting)
        {
            jumpStartTimer -= Time.deltaTime;

            // Launch the actual jump now
            if (jumpStartTimer <= 0f && m_characterController.isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = true;
                isJumpStarting = false;
                jumpCooldownTimer = jumpCooldown;

                if (retainMomentumAfterJump)
                    airMomentum = MoveDirection;
            }

            return;
        }

        // Trigger jump start
        if (Input.GetKeyDown(KeyCode.Space) &&
            m_characterController.isGrounded &&
            jumpCooldownTimer <= 0f)
        {
            if (jumpStartDelay > 0f)
            {
                isJumpStarting = true;
                jumpStartTimer = jumpStartDelay;

                // ✅ Play jump start animation immediately
                Player.instance.ChangeAnimationState("player_jump"); 
            }
            else
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = true;
                jumpCooldownTimer = jumpCooldown;

                if (retainMomentumAfterJump)
                    airMomentum = MoveDirection;

                // ✅ Still play animation immediately even if no delay
                Player.instance.ChangeAnimationState("player_jump");
            }

            return;
        }

        // Reset if grounded and not in a delayed jump
        if (m_characterController.isGrounded && verticalVelocity < 0f && !isJumpStarting)
        {
            verticalVelocity = -2f;
            isJumping = false;
        }
    }


    private void HandleGravity()
    {
        if (!m_characterController.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
            else
                verticalVelocity += gravity * riseMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovementMode()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isWalking = !isWalking;
        }

        bool isHoldingSprint = Input.GetKey(KeyCode.LeftShift);
        bool isTryingToSprint = isHoldingSprint && MoveDirection.magnitude > 0.1f;
        bool canSprint = currentStamina > 0f;

        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            dashCooldownTimer <= 0f &&
            currentStamina >= dashStaminaCost &&
            MoveDirection.magnitude > 0.1f)
        {
            Dash();
        }

        if ((wasHoldingSprint && !isHoldingSprint) && staminaRegenTimer <= 0f)
            staminaRegenTimer = staminaRegenDelay;

        if (currentStamina <= 0f && staminaRegenTimer <= 0f)
            staminaRegenTimer = staminaRegenDelay;

        if (!isDashing)
        {
            if (isTryingToSprint && canSprint)
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

        wasHoldingSprint = isHoldingSprint;
    }

    private void Dash()
    {
        isDashing = true;
        dashTimeRemaining = dashDuration;
        dashCooldownTimer = dashCooldown;

        currentStamina -= dashStaminaCost;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        dashDirection = MoveDirection != Vector3.zero ? MoveDirection : transform.forward;
        verticalVelocity = 0f;

        OnDashStarted?.Invoke();
    }

    public bool IsDashing() => isDashing;
    public float GetDashCooldownRemaining() => Mathf.Clamp(dashCooldownTimer, 0f, dashCooldown);
}
