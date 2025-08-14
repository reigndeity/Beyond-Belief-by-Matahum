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
}
