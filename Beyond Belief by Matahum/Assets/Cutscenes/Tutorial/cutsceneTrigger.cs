using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(Collider))]
public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public TimelineAsset timeline;   // Timeline to play
    public bool playOnce = true;     // If true, this trigger can only play once

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnce && hasPlayed) return;

        if (CutsceneManager.Instance != null && timeline != null)
        {
            CutsceneManager.Instance.PlayCutscene(timeline);
            hasPlayed = true;
        }
    }
}
