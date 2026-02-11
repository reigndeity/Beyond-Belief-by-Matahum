using UnityEngine;

public class PushPlayer : MonoBehaviour
{
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private Vector3 pushDirection = Vector3.up; // push upward by default

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if we actually hit the player
        PlayerMovement player = hit.collider.GetComponent<PlayerMovement>();
        if (player == null) return;

        // Apply push
        player.m_characterController.Move(pushDirection.normalized * pushForce * Time.deltaTime);
    }
}
