using UnityEngine;

public class RebultoSFX : MonoBehaviour
{
    private AudioSource rebultoSource;

    [Header("SFX Clips")]
    public AudioClip rebultoUnlockClip;
    public AudioClip rebultoUnlockedLoopClip;

    private void Awake()
    {
        rebultoSource = GetComponent<AudioSource>();
        rebultoSource.playOnAwake = false;
        rebultoSource.loop = false; // default
    }

    // Called when unlocking happens for the FIRST time
    public void PlayUnlockSFX()
    {
        rebultoSource.loop = false;
        rebultoSource.clip = rebultoUnlockClip;
        rebultoSource.Play();
    }

    // Called whenever the statue is already unlocked (Start() or reloading)
    public void PlayUnlockedLoop()
    {
        rebultoSource.loop = true;
        rebultoSource.clip = rebultoUnlockedLoopClip;
        rebultoSource.Play();
    }
}
