using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private CharacterController m_characterController;

    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_characterController = GetComponent<CharacterController>();
    }

    public void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_playerAnimator.animator.SetTrigger("attack");
        }
    }

    public void EnableRootMotion()
    {
        m_playerAnimator.animator.applyRootMotion = true;
    }
    public void DisableRootMotion()
    {
        m_playerAnimator.animator.applyRootMotion = false;
    }
    public void AttackTwoPush() => PushPlayerForward(0.5f,0.12f);
    public void AttackThreePush() => PushPlayerForward(1f, 0.3f);
    public void AttackFourPush() => PushPlayerForward(0.5f, 0.15f);
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
            m_characterController.Move(direction * step);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }


}
