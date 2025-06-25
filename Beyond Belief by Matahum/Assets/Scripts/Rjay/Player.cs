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
public class Player : MonoBehaviour
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;
    private PlayerCombat m_playerCombat;
    
    public PlayerState currentState;


    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
        m_playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        m_playerAnimator.HandleAnimations();
        m_playerMovement.HandleMovement();
        m_playerMovement.HandleDash();
        m_playerMovement.HandleJump();
        m_playerCombat.HandleAttack();

        float currentSpeed = m_playerMovement.Speed;
        m_playerAnimator.lastFrameSpeed = currentSpeed;

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
}
