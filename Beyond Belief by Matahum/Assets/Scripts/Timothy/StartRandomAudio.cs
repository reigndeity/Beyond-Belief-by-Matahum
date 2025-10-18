using UnityEngine;

public class StartRandomAudio : MonoBehaviour
{
    AudioSource audioSource;
    AudioWrapper wrapper;
    public CharacterAudioClip[] clips;
    public bool randomPitch;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        int randomizer = Random.Range(0, clips.Length);
        audioSource.clip = clips[randomizer].clip;
        audioSource.volume = AdjustVolume(clips[randomizer]);
        audioSource.pitch = ChangePitch();
        audioSource.Play();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
        else
        {
            if (!audioSource.isPlaying && audioSource.clip != null)
                audioSource.UnPause();
        }
    }

    private float AdjustVolume(CharacterAudioClip clip)
    {
        if (clip == null)
            return 1;

        // Get base clip volume (0–1 preferred)
        float clipVolume = Mathf.Clamp01(clip.volume);

        // Scale it by wrapper's max volume range
        float finalVolume = clipVolume * (wrapper != null ? wrapper.maxVolume : 1f);

        return finalVolume;
    }

    public float ChangePitch()
    {
        if (randomPitch)
            return Random.Range(0.8f, 1.2f);
        else
            return 1;
    }
}
