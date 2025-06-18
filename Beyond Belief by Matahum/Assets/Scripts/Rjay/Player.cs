using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;

    private Animator m_animator;
    private string currentAnimationState;

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
        float speed = m_playerMovement.Speed;
        Vector3 direction = m_playerMovement.MoveDirection;

        if (speed > 0.1f)
        {
            ChangeAnimationState("player_jog", 0.10f);
        }
        else
        {
            ChangeAnimationState("player_idle_1", 0.25f);
        }
    }
    public void ChangeAnimationState(string newAnimationState, float transitionDuration = 0.1f)
    {
        if (currentAnimationState == newAnimationState) return;
        m_animator.CrossFade(newAnimationState, transitionDuration);
        currentAnimationState = newAnimationState;
    }
}
