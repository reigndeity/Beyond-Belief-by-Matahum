using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;
    private PlayerCombat m_playerCombat;

    public Animator animator;
    private string currentAnimationState;

    private Coroutine idleCycleCoroutine;
    private bool isInIdleCycle = false;

    private Coroutine stopAnimationCoroutine;
    private bool isPlayingStopAnimation = false;

    private int consecutiveIdle1Count = 0;
    private int requiredIdle1Repeats = 0;

    private enum JumpState { None, Jumping, Falling, Landing }
    private JumpState jumpState = JumpState.None;

    public float lastFrameSpeed = 0f;

    // Speed buffer fields
    private Queue<float> recentSpeeds = new Queue<float>();
    private float speedSampleInterval = 0.02f;
    private float speedSampleTimer = 0f;
    private float speedMemoryDuration = 0.1f;

    // 👇 Air time tracking to prevent false land triggers
    private float airTime = 0f;
    private const float minFallTime = 0.1f;

    [Header("Hit Properties")]
    public bool isHit;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float faceEnemyRadius = 10f;
    [SerializeField] private float rotationSmoothness = 10f; // Degrees per second
    private Coroutine faceEnemyCoroutine;

    void OnEnable()
    {
        PlayerMovement.OnDashStarted += HandleDashAnimation;
    }

    void OnDisable()
    {
        PlayerMovement.OnDashStarted -= HandleDashAnimation;
    }

    void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
        m_playerCombat = GetComponent<PlayerCombat>();
        animator = GetComponent<Animator>();
        ResetIdleRepeatCount();
    }

    public void HandleAnimations()
    {
        if (isHit) return;
        if (m_playerMovement.IsDashing()) return;

        float speed = m_playerMovement.Speed;
        lastFrameSpeed = speed;

        // Update recent speed history
        speedSampleTimer += Time.deltaTime;
        if (speedSampleTimer >= speedSampleInterval)
        {
            speedSampleTimer = 0f;
            recentSpeeds.Enqueue(speed);
            while (recentSpeeds.Count > Mathf.CeilToInt(speedMemoryDuration / speedSampleInterval))
                recentSpeeds.Dequeue();
        }

        bool hasInput = m_playerMovement.MoveDirection.magnitude > 0.1f;
        bool isMoving = hasInput && !m_playerCombat.IsAttacking();

        float verticalVelocity = m_playerMovement.GetVerticalVelocity();
        bool isGrounded = m_playerMovement.GetComponent<CharacterController>().isGrounded;
        bool isJumping = m_playerMovement.IsJumping();

        if (isJumping && jumpState == JumpState.None)
        {
            jumpState = JumpState.Jumping;
            ChangeAnimationState("player_jump");
            return;
        }

        if (!isGrounded)
        {
            airTime += Time.deltaTime;

            if (verticalVelocity < -1f && jumpState != JumpState.Falling && airTime > minFallTime)
            {
                jumpState = JumpState.Falling;
                ChangeAnimationState("player_falling");
                return;
            }
        }
        else
        {
            if (jumpState == JumpState.Falling)
            {
                jumpState = JumpState.Landing;
                StartCoroutine(PlayLandAnimation());
                return;
            }

            airTime = 0f; // reset when grounded
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
            HandleStopAndIdleAnimations();
        }
        else
        {
            if (idleCycleCoroutine != null)
            {
                StopCoroutine(idleCycleCoroutine);
                idleCycleCoroutine = null;
                isInIdleCycle = false;
                ResetIdleRepeatCount();
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

        // THIS IS NEW, MIGHT REMOVE IF STILL SHIT BUT IT SEEMS TO FIX THE ISSUE WITH THE IDLE SLIDING BUG 09/07/2025 - 2:23PM
        if (m_playerMovement.MoveDirection.magnitude > 0.1f &&
            currentAnimationState.StartsWith("player_idle"))
        {
            if (m_playerMovement.isSprinting)
                ChangeAnimationState("player_run");
            else if (m_playerMovement.IsWalking)
                ChangeAnimationState("player_walk");
            else
                ChangeAnimationState("player_jog");
        }
    }

    private void HandleStopAndIdleAnimations()
    {
        if (isPlayingStopAnimation) return;

        if (JustStoppedAbruptly(7.9f))
        {
            ChangeAnimationState("player_runToStop");
            stopAnimationCoroutine = StartCoroutine(PlayStopAnimation("player_runToStop"));
            return;
        }
        else if (JustStoppedAbruptly(4.9f))
        {
            ChangeAnimationState("player_jogToStop");
            stopAnimationCoroutine = StartCoroutine(PlayStopAnimation("player_jogToStop"));
            return;
        }

        if (currentAnimationState != "player_idle_1" &&
            currentAnimationState != "player_idle_2" &&
            currentAnimationState != "player_idle_3")
        {
            ChangeAnimationState("player_idle_1");
        }

        if (!isInIdleCycle && currentAnimationState == "player_idle_1" && m_playerMovement.Speed <= 0.1f)
        {
            idleCycleCoroutine = StartCoroutine(IdleCycleRoutine());
        }
    }

    private bool JustStoppedAbruptly(float threshold)
    {
        if (m_playerMovement.Speed > 0.1f) return false;

        foreach (float pastSpeed in recentSpeeds)
        {
            if (pastSpeed >= threshold)
                return true;
        }

        return false;
    }

    private IEnumerator PlayStopAnimation(string animName)
    {
        isPlayingStopAnimation = true;

        float duration = GetAnimationLength(animName);
        float timer = 0f;

        while (timer < duration)
        {
            if (m_playerMovement.Speed > 0.1f)
            {
                isPlayingStopAnimation = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isPlayingStopAnimation = false;
    }

    private IEnumerator PlayLandAnimation()
    {
        // Extra safeguard: only land if airtime was real
        if (airTime < 0.15f)
        {
            jumpState = JumpState.None;
            yield break;
        }

        ChangeAnimationState("player_land");
        recentSpeeds.Clear();

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

        while (true)
        {
            yield return new WaitForSeconds(GetAnimationLength("player_idle_1"));

            if (m_playerMovement.Speed > 0.1f || !m_playerMovement.GetComponent<CharacterController>().isGrounded)
            {
                ResetIdleCycle();
                yield break;
            }

            if (currentAnimationState != "player_idle_1")
            {
                ResetIdleCycle();
                yield break;
            }

            consecutiveIdle1Count++;

            if (consecutiveIdle1Count >= requiredIdle1Repeats)
            {
                string nextIdle = Random.value > 0.5f ? "player_idle_2" : "player_idle_3";
                ChangeAnimationState(nextIdle);

                yield return new WaitForSeconds(GetAnimationLength(nextIdle));

                if (m_playerMovement.Speed > 0.1f)
                {
                    ResetIdleCycle();
                    yield break;
                }

                ChangeAnimationState("player_idle_1");
                ResetIdleRepeatCount();
            }
        }
    }

    private void ResetIdleRepeatCount()
    {
        consecutiveIdle1Count = 0;
        requiredIdle1Repeats = Random.Range(3, 6);
    }

    private void ResetIdleCycle()
    {
        isInIdleCycle = false;
        ResetIdleRepeatCount();

        if (idleCycleCoroutine != null)
        {
            StopCoroutine(idleCycleCoroutine);
            idleCycleCoroutine = null;
        }
    }

    private float GetAnimationLength(string animName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
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
        recentSpeeds.Clear();
    }

    public void GetHit()
    {
        isHit = true;
        FaceClosestEnemy();
        int hitIndex = Random.Range(1, 4);
        string hitAnim = $"player_getHit_{hitIndex}";
        ChangeAnimationState(hitAnim);
        StartCoroutine(ApplyHitStun(0.8f));
    }
    private IEnumerator ApplyHitStun(float duration)
    {
        m_playerInput.enabled = false;
        m_playerMovement.enabled = false;
        yield return new WaitForSeconds(duration);
        m_playerInput.enabled = true;
        m_playerMovement.enabled = true;

        isHit = false;
    }

    private void FaceClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, faceEnemyRadius, enemyLayer);
        if (enemies.Length == 0) return;

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closest = enemy.transform;
                closestDistance = dist;
            }
        }

        if (closest != null)
        {
            if (faceEnemyCoroutine != null)
                StopCoroutine(faceEnemyCoroutine);

            faceEnemyCoroutine = StartCoroutine(SmoothRotateTo(closest));
        }
    }
    private IEnumerator SmoothRotateTo(Transform target)
    {
        Quaternion startRotation = transform.rotation;
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float angleDiff = Quaternion.Angle(startRotation, targetRotation);
        float duration = angleDiff / rotationSmoothness;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        if (currentAnimationState == newAnimationState) return;

        float transitionDuration = GetTransitionDuration(currentAnimationState, newAnimationState);
        animator.CrossFade(newAnimationState, transitionDuration);
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
        if (from == "player_run" && to == "player_jog") return 0.5f;
        if (from == "player_walk" && to == "player_jog") return 0.15f;

        if (from == "player_land" && to == "player_jog") return 0.15f;
        if (from == "player_land" && to == "player_walk") return 0.15f;
        if (from == "player_land" && to == "player_run") return 0.15f;

        if (from == "player_idle_1" && to == "player_idle_2") return 0f;
        if (from == "player_idle_1" && to == "player_idle_3") return 0f;
        if (from == "player_idle_2" && to == "player_idle_3") return 0f;
        if (from == "player_idle_3" && to == "player_idle_2") return 0f;

        if (from == "player_run" && to == "player_idle_1") return 0.25f;

        if (to == "player_idle_1") return 0.25f;
        if (to == "player_run") return 0.1f;
        if (to == "player_walk") return 0.25f;
        if (to == "player_dash") return 0.1f;
        if (to == "player_jump") return 0.1f;
        if (to == "player_falling") return 0.25f;
        if (to == "player_land") return 0.1f;
        if (to == "player_getHit_1") return 0f;
        if (to == "player_getHit_2") return 0f;
        if (to == "player_getHit_3") return 0f;

        if (from == "player_jog" && to == "player_jogToStop") return 0.3f;
        if (from == "player_run" && to == "player_jogToStop") return 0.25f;
        if (from == "player_run" && to == "player_runToStop") return 0.5f;

        if (from == "player_attack_1" && to == "player_jog") return 0.25f;
        if (from == "player_attack_2" && to == "player_jog") return 0.25f;
        if (from == "player_attack_3" && to == "player_jog") return 0.25f;
        if (from == "player_attack_4" && to == "player_jog") return 0.25f;

        return 0.2f;
    }
    public void ForceIdleState()
    {
        StopAllCoroutines();
        isHit = false;

        idleCycleCoroutine = null;
        stopAnimationCoroutine = null;
        isInIdleCycle = false;
        isPlayingStopAnimation = false;

        ChangeAnimationState("player_idle_1");
    }
}