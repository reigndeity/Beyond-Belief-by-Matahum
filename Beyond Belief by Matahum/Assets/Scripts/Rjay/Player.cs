using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;

    void Awake()
    {
        
    }
    void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {
        // MOVES THE PLAYER
        m_playerMovement.HandleMovement();
    }
}
