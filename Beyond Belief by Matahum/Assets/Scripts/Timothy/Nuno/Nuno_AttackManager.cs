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
    public bool isStunned = false;

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
        // If stunned, pause attack logic
        if (isStunned)
        {
            Debug.Log("Nuno is Stunned");
            yield return new WaitForSeconds(5f);
            isStunned = false;
        }

        // Random pre-attack delay
        float cooldown = Random.Range(1f, 3f);
        yield return new WaitForSeconds(cooldown);

        // Skip attacking if stunned during cooldown
        if (isStunned)
        {
            Debug.Log("Attack interrupted because of stun");
            isAttacking = false;
            yield break;
        }

        // Randomly pick a skill or idle
        int skillIndex = Random.Range(0, abilityList.Count + 1);
        if (skillIndex < abilityList.Count)
        {
            abilityList[skillIndex].Activate();
            Debug.Log($"Attacking with {abilityList[skillIndex].name}");

            // ✅ Wait for attack to "finish"
            float attackDuration = 3f; // could be animation length instead
            float elapsed = 0f;
            while (elapsed < attackDuration)
            {
                if (isStunned) // interrupt attack if stunned mid-cast
                {
                    Debug.Log("Attack interrupted by stun");
                    yield return new WaitForSeconds(5f); // stun duration
                    isStunned = false;
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            Debug.Log("Nuno is Idle");
        }

        // ✅ Only allow next attack after finishing the full cycle
        isAttacking = false;
    }

}
