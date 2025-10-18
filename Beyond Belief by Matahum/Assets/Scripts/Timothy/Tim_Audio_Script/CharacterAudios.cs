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

    public void SFX(int index)
    {
        if (clips == null || clips.Length <= index || clips[index].clip == null)
            return;

        // Get base clip volume (0–1 preferred)
        float clipVolume = Mathf.Clamp01(clips[index].volume);

        // Scale it by wrapper's max volume range
        float finalVolume = clipVolume * (wrapper != null ? wrapper.maxVolume : 1f);

        audioSource.PlayOneShot(clips[index].clip, finalVolume);
    }
}

[Serializable]
public class CharacterAudioClip
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}
