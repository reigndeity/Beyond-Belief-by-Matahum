using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Nuno_AttackManager : MonoBehaviour
{
    public static Nuno_AttackManager Instance;

    public List<Nuno_Ability> abilityList = new List<Nuno_Ability>();
    public bool isBattleStart = false;
    public bool isAttacking = false;

    [Header("Skill 1 Properties")]
    public Transform[] bulletPosition;
    public Transform bulletHolder;

    private Player player;
    private Animator anim;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isBattleStart) return;

        // Always face the player
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(SkillRandomizer());
        }
    }

    private IEnumerator SkillRandomizer()
    {
        // Random cooldown before skill
        float cooldown = Random.Range(1f, 3f);
        yield return new WaitForSeconds(cooldown);

        // Randomly pick a skill or skip
        int skillIndex = Random.Range(0, abilityList.Count + 1); // inclusive upper bound
        if (skillIndex < abilityList.Count)
        {
            abilityList[skillIndex].Activate();
            Debug.Log($"Attacking with {abilityList[skillIndex].name}");
        }
        else
        {
            Debug.Log($"Still Idle");
            isAttacking = false;
            yield break;
        }

        //Base the timing for the next attack based on the animation
        /*AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animLength = stateInfo.length;*/
        yield return new WaitForSeconds(5);
        isAttacking = false;
    }
}
