using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;

public class Mangkukulam_AttackManager : MonoBehaviour
{
    public static Mangkukulam_AttackManager Instance;

    public List<Mangkukulam_Ability> abilityList = new List<Mangkukulam_Ability>();
    public Mangkukulam_Ability castingCurrentAbility;
    
    public bool isAttacking = false;
    public bool canAttack = true;
    public int stunDuration = 3;

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

        foreach (var ability in abilityList)
        {
            ability.Initialize();
        }
    }

    private void Update()
    {
        if (mangkukulam.IsDead()) return;
        if (mangkukulam.isFlying) return;
        if(!mangkukulam.isBattleStart) return;

        animator.HandleAnimations();

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
            yield return new WaitForSeconds(stunDuration);
            mangkukulam.isStunned = false;
        }

        // Random pre-attack delay
        float cooldown = Random.Range(1f, 2f);
        yield return new WaitForSeconds(cooldown);

        // Skip attacking if stunned during cooldown
        if (mangkukulam.isStunned)
        {
            canAttack = true;
            yield break;
        }

        int skillIndex = Random.Range(0, abilityList.Count);

        if (abilityList[skillIndex].CanBeUsed())
        {
            isAttacking = true;
            castingCurrentAbility = abilityList[skillIndex];

            abilityList[skillIndex].Activate(gameObject);
            mangkukulam.uiCanvas.currentCastingSkillTxt.text = $"Casting {abilityList[skillIndex].name} . . .";
        }
        else
        {
            mangkukulam.uiCanvas.currentCastingSkillTxt.text = $" ";
            canAttack = true;
        }
    }

}
