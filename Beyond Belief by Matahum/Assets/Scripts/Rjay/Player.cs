using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;

    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        m_playerMovement.HandleMovement();
        m_playerAnimator.HandleAnimations();

        float currentSpeed = m_playerMovement.Speed;
        m_playerAnimator.lastFrameSpeed = currentSpeed;
    }
}
