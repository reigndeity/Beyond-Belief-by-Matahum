using UnityEngine;
using System.Collections;
using DissolveExample;

[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Needle Barrage")]

public class NeedleBarrage : Mangkukulam_Ability
{
    public Coroutine currentCoroutine;
    public bool canBeUsed = true;
    public float cooldown;

    public GameObject needleBarrageHolderPrefab;

    public override bool CanBeUsed() => canBeUsed;
    public override float Cooldown() => cooldown;
    public override void Initialize() => canBeUsed = true;
    public override void Activate(GameObject user)
    {
        canBeUsed = false;
        CoroutineRunner.Instance.RunCoroutine(StartSpinning(user));
    }
    IEnumerator StartSpinning(GameObject user)
    {
        Mangkukulam_AnimationManager animator = user.GetComponent<Mangkukulam_AnimationManager>();
        animator.ChangeAnimationState("Mangkukulam_Skill_4_Needle Barrage");
        CoroutineRunner.Instance.RunCoroutine(SetRotation(user));
        yield return new WaitForSeconds(0.5f);
        Vector3 offset = new Vector3(0, 5, 0);

        // Pick a random Y rotation (0–360°)
        float randomize = Random.Range(0f, 360f);
        Quaternion randomizeRotation = Quaternion.Euler(0, randomize, 0);

        // Spawn with random rotation
        GameObject needleHolderPrefab = Instantiate(
            needleBarrageHolderPrefab, //prefab
            user.transform.parent.position + offset, //position
            randomizeRotation, //rotation
            user.transform.parent //parent
        );

        float animLength = user.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Needle Barrage animation length is: " + animLength);
        yield return new WaitForSeconds(animLength);

        Mangkukulam_AttackManager atkMngr = user.GetComponent<Mangkukulam_AttackManager>();
        atkMngr.isAttacking = false;
        atkMngr.canAttack = true;
        atkMngr.castingCurrentAbility = null;

        NeedleBarrageHolder holderScript = needleHolderPrefab.GetComponent<NeedleBarrageHolder>();
        CoroutineRunner.Instance.RunCoroutine(StartCooldown(holderScript));
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

    IEnumerator StartCooldown(NeedleBarrageHolder holderScript)
    {
        yield return new WaitForSeconds(Cooldown() + holderScript.spinDuration);
        canBeUsed = true;
    }
}
