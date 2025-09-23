using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Demonic Ritual")]
public class DemonicRitual_MangkukulamAbility : Mangkukulam_Ability
{
    public GameObject candlePillarPrefab;
    public float candleToHealthRatio = 20;
    public float cooldown;
    public override float Cooldown() => cooldown;

    [Header("Move to center")]
    public float moveTime = 3;
    public float rotationSpeed = 180;

    private Coroutine runningRoutine;

    public bool canBeUsed = true;
    public override bool CanBeUsed() => canBeUsed;

    public override void Initialize() => canBeUsed = true;

    private void OnDisable()
    {
        if (runningRoutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(runningRoutine);
            runningRoutine = null;
        }
    }

    public override void Activate(GameObject user)
    {
        // Prevent starting twice
        if (runningRoutine != null) return;

        runningRoutine = CoroutineRunner.Instance.RunCoroutine(FlyToCenter(user));
    }

    IEnumerator FlyToCenter(GameObject user)
    {
        Mangkukulam.instance.isVulnerable = false;
        canBeUsed = false;
        NavMeshAgent agent = user.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Mangkukulam.instance.isFlying = true;
        Mangkukulam_AnimationManager animator = user.GetComponent<Mangkukulam_AnimationManager>();
        animator.ChangeAnimationState("Mangkukulam_StartFly");
        yield return new WaitForSeconds(animator.GetAnimationLength("Mangkukulam_StartFly"));

        // Move target = parent (center of arena?)
        Transform target = user.transform.parent;

        // Bezier arc
        Vector3 start = user.transform.position;
        Vector3 mid = start + Vector3.up * 3f;
        Vector3 end = target.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            user.transform.position = Bezier(start, mid, end, t);

            SetRotation(user, target);

            animator.ChangeAnimationState("Mangkukulam_Flying");

            yield return null;
        }

        animator.ChangeAnimationState("Mangkukulam_Landing");
        yield return new WaitForSeconds(animator.GetAnimationLength("Mangkukulam_Landing"));
        Mangkukulam.instance.isFlying = false;

        // Spawn candle holder
        CandleHolder holder = Instantiate(
            candlePillarPrefab,
            user.transform.position,
            user.transform.rotation,
            user.transform
        ).GetComponent<CandleHolder>();

        holder.Initialize(
            user.GetComponent<Mangkukulam>(),
            user.GetComponent<EnemyStats>(),
            candleToHealthRatio
        );

        holder.SpawnCandles();

        animator.ChangeAnimationState("Mangkukulam_Skill_3_Demonic Ritual");
        yield return new WaitForSeconds(animator.GetAnimationLength("Mangkukulam_Skill_3_Demonic Ritual"));

        // --- Ritual loop indefinitely ---
        while (holder != null) // keep looping until candles destroyed
        {
            animator.ChangeAnimationState("Demonic Ritual_Idle");
            yield return null;
        }

        Debug.Log("Ritual has ended");
        // Ritual ended externally (candles destroyed)
        CoroutineRunner.Instance.RunCoroutine(DemonicRitualOnCooldown(animator));
        // Restore ground movement
        agent.enabled = true;
        agent.Warp(user.transform.position);
    }

    IEnumerator DemonicRitualOnCooldown(Mangkukulam_AnimationManager animator)
    {
        animator.ChangeAnimationState("Mangkukulam_Stun_Start");
        yield return new WaitForSeconds(animator.GetAnimationLength("Mangkukulam_Stun_Start"));

        Mangkukulam.instance.isStunned = true;
        Mangkukulam_AttackManager.Instance.isAttacking = false;
        Mangkukulam_AttackManager.Instance.canAttack = true;
        Mangkukulam_AttackManager.Instance.castingCurrentAbility = null;
        Mangkukulam.instance.isVulnerable = true;
        runningRoutine = null;
        yield return new WaitForSeconds(Cooldown());
        canBeUsed = true;
    }

    void SetRotation(GameObject user, Transform target)
    {
        Vector3 dir = target.position - user.transform.position;
        dir.y = 0f;

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
}
