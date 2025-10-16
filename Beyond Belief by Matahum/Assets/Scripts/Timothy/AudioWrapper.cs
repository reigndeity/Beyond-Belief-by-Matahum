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

        if (isSFX)
        {
            AudioManager.instance.OnSFXVolumeChanged += ChangeVolume;
            ChangeVolume(AudioManager.instance.SFXvolumeValue);
        }
        else
        {
            AudioManager.instance.OnBGMVolumeChanged += ChangeVolume;
            ChangeVolume(AudioManager.instance.BGMvolumeValue);
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

        if (isSFX)
            AudioManager.instance.OnSFXVolumeChanged -= ChangeVolume;
        else
            AudioManager.instance.OnBGMVolumeChanged -= ChangeVolume;

        isSubscribed = false;
    }

    private void ChangeVolume(float newVolume)
    {
        if (audioSource == null) return;

        // Remap global volume (0–1) to this wrapper's local min–max range
        float scaledVolume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(newVolume));

        audioSource.volume = scaledVolume;
    }
}
