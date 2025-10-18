using System;
using UnityEngine;

public class CharacterAudios : MonoBehaviour
{
    public AudioSource audioSource;
    [Tooltip("This is for specific sfx for one action")]
    public CharacterAudioClip[] clips;
    [Tooltip("This is for randomized sfx for one action")]
    public RandomAudioClip[] clipGroup;

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
    /// <summary>
    /// This plays PlayOneShot. AudioSource will not be affected.
    /// </summary>
    public void SFX(int index)
    {
        if (clips == null || index < 0 || index >= clips.Length || clips[index].clip == null)
            return;

        float finalVolume = AdjustVolume(clips[index]);
        audioSource.PlayOneShot(clips[index].clip, finalVolume);
    }
    /// <summary>
    /// This plays a random PlayOneShot. AudioSource will not be affected.
    /// </summary>
    public void RandomOneShotSFX(int index)
    {
        if (clipGroup == null || index < 0 || index >= clipGroup.Length)
            return;

        if (clipGroup[index].clips == null || clipGroup[index].clips.Length == 0)
            return;

        int randomIndex = UnityEngine.Random.Range(0, clipGroup[index].clips.Length);
        var chosen = clipGroup[index].clips[randomIndex];
        if (chosen.clip == null) return;

        float finalVolume = AdjustVolume(chosen);
        audioSource.PlayOneShot(chosen.clip, finalVolume);
    }
    /// <summary>
    /// This plays Play() that can be stopped. This will add the clip to the AudioSource and adjust the volume.
    /// </summary>
    public void SFXNormalPlay(int index)
    {
        float finalVolume = AdjustVolume(clips[index]);

        audioSource.volume = finalVolume;
        audioSource.clip = clips[index].clip;
        audioSource.Play();
    }
    /// <summary>
    /// This plays a random Play() that can be stopped. This will add the clip to the AudioSource and adjust the volume.
    /// </summary>
    public void RandomPlaySFX(int index)
    {
        int randomIndex = UnityEngine.Random.Range(0, clipGroup[index].clips.Length);
        float finalVolume = AdjustVolume(clipGroup[index].clips[randomIndex]);

        audioSource.volume = finalVolume;
        audioSource.clip = clipGroup[index].clips[randomIndex].clip;
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

[Serializable]
public class RandomAudioClip
{
    public string clipGroupName;
    public CharacterAudioClip[] clips;
}
