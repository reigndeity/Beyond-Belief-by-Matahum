using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Mangkukulam_AttackManager : MonoBehaviour
{
    public static Mangkukulam_AttackManager Instance;

    public List<Mangkukulam_Ability> abilityList = new List<Mangkukulam_Ability>();
    public Mangkukulam_Ability castingCurrentAbility;
    
    [HideInInspector] public bool isAttacking = false;
    private bool canAttack = true;
    public int stunAmount = 3;

    private Player player;
    private Animator anim;
    private Mangkukulam mangkukulam;
    private Mangkukulam_AnimationManager animator;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        player = FindFirstObjectByType<Player>();
        mangkukulam = GetComponent<Mangkukulam>();
        animator = GetComponent<Mangkukulam_AnimationManager>();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (mangkukulam.IsDead()) return;

        //animator.HandleAnimations();

        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(SkillRandomizer());
        }
    }

    private IEnumerator SkillRandomizer()
    {
        // If stunned, pause attack logic
        if (mangkukulam.isStunned)
        {
            Debug.Log("Mangkukulam is Stunned");

            yield return new WaitForSeconds(stunAmount);
            mangkukulam.isStunned = false;
        }

        // Random pre-attack delay
        float cooldown = Random.Range(2f, 5f);
        yield return new WaitForSeconds(cooldown);

        // Skip attacking if stunned during cooldown
        if (mangkukulam.isStunned)
        {
            Debug.Log("Attack interrupted because of stun");
            canAttack = true;
            yield break;
        }

        int skillIndex = Random.Range(0, abilityList.Count + 1);

        if (skillIndex < abilityList.Count && abilityList[skillIndex].CanBeUsed())
        {
            isAttacking = true;
            castingCurrentAbility = abilityList[skillIndex];

            abilityList[skillIndex].Activate(gameObject);
            Debug.Log($"Attacking with {abilityList[skillIndex].name}");

            // ✅ Wait until animator is actually in the right state
            /*string expectedState = $"Mangkukulam_Skill_{skillIndex + 1}";
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(expectedState));*/

            // ✅ Now get the correct animation length
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            //Debug.Log($"Playing {expectedState} for {animLength} seconds");

            yield return new WaitForSeconds(5);//animLength);
        }
        else
        {
            Debug.Log("Mangkukulam is not attacking");
        }

        // ✅ Allow next attack
        isAttacking = false;
        canAttack = true;
        castingCurrentAbility = null;
    }

}
