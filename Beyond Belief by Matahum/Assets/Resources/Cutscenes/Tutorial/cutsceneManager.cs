using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("References")]
    public PlayableDirector director; // Drag in from scene

    [Header("Events")]
    public UnityEvent OnCutsceneStart;
    public UnityEvent OnCutsceneEnd;

    private bool isPlaying = false;
    private System.Action onCutsceneFinish;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartCutscene(TimelineAsset timeline)
    {
        PlayCutscene(timeline, null);
    }

    public void PlayCutscene(TimelineAsset timeline, System.Action onFinish = null)
    {
        if (isPlaying || timeline == null) return;

        isPlaying = true;
        onCutsceneFinish = onFinish;

        director.playableAsset = timeline;

        OnCutsceneStart?.Invoke();
        director.Play();
    }

    /// <summary>
    /// Ends the cutscene manually — call this from anywhere (e.g., after dialogue).
    /// </summary>
    public void EndCutscene()
    {
        if (!isPlaying) return;

        isPlaying = false;

        OnCutsceneEnd?.Invoke();
        onCutsceneFinish?.Invoke();

        Debug.Log("[CutsceneManager] EndCutscene called");

        if (director != null)
        director.Stop(); // force stop timeline so it stops controlling cameras

    }

    public void PauseTimeline()
    {
        if (director != null && director.state == PlayState.Playing)
            director.Pause();
    }

    public void ResumeTimeline()
    {
        if (director != null && director.state == PlayState.Paused)
            director.Play();
    }

    public void MovePlayerToTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("[CutsceneManager] Target transform is null.");
            return;
        }

        var player = GameObject.FindWithTag("Player"); // or keep a cached reference
        if (player != null)
        {
            player.transform.SetPositionAndRotation(target.position, target.rotation);
            Debug.Log($"[CutsceneManager] Player moved to {target.position}");
        }
        else
        {
            Debug.LogWarning("[CutsceneManager] No GameObject with tag 'Player' found.");
        }
    }

    public void PlayAnimation(Animator animator, string animationName)
    {
        if (animator == null)
        {
            Debug.LogWarning("[CutsceneManager] Animator is null.");
            return;
        }

        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("[CutsceneManager] Animation name is empty.");
            return;
        }

        animator.Play(animationName);
        Debug.Log($"[CutsceneManager] Playing animation '{animationName}' on {animator.name}");
    }

    [Header("Animation Control")]
    private Animator targetAnimator; // Assign the target animator in the inspector

    public void PlayAnimationOne(string animationName)
    {
        PlayAnimationWithoutSnapping(animationName);
    }

    public void PlayAnimationTwo(string animationName)
    {
        PlayAnimationWithoutSnapping(animationName);
    }

    private void PlayAnimationWithoutSnapping(string animationName)
    {
        if (targetAnimator == null)
        {
            Debug.LogWarning("[CutsceneManager] No target animator assigned.");
            return;
        }

        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("[CutsceneManager] Animation name is empty.");
            return;
        }

        // Smoothly transition to the animation without resetting position
        targetAnimator.CrossFade(animationName, 0.1f, -1, 0f);

        Debug.Log($"[CutsceneManager] Playing animation '{animationName}' on {targetAnimator.name} without snapping");
    }
    public void SetTargetAnimator(Animator newAnimator)
    {
        if (newAnimator == null)
        {
            Debug.LogWarning("[CutsceneManager] Attempted to set targetAnimator to null.");
            return;
        }

        targetAnimator = newAnimator;
        Debug.Log($"[CutsceneManager] Target animator set to {targetAnimator.name}");
    }
}
