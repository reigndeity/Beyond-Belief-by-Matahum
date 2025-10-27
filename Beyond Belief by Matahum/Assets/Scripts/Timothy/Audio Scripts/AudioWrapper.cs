using UnityEngine;
using System.Collections;

public class AudioWrapper : MonoBehaviour
{
    [Header("References")]
    private AudioSource audioSource;

    [Header("Volume Range (local volume range)")]
    private float minVolume = 0f;
    [Range(0f, 1f)] public float maxVolume = 0.25f;

    [Header("Type Settings")]
    public bool isSFX = true;
    public bool isAmbience = false;

    private bool isSubscribed = false;

    private void Awake()
    {
        // Auto-assign AudioSource if not set
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (AudioManager.instance != null)
        {
            SubscribeToAudioManager();
        }
        else
        {
            StartCoroutine(WaitForAudioManager());
        }
    }

    private IEnumerator WaitForAudioManager()
    {
        while (AudioManager.instance == null)
            yield return null;

        SubscribeToAudioManager();
    }

    private void SubscribeToAudioManager()
    {
        if (isSubscribed || AudioManager.instance == null)
            return;

        if (isSFX && !isAmbience)
        {
            AudioManager.instance.OnSFXVolumeChanged += ChangeVolume;
            ChangeVolume(AudioManager.instance.SFXvolumeValue);
        }
        else if (!isSFX && !isAmbience)
        {
            AudioManager.instance.OnBGMVolumeChanged += ChangeVolume;
            ChangeVolume(AudioManager.instance.BGMvolumeValue);
        }
        else if (isAmbience)
        {
            AudioManager.instance.OnAmbienceVolumeChanged += ChangeVolume;
            ChangeVolume(AudioManager.instance.ambienceVolumeValue);
        }

        isSubscribed = true;
    }

    private void OnEnable()
    {
        if (AudioManager.instance != null)
            SubscribeToAudioManager();
    }

    private void OnDisable()
    {
        if (!isSubscribed || AudioManager.instance == null)
            return;

        if (isSFX && !isAmbience)
            AudioManager.instance.OnSFXVolumeChanged -= ChangeVolume;
        else if(!isSFX && !isAmbience)
            AudioManager.instance.OnBGMVolumeChanged -= ChangeVolume;
        else if(isAmbience)
            AudioManager.instance.OnAmbienceVolumeChanged -= ChangeVolume;

        isSubscribed = false;
    }

    private void ChangeVolume(float newVolume)
    {
        if (audioSource == null) return;

        // Remap global volume (0–1) to this wrapper's local min–max range
        float scaledVolume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(newVolume));

        audioSource.volume = scaledVolume;
    }

    public float GetCurrentVolume()
    {
        if (audioSource == null) return 0f;
        return audioSource.volume;
    }

}
