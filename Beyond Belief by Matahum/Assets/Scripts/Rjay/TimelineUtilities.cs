using UnityEngine;

public class TimelineUtilities : MonoBehaviour
{

    public AudioClip cutsceneAudio;
    public AudioSource cutsceneAudioSource;
    public void LoadNextScene(int index)
    {
        Loader.Load(index);
    }

    public void PlayCutsceneAudio()
    {
        cutsceneAudioSource.PlayOneShot(cutsceneAudio);
    }
}
