using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;

    private Animator m_animator;
    private string currentAnimationState;

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
        // MOVES THE PLAYER
        m_playerMovement.HandleMovement();
        HandleAnimations();
    }

    public void HandleAnimations()
    {
         if (m_playerMovement.IsDashing()) return;
        float speed = m_playerMovement.Speed;
        bool isMoving = speed > 0.1f;

        if (!isMoving)
        {
            ChangeAnimationState("player_idle_1", 0.25f);
            return;
        }

        if (m_playerMovement.isSprinting)
        {
            ChangeAnimationState("player_run", 0.1f);
        }
        else if (m_playerMovement.IsWalking)
        {
            ChangeAnimationState("player_walk", 0.25f);
        }
        else
        {
            ChangeAnimationState("player_jog", 0.25f);
        }
    }
    private void HandleDashAnimation() => ChangeAnimationState("player_dash", 0.1f);
    public void ChangeAnimationState(string newAnimationState, float transitionDuration)
    {
        if (currentAnimationState == newAnimationState) return;
        m_animator.CrossFade(newAnimationState, transitionDuration);
        currentAnimationState = newAnimationState;
    }
}
