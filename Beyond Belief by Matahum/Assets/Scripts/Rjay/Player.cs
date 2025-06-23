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

    private int consecutiveIdle1Count = 0;
    private int requiredIdle1Repeats = 0;

    private enum JumpState { None, Jumping, Falling, Landing }
    private JumpState jumpState = JumpState.None;

    private float lastFrameSpeed = 0f;

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
        ResetIdleRepeatCount();
    }

    void Update()
    {
        float currentSpeed = m_playerMovement.Speed;

        m_playerMovement.HandleMovement();

        HandleAnimations();

        lastFrameSpeed = currentSpeed;
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

        if (isJumping && jumpState == JumpState.None)
        {
            jumpState = JumpState.Jumping;
            ChangeAnimationState("player_jump");
            return;
        }

        if (!isGrounded && verticalVelocity < -1f && jumpState != JumpState.Falling)
        {
            jumpState = JumpState.Falling;
            ChangeAnimationState("player_falling");
            return;
        }

        if (isGrounded && jumpState == JumpState.Falling)
        {
            jumpState = JumpState.Landing;
            StartCoroutine(PlayLandAnimation());
            return;
        }

        if (jumpState != JumpState.None || 
            !isGrounded || 
            m_playerMovement.IsJumping() || 
            m_playerMovement.IsAboutToJump())
        {
            return;
        }

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
        float timer = 0f;

        while (timer < duration)
        {
            if (m_playerMovement.Speed > 0.1f)
            {
                jumpState = JumpState.None;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        jumpState = JumpState.None;
    }

    private IEnumerator IdleCycleRoutine()
    {
        isInIdleCycle = true;

        if (!m_playerMovement.GetComponent<CharacterController>().isGrounded)
        {
            isInIdleCycle = false;
            yield break;
        }

        yield return new WaitForSeconds(GetAnimationLength("player_idle_1"));

        if (m_playerMovement.Speed > 0.1f) { isInIdleCycle = false; yield break; }

        consecutiveIdle1Count++;

        if (consecutiveIdle1Count < requiredIdle1Repeats)
        {
            ChangeAnimationState("player_idle_1");
            idleCycleCoroutine = StartCoroutine(IdleCycleRoutine());
            yield break;
        }

        string nextIdle = Random.value > 0.5f ? "player_idle_2" : "player_idle_3";
        ChangeAnimationState(nextIdle);
        yield return new WaitForSeconds(GetAnimationLength(nextIdle));

        if (m_playerMovement.Speed > 0.1f) { isInIdleCycle = false; yield break; }

        ChangeAnimationState("player_idle_1");
        ResetIdleRepeatCount();

        idleCycleCoroutine = StartCoroutine(IdleCycleRoutine());
    }

    private void ResetIdleRepeatCount()
    {
        consecutiveIdle1Count = 0;
        requiredIdle1Repeats = Random.Range(3, 6);
    }

    private float GetAnimationLength(string animName)
    {
        RuntimeAnimatorController ac = m_animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
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

        if (from == "player_land" && to == "player_jog") return 0.15f;
        if (from == "player_land" && to == "player_walk") return 0.15f;
        if (from == "player_land" && to == "player_run") return 0.15f;
        
        // MIGHT REMOVE THIS LATER
        if (from == "player_idle_1" && to == "player_idle_2") return 0f;
        if (from == "player_idle_1" && to == "player_idle_3") return 0f;
        if (from == "player_idle_2" && to == "player_idle_3") return 0f;
        if (from == "player_idle_3" && to == "player_idle_2") return 0f;
        // TBD

        if (from == "player_run" && to == "player_idle_1") return 0.25f;

        if (to == "player_idle_1") return 0.25f;
        if (to == "player_run") return 0.1f;
        if (to == "player_walk") return 0.25f;
        if (to == "player_dash") return 0.1f;
        if (to == "player_jump") return 0.1f;
        if (to == "player_falling") return 0.25f;
        if (to == "player_land") return 0.1f;

        return 0.2f;
    }
}
