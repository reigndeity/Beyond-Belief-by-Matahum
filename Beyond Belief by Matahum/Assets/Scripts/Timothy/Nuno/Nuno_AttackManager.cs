using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Nuno_AttackManager : MonoBehaviour
{
    public static Nuno_AttackManager Instance;

    public float rotationSpeed = 5f;
    public List<Nuno_Ability> abilityList = new List<Nuno_Ability>();
    public Nuno_Ability castingCurrentAbility;
    public bool isBattleStart = false;
    [HideInInspector] public bool isAttacking = false;
    private bool canAttack = true;
    public bool isStunned = false;
    public int stunAmount = 3;    

    private Player player;
    private Animator anim;
    private Nuno nuno;
    private Nuno_Animations animator;
    [HideInInspector] public Boss_UICanvas uiCanvas;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        player = FindFirstObjectByType<Player>();
        nuno = GetComponent<Nuno>();
        animator = GetComponent<Nuno_Animations>();
        uiCanvas = GetComponent<Boss_UICanvas>();

        uiCanvas.currentCastingSkillTxt.text = " ";
    }

    private void Update()
    {
        if(nuno.isDead) return;

        animator.HandleAnimations();

        if (!isBattleStart)
        {
            Invoke("BattleStart", 2);
            return;
        }

        if (isStunned)
        {
            uiCanvas.currentCastingSkillTxt.text = "Nuno is Stunned!";
            foreach (var ability in abilityList)
            {
                ability.Deactivate();
            }
        }
        // Get direction to player but ignore Y axis
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f; // prevents looking up/down

        if (direction != Vector3.zero && !isStunned)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(SkillRandomizer());
        }
    }

    void BattleStart()
    {
        isBattleStart = true;
    }

    private IEnumerator SkillRandomizer()
    {
        // If stunned, pause attack logic
        if (isStunned)
        {
            Debug.Log("Nuno is Stunned");
            yield return new WaitForSeconds(stunAmount);
            uiCanvas.currentCastingSkillTxt.text = " ";
            isStunned = false;
        }

        // Random pre-attack delay
        float cooldown = Random.Range(1f, 2f);
        yield return new WaitForSeconds(cooldown);

        // Skip attacking if stunned during cooldown
        if (isStunned)
        {
            Debug.Log("Attack interrupted because of stun");
            canAttack = true;
            uiCanvas.currentCastingSkillTxt.text = " ";
            yield break;
        }

        int skillIndex = Random.Range(0, abilityList.Count);

        // ⛔ If shield picked but Nuno is not vulnerable, retry
        if (skillIndex == 4 && !nuno.isVulnerable)
        {
            Debug.Log("Shield skipped (Nuno still invulnerable), retrying...");
            canAttack = true;
            yield break; // exit coroutine early → Update() will restart SkillRandomizer next frame
        }

        if (skillIndex < abilityList.Count)
        {
            isAttacking = true;
            castingCurrentAbility = abilityList[skillIndex];

            abilityList[skillIndex].Activate();
            uiCanvas.currentCastingSkillTxt.text = $"Casting {abilityList[skillIndex].name} . . .";

            Debug.Log($"Attacking with {abilityList[skillIndex].name}");

            // ✅ Wait until animator is actually in the right state
            string expectedState = $"Nuno_Skill_{skillIndex + 1}";
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(expectedState));

            // ✅ Now get the correct animation length
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log($"Playing {expectedState} for {animLength} seconds");

            yield return new WaitForSeconds(animLength);
        }

        // ✅ Allow next attack
        isAttacking = false;
        canAttack = true;
        castingCurrentAbility = null;
        uiCanvas.currentCastingSkillTxt.text = " ";
    }

}
