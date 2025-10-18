using System;
using UnityEngine;

public class CharacterAudios : MonoBehaviour
{
    public AudioSource audioSource;
    public CharacterAudioClip[] clips;

    public AudioWrapper wrapper;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (wrapper == null)
            wrapper = GetComponent<AudioWrapper>();
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

    private float AdjustVolume(int index)
    {
        if (clips == null || clips.Length <= index || clips[index].clip == null)
            return 1;

        // Get base clip volume (0–1 preferred)
        float clipVolume = Mathf.Clamp01(clips[index].volume);

        // Scale it by wrapper's max volume range
        float finalVolume = clipVolume * (wrapper != null ? wrapper.maxVolume : 1f);

        return finalVolume;
    }
    /// <summary>
    /// This plays PlayOneShot. AudioSource will not be affected.
    /// </summary>
    public void SFX(int index)
    {
        float finalVolume = AdjustVolume(index);

        audioSource.PlayOneShot(clips[index].clip, finalVolume);
    }
    /// <summary>
    /// This plays Play() that can be stopped. This will add the clip to the AudioSource and adjust the volume.
    /// </summary>
    public void SFXNormalPlay(int index)
    {
        float finalVolume = AdjustVolume(index);

        audioSource.volume = finalVolume;
        audioSource.clip = clips[index].clip;
        audioSource.Play();
    }

    public void StopAudioSource()
    {
        audioSource.Stop();
    }
}

[Serializable]
public class CharacterAudioClip
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}
