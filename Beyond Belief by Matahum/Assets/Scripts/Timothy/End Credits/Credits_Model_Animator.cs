using UnityEngine;
using System.Collections;
using System;

public class Credits_Model_Animator : MonoBehaviour
{
    public Animator animator;
    public DoAnimationWithDelay[] nextAnimation;
    public string currentAnimationState;
    public float delay;

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0); // Default: rotate around Y-axis
    public float rotationSpeed = 50f; // Degrees per second
    public Space rotationSpace = Space.Self; // Choose between Local or World space

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime; // <-- ignore timeScale
        PlayAnimations();
    }

    private void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, rotationSpace);
    }

    public void PlayAnimations()
    {
        StartCoroutine(PlayPreviewCycle());
    }
    IEnumerator PlayPreviewCycle()
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < nextAnimation.Length; i++)
        {
            Debug.Log($"Playing {nextAnimation[i].animationName}");
            yield return PlayAndWait(nextAnimation[i]);
        }     
    }

    private IEnumerator PlayAndWait(DoAnimationWithDelay animName)
    {
        ChangeAnimationState(animName.animationName);
        yield return new WaitForSecondsRealtime(GetAnimationLength(animName.animationName) + animName.delay);
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

[Serializable]
public class DoAnimationWithDelay
{
    public string animationName;
    public float delay;
}