using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController m_characterController;
    private PlayerInput m_playerInput;
    [Header("Movement Properties")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    private float verticalVelocity = 0f;
    public Vector3 MoveDirection { get; private set; }
    public float Speed => new Vector2(MoveDirection.x, MoveDirection.z).magnitude;
    [Header("Debug View")]
    [SerializeField] private Vector3 debugMoveDirection;
    [SerializeField] private float debugSpeed;
    [SerializeField] private float debugVerticalVelocity;

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

        MoveDirection = (forward * input.y + right * input.x).normalized;

        HandleGravity();

        Vector3 velocity = MoveDirection * moveSpeed;
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
    }

    private void HandleGravity()
    {
        if (m_characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
