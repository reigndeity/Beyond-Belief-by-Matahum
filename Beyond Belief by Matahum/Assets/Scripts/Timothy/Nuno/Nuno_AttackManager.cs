using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Nuno_AttackManager : MonoBehaviour
{
    public static Nuno_AttackManager Instance;

    public List<Nuno_Ability> abilityList = new List<Nuno_Ability>();
    public bool isBattleStart = false;
    private bool isAttacking = false;

    [Header("Skill 1 Properties")]
    public Transform[] bulletPosition;
    public Transform bulletHolder;

    private Player player;
    private Animator anim;
    private Nuno nuno;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        nuno = GetComponent<Nuno>();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isBattleStart) return;

        // Get direction to player but ignore Y axis
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f; // prevents looking up/down

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

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
        float animLength = stateInfo.length;

        float elapse = 0;
        float duration = animLength;
        while (elapse < duration && !nuno.isStunned)
        {

        }*/

        yield return new WaitForSeconds(5);
        isAttacking = false;
    }
}
