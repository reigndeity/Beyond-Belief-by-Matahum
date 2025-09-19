using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Potion Throw")]
public class PotionThrow_MangkukulamAbility : Mangkukulam_Ability
{
    [Header("Attacking")]
    public GameObject potionPrefab;
    public float throwForce = 10f;        // Base throw strength
    public float upwardForce = 5f;        // Extra arc height
    public float rotationSpeed = 180;

    public float cooldown = 5;
    public override float Cooldown() => cooldown;
    public bool canBeUsed = true;
    public override bool CanBeUsed() => canBeUsed;
    public override void Initialize() => canBeUsed = true;
    public override void Activate(GameObject user)
    {
        canBeUsed = true;
        CoroutineRunner.Instance.RunCoroutine(AttackWithPotion(user));
    }
    IEnumerator AttackWithPotion(GameObject user)
    {
        Mangkukulam_AnimationManager animator = user.GetComponent<Mangkukulam_AnimationManager>();

        animator.ChangeAnimationState("Mangkukulam_Skill_1_Potion Blitz");
        yield return new WaitForSeconds(0.2f);
        
        Transform target = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (target == null)
        {
            Debug.LogWarning("PotionBlitz: No player found to target!");
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            SetRotation(user, target);
            yield return null;
        }

        ThrowPotions(user, target);

        yield return new WaitForSeconds(1.2f);

        CoroutineRunner.Instance.RunCoroutine(StartCooldown());

        Mangkukulam_AttackManager atkMngr = user.GetComponent<Mangkukulam_AttackManager>();
        atkMngr.isAttacking = false;
        atkMngr.canAttack = true;
        atkMngr.castingCurrentAbility = null;
    }
    void ThrowPotions(GameObject user, Transform target)
    {
        Vector3 throwPoint = user.transform.position + user.transform.forward * 1f + Vector3.up * 1f;
        GameObject projectile = Instantiate(potionPrefab, throwPoint, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 targetPos = target.position + Vector3.up * 1f; // aim at chest/head height
            Vector3 velocity = CalculateLaunchVelocity(throwPoint, targetPos, 1.5f); // 1.5s flight time
            rb.linearVelocity = velocity;
        }
    }
    Vector3 CalculateLaunchVelocity(Vector3 origin, Vector3 target, float timeToTarget)
    {
        Vector3 toTarget = target - origin;

        // Separate horizontal and vertical distances
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float t = timeToTarget;

        // Solve for velocity
        float vY = y / t + 0.5f * Mathf.Abs(Physics.gravity.y) * t;
        float vXZ = xz / t;

        Vector3 result = toTargetXZ.normalized * vXZ;
        result.y = vY;

        return result;
    }
    void SetRotation(GameObject user, Transform target)
    {
        // Get the direction to target, but only in XZ plane
        Vector3 dir = target.position - user.transform.position;
        dir.y = 0f; // remove vertical difference

        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            user.transform.rotation = Quaternion.Lerp(
                user.transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(Cooldown());
        canBeUsed = true;
    }
}
