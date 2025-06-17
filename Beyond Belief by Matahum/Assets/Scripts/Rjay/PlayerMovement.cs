using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Properties")]
    public float moveSpeed = 5f;

    private CharacterController m_characterController;
    private PlayerInput m_playerInput;

    void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerInput = GetComponent<PlayerInput>();
    }
    public void HandleMovement()
    {
        Vector2 input = m_playerInput.GetMovementInput();
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        Vector3 moveDirection = (forward * input.y + right * input.x).normalized;
        m_characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}
