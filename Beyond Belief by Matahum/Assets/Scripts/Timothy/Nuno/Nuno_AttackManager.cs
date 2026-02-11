//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;

//public class Nuno_AttackManager : MonoBehaviour
//{
//    public static Nuno_AttackManager Instance;

//    public float rotationSpeed = 5f;
//    public List<Nuno_Ability> abilityList = new List<Nuno_Ability>();
//    public Nuno_Ability castingCurrentAbility;
//    public bool isBattleStart = false;
//    [HideInInspector] public bool isAttacking = false;
//    private bool canAttack = true;
//    public bool isStunned = false;
//    public int stunAmount = 3;

//    private Player player;
//    private Animator anim;
//    private Nuno nuno;
//    private Nuno_Animations animator;
//    [HideInInspector] public Boss_UICanvas uiCanvas;

//    private void Awake()
//    {
//        Instance = this;
//    }
//    private void Start()
//    {
//        anim = GetComponent<Animator>();
//        player = FindFirstObjectByType<Player>();
//        nuno = GetComponent<Nuno>();
//        animator = GetComponent<Nuno_Animations>();
//        uiCanvas = GetComponent<Boss_UICanvas>();

//        uiCanvas.currentCastingSkillTxt.text = " ";
//    }

//    private Coroutine attackRoutine;

//    private void Update()
//    {
//        if (nuno.isDead) return;

//        animator.HandleAnimations();

//        if (!isBattleStart)
//        {
//            Invoke("BattleStart", 2);
//            return;
//        }

//        if (isStunned)
//        {
//            uiCanvas.currentCastingSkillTxt.text = "Nuno is Stunned!";
//            foreach (var ability in abilityList)
//                ability.Deactivate();

//            // 🛑 Stop attacking coroutine if running
//            if (attackRoutine != null)
//            {
//                StopCoroutine(attackRoutine);
//                attackRoutine = null;
//            }
//            return; // skip rest of Update while stunned
//        }

//        // Look at player
//        Vector3 direction = player.transform.position - transform.position;
//        direction.y = 0f;
//        if (direction != Vector3.zero)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(direction);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//        }

//        // Start attack loop if allowed
//        if (canAttack && attackRoutine == null)
//        {
//            attackRoutine = StartCoroutine(SkillRandomizer());
//        }
//    }


//    void BattleStart()
//    {
//        isBattleStart = true;
//    }

//    private IEnumerator SkillRandomizer()
//    {
//        // If stunned, pause attack logic
//        /*if (isStunned)
//        {
//            Debug.Log("Nuno is Stunned");
//            yield return new WaitForSeconds(stunAmount);
//            uiCanvas.currentCastingSkillTxt.text = " ";
//            isStunned = false;
//        }*/

//        // Random pre-attack delay
//        float cooldown = Random.Range(1f, 2f);
//        yield return new WaitForSeconds(cooldown);

//        // Skip attacking if stunned during cooldown
//        /*if (isStunned)
//        {
//            Debug.Log("Attack interrupted because of stun");
//            canAttack = true;
//            uiCanvas.currentCastingSkillTxt.text = " ";
//            yield break;
//        }*/

//        int skillIndex = Random.Range(0, abilityList.Count);

//        // ⛔ If shield picked but Nuno is not vulnerable, retry
//        if (skillIndex == 4 && !nuno.isVulnerable)
//        {
//            Debug.Log("Shield skipped (Nuno still invulnerable), retrying...");
//            canAttack = true;
//            yield break; // exit coroutine early → Update() will restart SkillRandomizer next frame
//        }

//        if (abilityList[skillIndex].CanBeUsed())
//        {
//            isAttacking = true;
//            castingCurrentAbility = abilityList[skillIndex];

//            abilityList[skillIndex].Activate();
//            uiCanvas.currentCastingSkillTxt.text = $"Casting {abilityList[skillIndex].name} . . .";

//            Debug.Log($"Attacking with {abilityList[skillIndex].name}");

//            // ✅ Wait until animator is actually in the right state
//            string expectedState = $"Nuno_Skill_{skillIndex + 1}";
//            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(expectedState));

//            // ✅ Now get the correct animation length
//            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
//            Debug.Log($"Playing {expectedState} for {animLength} seconds");

//            yield return new WaitForSeconds(animLength);
//        }

//        // ✅ Allow next attack
//        isAttacking = false;
//        canAttack = true;
//        castingCurrentAbility = null;
//        uiCanvas.currentCastingSkillTxt.text = " ";
//    }

//    public void ApplyStun(float duration)
//    {
//        if (!isStunned)
//            StartCoroutine(StunRoutine(duration));
//    }

//    private IEnumerator StunRoutine(float duration)
//    {
//        isStunned = true;
//        uiCanvas.currentCastingSkillTxt.text = "Nuno is Stunned!";

//        yield return new WaitForSeconds(duration);

//        isStunned = false;
//        uiCanvas.currentCastingSkillTxt.text = " ";
//        canAttack = true; // resume attacks
//    }

//}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public float stunDuration = 3;
    public GameObject stunVFX;

    private Player player;
    private Animator anim;
    private Nuno nuno;
    private Nuno_Animations animator;
    [HideInInspector] public Boss_UICanvas uiCanvas;

    private Coroutine attackRoutine;

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
        if (nuno.isDead) return;

        animator.HandleAnimations();

        if (!isBattleStart)
        {
            Invoke("BattleStart", 2);
            return;
        }

        if (isStunned)
        {
            // Face player only when not stunned
            return;
        }

        // Rotate towards player
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Start attack loop
        if (canAttack && attackRoutine == null)
        {
            attackRoutine = StartCoroutine(SkillRandomizer());
        }
    }

    void BattleStart()
    {
        isBattleStart = true;
    }

    // 🔥 Refactored attack loop (no stun checks here anymore!)
    private IEnumerator SkillRandomizer()
    {
        canAttack = false;

        // Random pre-attack delay
        float cooldown = Random.Range(1f, 2f);
        yield return new WaitForSeconds(cooldown);

        int skillIndex = Random.Range(0, abilityList.Count);

        // Skip shield if Nuno not vulnerable
        if (skillIndex == 4 && !nuno.isVulnerable)
        {
            canAttack = true;
            attackRoutine = null;
            yield break;
        }

        if (abilityList[skillIndex].CanBeUsed())
        {
            isAttacking = true;
            castingCurrentAbility = abilityList[skillIndex];

            abilityList[skillIndex].Activate();
            uiCanvas.currentCastingSkillTxt.text = $"Casting {abilityList[skillIndex].name} . . .";

            string expectedState = $"Nuno_Skill_{skillIndex + 1}";
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(expectedState));

            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
        }

        isAttacking = false;
        castingCurrentAbility = null;
        uiCanvas.currentCastingSkillTxt.text = " ";
        canAttack = true;
        attackRoutine = null; // free slot for next Update loop
    }

    // 🎯 Unified stun logic
    public void ApplyStun()
    {
        if (!isStunned)
            StartCoroutine(StunRoutine(stunDuration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        uiCanvas.currentCastingSkillTxt.text = "Nuno is Stunned!";

        if (stunVFX != null)
        {
            stunVFX.SetActive(true);
        }

        // Cancel any ongoing attack immediately
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        isAttacking = false;
        castingCurrentAbility = null;

        // Cancel any active abilities
        foreach (var ability in abilityList)
            ability.Deactivate();

        yield return new WaitForSeconds(duration);

        isStunned = false;
        uiCanvas.currentCastingSkillTxt.text = " ";
        canAttack = true; // allow next attack

        if (stunVFX != null)
        {
            stunVFX.SetActive(false);
        }
    }
}
