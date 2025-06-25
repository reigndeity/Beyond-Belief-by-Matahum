using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private CharacterController m_characterController;

    public bool isAttacking = false;
    private bool canMoveDuringAttack = false;
    public bool IsAttacking() => isAttacking;
    public bool CanMoveDuringAttack() => canMoveDuringAttack;

    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_characterController = GetComponent<CharacterController>();
    }

    public void HandleAttack()
    {
        // Prevent attacking while not grounded
        if (!m_playerMovement.GetComponent<CharacterController>().isGrounded) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_playerAnimator.animator.SetTrigger("attack");
            AttackState();
        }
    }
    public void PushPlayerForward(float distance, float duration)
    {
        StartCoroutine(SmoothPushRoutine(distance, duration));
    }

    private IEnumerator SmoothPushRoutine(float totalDistance, float duration)
    {
        float elapsed = 0f;
        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction.Normalize();

        while (elapsed < duration)
        {
            float step = (totalDistance / duration) * Time.deltaTime;

            // Combine horizontal push and vertical motion
            Vector3 push = direction * step;
            push.y = m_playerMovement.GetVerticalVelocity() * Time.deltaTime;

            m_characterController.Move(push);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    // ANIMATOR EVENT SYSTEM
    public void AttackTwoPush() => PushPlayerForward(0.5f,0.12f);
    public void AttackThreePush() => PushPlayerForward(1f, 0.3f);
    public void AttackFourPush() => PushPlayerForward(0.5f, 0.15f);
    private void AttackState()
    {
        isAttacking = true;
        canMoveDuringAttack = false;
    }
    private void ResetAttackState()
    {
        isAttacking = false;
        canMoveDuringAttack = true;
    }
    public void ResetAttackCombo()
    {
        int attackLayerIndex = m_playerAnimator.animator.GetLayerIndex("Attack Layer");
        m_playerAnimator.animator.Play("Empty State", attackLayerIndex, 0f);
    }
}
