using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class Mangkukulam_AnimationManager : MonoBehaviour
{
    private Animator animator;
    private Mangkukulam_AttackManager atkMngr;
    private Mangkukulam mangkukulam;
    private Mangkukulam_MovementBehaviour movement;
    private string currentAnimationState = "Mangkukulam_Idle";
    public bool isHit = false;

    private void Start()
    {
        mangkukulam = GetComponent<Mangkukulam>();
        animator = GetComponent<Animator>();
        atkMngr = GetComponent<Mangkukulam_AttackManager>();
        movement = GetComponent<Mangkukulam_MovementBehaviour>();
    }

    public void HandleAnimations()
    {
        if (mangkukulam.isDead)
        {
            ChangeAnimationState("Mangkukulam_Stun_Idle"); //This will act as the dead animation
            return;
        }
        if (isHit) return;

        if (mangkukulam.isStunned)
            ChangeAnimationState("Mangkukulam_Stun_Idle");

        if (atkMngr.isAttacking && !mangkukulam.isStunned)
        {
            for (int i = 0; i < atkMngr.abilityList.Count; i++)
            {
                if (atkMngr.castingCurrentAbility == atkMngr.abilityList[i])
                {
                    if (!(atkMngr.abilityList[i] is PotionBlitz_MangkukulamAbility) &&
                        !(atkMngr.abilityList[i] is DemonicRitual_MangkukulamAbility))
                    {
                        ChangeAnimationState($"Mangkukulam_Skill_{i + 1}_{atkMngr.abilityList[i].name}");
                    }
                }
            }
        }
    }

    public float GetAnimationLength(string animName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
    }
    public void GetHit()
    {
        isHit = true;
        int hitIndex;
        string hitAnim;
        if (mangkukulam.isStunned)
        {
            hitIndex = Random.Range(1, 3);
            hitAnim = $"Mangkukulam_StunnedHit_{hitIndex}";
        }
        else
        {
            hitIndex = Random.Range(1, 3);
            hitAnim = $"Mangkukulam_Hit_{hitIndex}";
        }
        
        ChangeAnimationState(hitAnim);
        Invoke("GetHitFalse", 1f);
    }

    void GetHitFalse()
    {
        isHit = false;
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        if (currentAnimationState == newAnimationState) return;

        float transitionDuration = GetTransitionDuration(currentAnimationState, newAnimationState);
        animator.CrossFade(newAnimationState, transitionDuration);
        currentAnimationState = newAnimationState;
    }

    private float GetTransitionDuration(string from, string to)
    {
        //Animation to Using Skills
        if (from == "Mangkukulam_Idle" && to.StartsWith("Mangkukulam_Skill")) return 0.1f;
        if (from == "Mangkukulam_Angry_Walk" && to.StartsWith("Mangkukulam_Skill")) return 0.1f;
        if (from == "Mangkukulam_Landing" && to.StartsWith("Mangkukulam_Skill")) return 1f;

        //From Using Skills to Idle
        if (from.StartsWith("Mangkukulam_Skill") && to == "Mangkukulam_Idle") return 0.1f;

        //From Using Skills to Moving
        if (from.StartsWith("Mangkukulam_Skill") && to == "Mangkukulam_Angry_Walk") return 0.1f;

        //From Any animation to Stunned
        if (from == "Demonic Ritual_Idle" && to == "Mangkukulam_Stun_Start") return 0;
        if (to == "Mangkukulam_Stun_Start" || to == "Mangkukulam_Stun_Idle") return 0.15f;

        //From Any Animation to Start Flying
        if (to == "Mangkukulam_StartFlying") return 0.15f;


        //Being hit while Stunned
        if (from == "Mangkukulam_Stun_Idle" && to.StartsWith("Mangkukulam_StunnedHit")) return 0.15f;
        if (from.StartsWith("Mangkukulam_StunnedHit") && to == "Mangkukulam_Stun_Idle") return 0.15f;

        //Being hit while Walking or Idle
        if (from == "Mangkukulam_Idle" && to.StartsWith("Mangkukulam_Hit")) return 0.15f;
        if (from == "Mangkukulam_Angry_Walk" && to.StartsWith("Mangkukulam_Hit")) return 0.15f;


        //While Idle
        if (from.StartsWith("Mangkukulam_Hit") && to == "Mangkukulam_Idle") return 0.15f;
        if (from.StartsWith("Mangkukulam_Hit") && to == "Mangkukulam_Angry_Walk") return 0.15f;

        //When Defeated
        //if (to == "Mangkukulam_Stun_Idle") return 0.3f; //Death Handler since no defeat animation
        //if (from == "Nuno_Death" && to == "Nuno_Death_to_Stand") return 0.25f;

        return 0.2f;
    }
}