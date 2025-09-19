using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Voodoo Summoning")]
public class VoodooSummoning_MangkukulamAbility : Mangkukulam_Ability
{
    public GameObject layagVoodooPrefab;
    public int voodooCount;
    public List<Enemy> spawnedVoodoos = new List<Enemy>();
    public float radius = 5f;

    [Header("Cooldown")]
    public float cooldown;

    public override float Cooldown() => cooldown;

    public bool canBeUsed = true;
    public override bool CanBeUsed() => canBeUsed;
    public override void Initialize() => canBeUsed = true;

    private void OnDisable() => spawnedVoodoos.Clear();
    public override void Activate(GameObject user)
    {
        canBeUsed = false;
        CoroutineRunner.Instance.RunCoroutine(SummonVoodoo(user));
    }

    IEnumerator SummonVoodoo(GameObject user)
    {
        Mangkukulam_AnimationManager animator = user.GetComponent<Mangkukulam_AnimationManager>();
        animator.ChangeAnimationState("Mangkukulam_Skill_2_Voodoo Summoning");
        CoroutineRunner.Instance.RunCoroutine(SetRotation(user));
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < voodooCount; i++)
        {
            float angle = i * Mathf.PI * 2f / voodooCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            // Base position around the caster
            Vector3 basePos = user.transform.position + offset;

            // Try to find the nearest point on the NavMesh
            if (NavMesh.SamplePosition(basePos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                // Spawn at the navmesh location
                Vector3 navMeshOffset = new Vector3(0, 0.11f, 0);
                Enemy enemy = Instantiate(layagVoodooPrefab, hit.position - navMeshOffset, Quaternion.identity, user.transform.parent).GetComponent<Enemy>();
                spawnedVoodoos.Add(enemy);

                enemy.OnDeath += () => OnEnemyDeath(enemy);
            }
        }

        yield return new WaitForSeconds(animator.GetAnimationLength("Mangkukulam_Skill_2_Voodoo Summoning"));

        Mangkukulam_AttackManager atkMngr = user.GetComponent<Mangkukulam_AttackManager>();
        atkMngr.isAttacking = false;
        atkMngr.canAttack = true;
        atkMngr.castingCurrentAbility = null;
        Mangkukulam.instance.uiCanvas.currentCastingSkillTxt.text = $" ";
    }

    IEnumerator SetRotation(GameObject user)
    {
        Transform target = GameObject.FindGameObjectWithTag("Player").transform;
        // Get the direction to target, but only in XZ plane
        Vector3 dir = target.position - user.transform.position;
        dir.y = 0f; // remove vertical difference

        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;

            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                user.transform.rotation = Quaternion.Lerp(
                    user.transform.rotation,
                    targetRot,
                    15 * Time.deltaTime
                );
            }

            yield return null;
        }
    }

    void OnEnemyDeath(Enemy enemy)
    {
        spawnedVoodoos.Remove(enemy);

        if (spawnedVoodoos.Count == 0)
            CoroutineRunner.Instance.RunCoroutine(StartCooldown());
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(Cooldown());
        canBeUsed = true;
    }
}
