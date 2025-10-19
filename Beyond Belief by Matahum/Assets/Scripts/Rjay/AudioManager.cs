using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static event System.Action<bool> OnDangerStateChanged;
    private Coroutine dangerFadeCoroutine;


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
    public AudioClip[] buttonClickSFX;
    public AudioClip[] dialogueCompleteLineSFX;
    public AudioClip onHoverSFX;
    public AudioClip equipSFX;
    public AudioClip unequipSFX;

    [Header("NPC Audio")]
    public AudioSource npcSource;
    public AudioClip tupasInteractClip;
    public AudioClip deerInteractClip;
    public AudioClip monkeyInteractClip;

    [Header("Music Audio")]
    public AudioSource musicSource;
    public AudioClip lewenriVillageMusic;
    public AudioClip lewenriTrainingAreaMusic;
    public AudioClip outsideTheVillageMusic;
    public AudioClip swampMusic;
    public AudioClip inDangerClip;

    [Header("Ambience Audio")]
    public AudioSource ambienceSource;
    public AudioClip forestAmbience;
    public AudioClip swampAmbience;
    public AudioClip waterfallAmbience;

    public bool isInVillage;
    public bool isInTraining;
    public bool isInSwamp;
    public bool isInWaterfall;

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

        //AudioCheck();
    }

    private void Start()
    {
        SFXvolumeValue = PlayerPrefs.GetFloat("Audio_SFX", 100);
        BGMvolumeValue = PlayerPrefs.GetFloat("Audio_BGM", 100);
        SetSFXVolume(SFXvolumeValue);
        SetBGMVolume(BGMvolumeValue);

        if (UI_Game.Instance != null)
        {
            UI_Game.Instance.OnGamePause += HandlePauseState;
        }

        PlayForestAmbience(2);
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

    public void PlayTutorialPopUpSFX()
    {
        sfxSource.PlayOneShot(tutorialPopUpSFX);
    }

    public void PlayDialogueComepleteLineSFX()
    {
        int audio = Random.Range(0, dialogueCompleteLineSFX.Length);
        sfxSource.PlayOneShot(dialogueCompleteLineSFX[audio]);
    }
    public void PlayButtonClickSFX()
    {
        int audio = Random.Range(0, buttonClickSFX.Length);
        sfxSource.PlayOneShot(buttonClickSFX[audio]);
    }
    public void PlayOnHoverSFX()
    {
        sfxSource.PlayOneShot(onHoverSFX);
    }
    public void PlayEquipSFX()
    {
        sfxSource.PlayOneShot(equipSFX);
    }
    public void PlayUnequipSFX()
    {
        sfxSource.PlayOneShot(unequipSFX);
    }
    #endregion
    #region NPC SFX
    public void PlayTupasInteractSFX()
    {
        npcSource.PlayOneShot(tupasInteractClip);
    }
    public void PlayerDeerInteractSFX()
    {
        npcSource.PlayOneShot(deerInteractClip);
    }
    public void PlayMonkeyInteractSFX()
    {
        npcSource.PlayOneShot(monkeyInteractClip);
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


    #region OPEN WORLD AUDIO
    public void PlayForestAmbience(float fadeDuration = 2f)
    {
        if (ambienceSource == null || forestAmbience == null)
            return;

        // If it's already playing this clip, skip
        if (ambienceSource.clip == forestAmbience && ambienceSource.isPlaying)
            return;

        StopAllCoroutines(); // stop any previous fades
        StartCoroutine(FadeInAmbience(forestAmbience, fadeDuration));
    }

    private IEnumerator FadeInAmbience(AudioClip newClip, float fadeDuration)
    {
        AudioWrapper wrapper = ambienceSource.GetComponent<AudioWrapper>();
        float targetVolume = wrapper != null ? wrapper.GetCurrentVolume() : 1f;

        ambienceSource.clip = newClip;
        ambienceSource.loop = true;
        ambienceSource.volume = 0f;
        ambienceSource.Play();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        ambienceSource.volume = targetVolume;
    }
        
    public void PlayInDangerCue(bool inDanger)
    {
        if (musicSource == null || inDangerClip == null)
            return;

        // Stop only the danger fade, not all coroutines globally
        if (dangerFadeCoroutine != null)
            StopCoroutine(dangerFadeCoroutine);

        dangerFadeCoroutine = StartCoroutine(FadeDangerCue(inDanger));
        OnDangerStateChanged?.Invoke(inDanger);
    }


    private IEnumerator FadeDangerCue(bool inDanger)
    {
        float fadeDuration = 1.5f;
        float elapsed = 0f;

        AudioWrapper wrapper = musicSource.GetComponent<AudioWrapper>();
        float targetVolume = inDanger
            ? (wrapper != null ? wrapper.GetCurrentVolume() : 1f)
            : 0f;

        if (inDanger)
        {
            // Re-prepare clip even if it's been stopped earlier
            if (musicSource.clip != inDangerClip)
            {
                musicSource.clip = inDangerClip;
                musicSource.loop = true;
            }

            // Restart playback safely
            if (!musicSource.isPlaying)
            {
                musicSource.volume = 0f;
                musicSource.Play();
            }
        }

        float startVolume = musicSource.volume;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;

        if (!inDanger && Mathf.Approximately(targetVolume, 0f))
        {
            musicSource.Stop();
            // DO NOT set clip = null; keeps clip ready for next use
        }

        dangerFadeCoroutine = null;
    }

    #endregion




}

