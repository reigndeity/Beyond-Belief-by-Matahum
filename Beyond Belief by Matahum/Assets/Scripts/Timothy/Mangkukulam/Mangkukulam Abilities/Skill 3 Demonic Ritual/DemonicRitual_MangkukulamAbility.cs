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

    private void OnEnable()
    {
        canBeUsed = true;
    }

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
        canBeUsed = false;
        NavMeshAgent agent = user.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

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

            Mangkukulam.instance.isVulnerable = false;
            yield return null;
        }

        // Reached center
        Mangkukulam.instance.isVulnerable = true;

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

        // --- Ritual loop indefinitely ---
        while (holder != null) // keep looping until candles destroyed
        {
            RitualAnimation(user); // here you can trigger anim, particles, etc.
            yield return null;
        }
        Debug.Log("Ritual has ended");
        // Ritual ended externally (candles destroyed)
        CoroutineRunner.Instance.RunCoroutine(DemonicRitualOnCooldown());
        // Restore ground movement
        agent.enabled = true;
        agent.Warp(user.transform.position);
    }

    IEnumerator DemonicRitualOnCooldown()
    {
        Mangkukulam.instance.isStunned = true;
        Mangkukulam_AttackManager.Instance.isAttacking = false;
        Mangkukulam_AttackManager.Instance.canAttack = true;
        Mangkukulam_AttackManager.Instance.castingCurrentAbility = null;
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

    private void RitualAnimation(GameObject user)
    {
        Debug.Log("Doing a ritual");
    }
}
