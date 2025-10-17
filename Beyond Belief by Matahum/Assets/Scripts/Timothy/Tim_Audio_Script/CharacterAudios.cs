using System;
using UnityEngine;

public class CharacterAudios : MonoBehaviour
{
    public AudioSource audioSource;
    public CharacterAudioClip[] clips;

    private void Start()
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    private void SFX(int index)
    {
        audioSource.PlayOneShot(clips[index].clip, clips[index].volume);
    }
}

[Serializable]
public class CharacterAudioClip
{
    public AudioClip clip;
    public float volume;
}