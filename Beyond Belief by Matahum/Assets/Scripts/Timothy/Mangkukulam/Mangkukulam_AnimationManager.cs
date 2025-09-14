using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mangkukulam_AnimationManager : MonoBehaviour
{
    private Animator animator;
    private Mangkukulam_AttackManager atkMngr;
    private Mangkukulam mangkukulam;
    private string currentAnimationState = "Mangkukulam_Idle";
    public bool isHit = false;

    private void Start()
    {
        mangkukulam = GetComponent<Mangkukulam>();
        animator = GetComponent<Animator>();
        atkMngr = GetComponent<Mangkukulam_AttackManager>();
    }

    public void HandleAnimations()
    {
        if (isHit) return;

        if (mangkukulam.isStunned)
        {
            ChangeAnimationState("Nuno_Down_Idle");
        }

        if (!atkMngr.isAttacking && !mangkukulam.isStunned)
        {
            ChangeAnimationState("Nuno_Idle");
        }
        else if (atkMngr.isAttacking && !mangkukulam.isStunned)
        {
            for (int i = 0; i < atkMngr.abilityList.Count; i++)
            {
                if (atkMngr.castingCurrentAbility == atkMngr.abilityList[i])
                {
                    ChangeAnimationState($"Nuno_Skill_{i + 1}");
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
        //From Idle to Using Skills
        if (from == "Nuno_Idle" && to.StartsWith("Nuno_Skill")) return 0.1f;

        //From Using Skills to Idle
        if (from.StartsWith("Nuno_Skill") && to == "Nuno_Idle") return 0.1f;

        //From Any animation to Down
        if (to == "Nuno_Down" || to == "Nuno_Down_Idle") return 0.15f;

        //Being hit while down
        if (from == "Nuno_Down_Idle" && to.StartsWith("Nuno_GetHit")) return 0.15f;

        //While Idle
        if (from == "Nuno_GetHit_1" && to == "Nuno_Down_Idle") return 1f;
        if (from == "Nuno_GetHit_2" && to == "Nuno_Down_Idle") return 1f;

        if (from == "Nuno_Down_Idle" && to == "Nuno_Idle") return 0.5f;

        //When Defeated
        if (to == "Nuno_Death") return 0.3f;
        if (from == "Nuno_Death" && to == "Nuno_Death_to_Stand") return 0.25f;

        return 0.2f;
    }
}