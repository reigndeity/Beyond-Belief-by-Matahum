using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Player Danger Audio")]
    public AudioSource playerAudioCueSource;
    public AudioClip inDangerClip;
    public static event System.Action<bool> OnDangerStateChanged;


    [Header("Player Voice Audio")]
    public AudioSource playerVoiceSource;
    public AudioClip[] jumpVoiceSFX;
    public AudioClip[] dashVoiceSFX;
    public AudioClip[] hurtVoiceSFX;
    public AudioClip deathVoiceSFX;
    [Header("Player General Audio")]
    public AudioSource playerAudioSource;
    public AudioClip jumpSFX;
    public AudioClip landingSFX;
    public AudioClip dashSFX;

    [Header("Player Skills Audio")]
    public AudioSource playerSkillSource;
    public AudioClip normalSkillSFX;
    public AudioClip ultimateSkillSFX;

    [Header("Player Slash Audio")]
    public AudioSource playerSwordAudioSource;
    public AudioClip[] swordSlashSFX;

    [Header("Player Footsteps Audio")]
    public AudioSource playerFootstepAudioSource;
    public AudioClip[] walkStepSFX;
    private int walkCount = 0;
    public AudioClip[] footStepSFX;

    [Header("UI SFX Audio")]
    public AudioSource sfxSource;
    public AudioClip newQuestSFX;
    public AudioClip tutorialPopUpSFX;
    public AudioClip buttonClickSFX;
    public AudioClip onHoverSFX;

    //===================================================================================================
    public event System.Action<float> OnSFXVolumeChanged;
    public event System.Action<float> OnBGMVolumeChanged;

    [Header("SFX and BGM Volume")]
    public float SFXvolumeValue;
    public float BGMvolumeValue;
    public bool isPaused;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);  // destroy the whole GameObject, not just the component
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SFXvolumeValue = PlayerPrefs.GetFloat("Audio_SFX", 100);
        BGMvolumeValue = PlayerPrefs.GetFloat("Audio_BGM", 100);
        SetSFXVolume(SFXvolumeValue);
        SetBGMVolume(BGMvolumeValue);

        if(UI_Game.Instance != null)
        {
            UI_Game.Instance.OnGamePause += HandlePauseState;
        }
    }
    private void OnDestroy()
    {
        if (UI_Game.Instance != null)
        {
            UI_Game.Instance.OnGamePause -= HandlePauseState;
        }
    }
    private void HandlePauseState(bool isPaused)
    {
        AudioSource[] sources = UnityEngine.Object.FindObjectsByType<AudioSource>(
            FindObjectsSortMode.None
        );

        foreach (AudioSource audioSource in sources)
        {
            if (isPaused)
            {
                GameIsPaused(audioSource);
            }
            else
            {
                GameIsResumed(audioSource);
            }
        }

        
    }
    public void GameIsPaused(AudioSource audioSource)
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }

    public void GameIsResumed(AudioSource audioSource)
    {
        if (audioSource != null && !audioSource.isPlaying && audioSource.clip != null)
            audioSource.UnPause();
    }

    #region Player Audio
    public void PlaySwordSlash(int audio)
    {
        playerSwordAudioSource.PlayOneShot(swordSlashSFX[audio]);
    }

    public void PlayWalkStepSFX()
    {
        if (walkStepSFX == null || walkStepSFX.Length == 0)
            return;

        playerFootstepAudioSource.PlayOneShot(walkStepSFX[walkCount % walkStepSFX.Length]);
        walkCount++;
    }
    public void PlayFootStepSFX()
    {
        int random = Random.Range(0, 6);
        playerFootstepAudioSource.PlayOneShot(footStepSFX[random]);
    }

    public void PlayJumpSFX()
    {
        playerAudioSource.PlayOneShot(jumpSFX);
    }

    public void PlayLandSFX()
    {
        playerAudioSource.PlayOneShot(landingSFX);
    }
    public void PlayDashSFX()
    {
        playerAudioSource.PlayOneShot(dashSFX);
    }

    public void PlayNormalSkillSFX()
    {
        playerSkillSource.PlayOneShot(normalSkillSFX);
    }
    public void PlayUltimateSkillSFX()
    {
        playerSkillSource.PlayOneShot(ultimateSkillSFX);
    }

    public void PlayJumpVoiceSFX()
    {
        int audio = Random.Range(0, 2);
        playerVoiceSource.PlayOneShot(jumpVoiceSFX[audio]);
    }

    public void PlayDashVoiceSFX()
    {
        int audio = Random.Range(0, 4);
        playerVoiceSource.PlayOneShot(dashVoiceSFX[audio]);
    }

    public void PlayHurtVoiceSFX(int audio)
    {
        playerVoiceSource.PlayOneShot(hurtVoiceSFX[audio]);
    }
    public void PlayDeathVoiceSFX()
    {
        playerVoiceSource.PlayOneShot(deathVoiceSFX);
    }


    #endregion

    #region UI SFX
    public void PlayNewQuestSFX()
    {
        sfxSource.PlayOneShot(newQuestSFX);
    }
    public void PlayOnHoverSFX()
    {
        sfxSource.PlayOneShot(onHoverSFX);
    }
    #endregion


    public void SetSFXVolume(float newValue)
    {
        SFXvolumeValue = newValue;
        OnSFXVolumeChanged?.Invoke(newValue);
    }

    public void SetBGMVolume(float newValue)
    {
        BGMvolumeValue = newValue;
        OnBGMVolumeChanged?.Invoke(newValue);
    }
    public void PlayInDangerCue(bool inDanger)
    {
        if (playerAudioCueSource == null || inDangerClip == null)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeDangerCue(inDanger));
        OnDangerStateChanged?.Invoke(inDanger);
    }

    private IEnumerator FadeDangerCue(bool inDanger)
    {
        float fadeDuration = 1.5f;

        float targetVolume;
        AudioWrapper wrapper = playerAudioCueSource.GetComponent<AudioWrapper>();
        if (wrapper != null)
        {
            targetVolume = inDanger ? wrapper.GetCurrentVolume() : 0f;
        }
        else
        {
            targetVolume = inDanger ? 1f : 0f;
        }

        float elapsed = 0f;

        if (inDanger)
        {
            // 🔒 Prevent stacking — only start once
            if (!playerAudioCueSource.isPlaying)
            {
                playerAudioCueSource.clip = inDangerClip;
                playerAudioCueSource.loop = true;
                playerAudioCueSource.volume = 0f;
                playerAudioCueSource.Play();
            }
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            playerAudioCueSource.volume = Mathf.Lerp(0, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        playerAudioCueSource.volume = targetVolume;

        if (!inDanger && Mathf.Approximately(targetVolume, 0f))
        {
            // 🧹 Stop only when fading out is done
            playerAudioCueSource.Stop();
            playerAudioCueSource.clip = null;
        }
    }


}

