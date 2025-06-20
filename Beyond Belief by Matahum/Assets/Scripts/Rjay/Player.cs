using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player instance;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;

    private Animator m_animator;
    private string currentAnimationState;

    private Coroutine idleCycleCoroutine;
    private bool isInIdleCycle = false;

    private enum JumpState { None, Jumping, Falling, Landing }
    private JumpState jumpState = JumpState.None;

    void OnEnable()
    {
        PlayerMovement.OnDashStarted += HandleDashAnimation;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        m_playerMovement.HandleMovement();
        HandleAnimations();
    }

    public void HandleAnimations()
    {
        if (m_playerMovement.IsDashing()) return;

        float speed = m_playerMovement.Speed;
        bool isMoving = speed > 0.1f;
        float verticalVelocity = m_playerMovement.GetVerticalVelocity();
        bool isGrounded = m_playerMovement.GetComponent<CharacterController>().isGrounded;
        bool isJumping = m_playerMovement.IsJumping();
        bool justLanded = m_playerMovement.JustLanded();

        // JUMPING
        if (isJumping && jumpState == JumpState.None)
        {
            jumpState = JumpState.Jumping;
            ChangeAnimationState("player_jump");
            return;
        }

        // FALLING
        if (!isGrounded && verticalVelocity < -1f && jumpState != JumpState.Falling)
        {
            jumpState = JumpState.Falling;
            ChangeAnimationState("player_falling");
            return;
        }

        // LANDING
        if (justLanded && (jumpState == JumpState.Jumping || jumpState == JumpState.Falling))
        {
            jumpState = JumpState.Landing;
            StartCoroutine(PlayLandAnimation());
            return;
        }

        // Prevent idle/movement animation if jump-related animation is ongoing
        if (jumpState != JumpState.None) return;

        // Idle Cycle
        if (!isMoving)
        {
            if (currentAnimationState != "player_idle_1" &&
                currentAnimationState != "player_idle_2" &&
                currentAnimationState != "player_idle_3")
            {
                ChangeAnimationState("player_idle_1");
            }

            if (!isInIdleCycle && currentAnimationState == "player_idle_1")
            {
                idleCycleCoroutine = StartCoroutine(IdleCycleRoutine());
            }

            return;
        }

        // Stop idle cycle if player starts moving
        if (idleCycleCoroutine != null)
        {
            StopCoroutine(idleCycleCoroutine);
            idleCycleCoroutine = null;
            isInIdleCycle = false;
        }

        if (m_playerMovement.isSprinting)
        {
            ChangeAnimationState("player_run");
        }
        else if (m_playerMovement.IsWalking)
        {
            ChangeAnimationState("player_walk");
        }
        else
        {
            ChangeAnimationState("player_jog");
        }
    }

    private IEnumerator PlayLandAnimation()
    {
        ChangeAnimationState("player_land");

        float duration = GetAnimationLength("player_land");
        yield return new WaitForSeconds(duration);

        jumpState = JumpState.None;
    }

    private IEnumerator IdleCycleRoutine()
    {
        isInIdleCycle = true;

        yield return new WaitForSeconds(GetAnimationLength("player_idle_1"));

        if (m_playerMovement.Speed > 0.1f) { isInIdleCycle = false; yield break; }

        string nextIdle = Random.value > 0.5f ? "player_idle_2" : "player_idle_3";
        ChangeAnimationState(nextIdle);
        yield return new WaitForSeconds(GetAnimationLength(nextIdle));

        if (m_playerMovement.Speed > 0.1f) { isInIdleCycle = false; yield break; }

        ChangeAnimationState("player_idle_1");
        isInIdleCycle = false;
    }

    private float GetAnimationLength(string animName)
    {
        RuntimeAnimatorController ac = m_animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f; // fallback
    }

    private void HandleDashAnimation()
    {
        ChangeAnimationState("player_dash");
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        if (currentAnimationState == newAnimationState) return;

        float transitionDuration = GetTransitionDuration(currentAnimationState, newAnimationState);
        m_animator.CrossFade(newAnimationState, transitionDuration);
        currentAnimationState = newAnimationState;
    }

    private float GetTransitionDuration(string from, string to)
    {
        if (from == "player_idle_1" && to == "player_jog") return 0.05f;
        if (from == "player_idle_1" && to == "player_walk") return 0.1f;
        if (from == "player_idle_2" && to == "player_jog") return 0.05f;
        if (from == "player_idle_2" && to == "player_walk") return 0.1f;
        if (from == "player_idle_3" && to == "player_jog") return 0.05f;
        if (from == "player_idle_3" && to == "player_walk") return 0.1f;
        if (from == "player_run" && to == "player_jog") return 0.25f;
        if (from == "player_walk" && to == "player_jog") return 0.15f;
        if (to == "player_idle_1") return 0.25f;
        if (to == "player_run") return 0.1f;
        if (to == "player_walk") return 0.25f;
        if (to == "player_dash") return 0.1f;
        if (to == "player_jump") return 0.05f;
        if (to == "player_falling") return 0.25f;
        if (to == "player_land") return 0.05f;

        return 0.2f;
    }
}
