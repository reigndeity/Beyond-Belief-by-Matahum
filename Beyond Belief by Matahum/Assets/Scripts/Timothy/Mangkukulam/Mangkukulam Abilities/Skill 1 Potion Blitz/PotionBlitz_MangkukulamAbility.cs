using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Potion Blitz")]
public class PotionBlitz_MangkukulamAbility : Mangkukulam_Ability
{
    public GameObject setPositionPrefab;
    private PotionBlitzSetPosition positions;
    public float moveTime = 1.5f;
    public float cooldown;
    public bool canBeUsed = true;

    private Coroutine runningRoutine;
    private System.Action hitAction;

    [Header("Attacking")]
    public GameObject potionPrefab;
    public float throwForce = 10f;        // Base throw strength
    public float upwardForce = 5f;        // Extra arc height
    public float rotationSpeed = 180;

    public override float Cooldown() => cooldown;
    public override bool CanBeUsed() => canBeUsed;

    private void OnEnable()
    {
        canBeUsed = true; // reset every time the domain reloads (enter play mode, recompile, etc.)
    }
    public override void Activate(GameObject user)
    {
        // Prevent starting twice
        if (runningRoutine != null) return;
        Debug.Log("Potion Blitz");

        positions = Instantiate(setPositionPrefab, user.transform.parent.position, Quaternion.identity, user.transform.parent)
                        .GetComponent<PotionBlitzSetPosition>();

        positions.SpawnFlyPosition();

        // Subscribe to hit event
        hitAction = OnHit;
        Mangkukulam.instance.OnHit += hitAction;

        // Start routine
        runningRoutine = CoroutineRunner.Instance.RunCoroutine(DoPotionBlitz(user, positions));
    }

    private IEnumerator DoPotionBlitz(GameObject user, PotionBlitzSetPosition setPosition)
    {
        canBeUsed = false;
        NavMeshAgent agent = user.GetComponent<NavMeshAgent>();
        agent.enabled = false;

        while (true)
        {
            // Safety check
            if (setPosition.posToFly == null || setPosition.posToFly.Length == 0)
            {
                Debug.LogError("Potion Blitz cancelled: no fly positions!");
                break;
            }

            // Pick a random target position
            Transform target = setPosition.posToFly[Random.Range(0, setPosition.posToFly.Length)];

            // Start → arc → end
            Vector3 start = user.transform.position;
            Vector3 mid = start + Vector3.up * 3f; // little flying arc
            Vector3 end = target.position;

            // Fly animation
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / moveTime;
                user.transform.position = Bezier(start, mid, end, t);

                SetRotation(user, target);

                Mangkukulam.instance.isVulnerable = false; // invulnerable during flight
                yield return null;
            }

            // Skill trigger at destination
            Debug.Log("Potion Blitz attack triggered!");
            Mangkukulam.instance.isVulnerable = true;

            CoroutineRunner.Instance.RunCoroutine(AttackWithPotion(user));

            // Wait before flying again
            yield return new WaitForSeconds(5f);

            // If stunned → stop looping
            if (Mangkukulam.instance.isStunned)
            {
                Debug.Log("Potion Blitz cancelled due to stun!");
                break;
            }
        }

        // Cooldown after finishing the loop
        yield return new WaitForSeconds(cooldown);

        runningRoutine = null;
        canBeUsed = true;

        // Re-enable agent for ground movement
        agent.enabled = true;
        agent.Warp(user.transform.position);
    }

    IEnumerator AttackWithPotion(GameObject user)
    {
        yield return new WaitForSeconds(1f);

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

        Vector3 throwPoint = user.transform.position + user.transform.forward * 1f + Vector3.up * 0.5f;
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

    private Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    public void OnHit()
    {
        Debug.Log("Stunned");
        Mangkukulam.instance.OnHit -= hitAction;
        Mangkukulam.instance.isStunned = true;
        Mangkukulam_AttackManager.Instance.isAttacking = false;
        Mangkukulam_AttackManager.Instance.canAttack = true;
        Mangkukulam_AttackManager.Instance.castingCurrentAbility = null;
        runningRoutine = null;
        CoroutineRunner.Instance.RunCoroutine(StartCooldown());
        Destroy(positions.gameObject);
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(Cooldown());
        canBeUsed = true;
    }
}
