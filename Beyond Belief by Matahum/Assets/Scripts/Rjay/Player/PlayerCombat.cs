using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private PlayerWeapon m_playerWeapon;
    private CharacterController m_characterController;

    public bool isAttacking = false;
    private bool canMoveDuringAttack = false;
    public bool IsAttacking() => isAttacking;
    public bool CanMoveDuringAttack() => canMoveDuringAttack;

    [Header("Combat Assist Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float detectionAngle = 360f;
    [SerializeField] private float boostDistance = 2f;
    [SerializeField] private float boostDuration = 0.15f;
    [SerializeField] private float stopBeforeDistance = 0.5f; // Distance to stop before hitting the enemy
    [SerializeField] private float minBoostTriggerDistance = 1.5f;
    [SerializeField] private float maxBoostTriggerDistance = 2.0f;
    [SerializeField] private LayerMask enemyLayer;

    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerWeapon = GetComponentInChildren<PlayerWeapon>();
        m_characterController = GetComponent<CharacterController>();
    }

    public void HandleAttack()
    {
        if (!m_playerMovement.GetComponent<CharacterController>().isGrounded) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Transform enemy = GetNearestEnemy();
            FaceTarget(enemy);
            //TryCombatBoost(enemy);

            m_playerAnimator.animator.SetTrigger("attack");
            AttackState();
        }
    }

    public Transform GetNearestEnemy()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 origin = transform.position;

        Collider[] hits = Physics.OverlapSphere(origin, detectionRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            Vector3 dirToEnemy = (hit.transform.position - origin).normalized;
            float angleToEnemy = Vector3.Angle(transform.forward, dirToEnemy);

            if (angleToEnemy <= detectionAngle * 0.5f)
            {
                float dist = Vector3.Distance(origin, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = hit.transform;
                }
            }
        }

        return nearest;
    }

    public void TryCombatBoost(Transform enemy)
    {
        if (enemy == null) return;

        FaceTarget(enemy);

        float distance = Vector3.Distance(transform.position, enemy.position);

        if (distance > minBoostTriggerDistance && distance < maxBoostTriggerDistance)
        {
            StartCoroutine(DashTowardEnemy(enemy.position, boostDistance, boostDuration));
        }
    }

    private void FaceTarget(Transform target)
    {
        if (target == null) return;

        Vector3 lookPos = target.position;
        lookPos.y = transform.position.y; // Prevent tilting
        transform.LookAt(lookPos);
    }

    private IEnumerator DashTowardEnemy(Vector3 targetPos, float totalDistance, float duration)
    {
        float elapsed = 0f;
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0f;

        while (elapsed < duration)
        {
            float currentDistance = Vector3.Distance(transform.position, targetPos);

            if (currentDistance <= stopBeforeDistance)
                break;

            float step = (totalDistance / duration) * Time.deltaTime;
            Vector3 push = dir * step;
            push.y = m_playerMovement.GetVerticalVelocity() * Time.deltaTime;

            m_characterController.Move(push);
            elapsed += Time.deltaTime;
            yield return null;
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
            Vector3 push = direction * step;
            push.y = m_playerMovement.GetVerticalVelocity() * Time.deltaTime;

            m_characterController.Move(push);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // ANIMATOR EVENT SYSTEM
    public void DashState()
    {
        isAttacking = false;
        canMoveDuringAttack = true;
        DisableWeaponCollider();
        HideWeapon();
    }

    public void AttackTwoPush() => PushPlayerForward(0.5f, 0.12f);
    public void AttackThreePush() => PushPlayerForward(1f, 0.3f);
    public void AttackFourPush() => PushPlayerForward(0.5f, 0.15f);

    private void AttackState()
    {
        isAttacking = true;
        canMoveDuringAttack = false;
        ShowWeapon();
        HideWeaponParticle();
    }

    private void ResetAttackState()
    {
        isAttacking = false;
        canMoveDuringAttack = true;
        HideWeapon();
        ShowWeaponParticle();
    }
    public void ResetAttackCombo()
    {
        int attackLayerIndex = m_playerAnimator.animator.GetLayerIndex("Attack Layer");
        m_playerAnimator.animator.Play("Empty State", attackLayerIndex, 0f);
    }

    public void AttackScaling(float value) => m_playerWeapon.m_scalingAmount = value;
    public void EnableWeaponCollider() => m_playerWeapon.weaponCollider.enabled = true;
    public void DisableWeaponCollider() => m_playerWeapon.weaponCollider.enabled = false;
    public void ShowWeapon() => m_playerWeapon.UndissolveWeapon(0.1f);
    public void HideWeapon() => m_playerWeapon.DissolveWeapon(1f);
    public void ShowWeaponParticle() => m_playerWeapon.ShowDissolveWeaponParticles();
    public void HideWeaponParticle() => m_playerWeapon.HideDissolveWeaponParticles();

    public void SwordSlashOne() => m_playerWeapon.SpawnSwordTrail(0);
    public void SwordSlashTwo() => m_playerWeapon.SpawnSwordTrail(1);
    public void SwordSlashThree() => m_playerWeapon.SpawnSwordTrail(2);
    public void SwordSlashFour() => m_playerWeapon.SpawnSwordTrail(3);
    public void SwordSlashFive() => m_playerWeapon.SpawnSwordTrail(4);




    void OnDrawGizmosSelected()
    {
    #if UNITY_EDITOR
        // Draw detection radius
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        float halfAngle = detectionAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.Euler(0, -halfAngle, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, halfAngle, 0);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawRay(transform.position, leftRayDirection * detectionRadius);
        Gizmos.DrawRay(transform.position, rightRayDirection * detectionRadius);
    #endif
    }
}
