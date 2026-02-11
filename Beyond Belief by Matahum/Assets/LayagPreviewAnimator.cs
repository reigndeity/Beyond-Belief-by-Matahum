using UnityEngine;
using System.Collections;

public class LayagPreviewAnimator : MonoBehaviour
{
    public Animator animator;
    private string currentAnimationState;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime; // <-- ignore timeScale
        ChangeAnimationState("player_idle_1");
    }

    public void PlayAnimations()
    {
        StartCoroutine(PlayPreviewCycle());
    }
    IEnumerator PlayPreviewCycle()
    {
        while (true)
        {
            int randomizer = Random.Range(1, 4);
            Debug.Log($"player_idle_{randomizer}");
            yield return PlayAndWait($"player_idle_{randomizer}");
        }
    }

    private IEnumerator PlayAndWait(string animName)
    {
        ChangeAnimationState(animName);
        yield return new WaitForSecondsRealtime(GetAnimationLength(animName));
        ChangeAnimationState("player_idle_1");
        yield return new WaitForSecondsRealtime(GetAnimationLength("player_idle_1") * 2);
    }


    private float GetAnimationLength(string animName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;
        animator.CrossFade(newState, 0.1f);
        currentAnimationState = newState;
    }
}
