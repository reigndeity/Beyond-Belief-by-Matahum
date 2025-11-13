using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static event System.Action<bool> OnDangerStateChanged;
    private Coroutine dangerFadeCoroutine;

    [Header("References")]
    public Player player;

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
    public AudioClip archiveFilterSFX;

    [Header("NPC Audio")]
    public AudioSource npcSource;
    public AudioClip tupasInteractClip;
    public AudioClip deerInteractClip;
    public AudioClip monkeyInteractClip;

    [Header("Music Audio")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;

    public AudioClip currentMusic;
    public AudioClip inDangerClip;
    public bool lockMusic = false;

    [Header("Ambience Audio")]
    public AudioSource ambienceSourceA;
    public AudioSource ambienceSourceB;
    private AudioSource activeAmbienceSource;
    private AudioSource inactiveAmbienceSource;

    //===================================================================================================
    public event System.Action<float> OnGeneralVolumeChanged;
    public event System.Action<float> OnSFXVolumeChanged;
    public event System.Action<float> OnBGMVolumeChanged;
    public event System.Action<float> OnAmbienceVolumeChanged;

    [Header("SFX and BGM Volume")]
    public float GeneralVolumeValue;
    public float SFXvolumeValue;
    public float BGMvolumeValue;
    public float ambienceVolumeValue;
    public bool isPaused;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);  // destroy the whole GameObject, not just the component
            return;
        }

        instance = this;
    }

    private void Start()
    {
        GeneralVolumeValue = PlayerPrefs.GetFloat("General_Volume", 100);
        SFXvolumeValue = PlayerPrefs.GetFloat("Audio_SFX", 100);
        BGMvolumeValue = PlayerPrefs.GetFloat("Audio_BGM", 100);
        ambienceVolumeValue = PlayerPrefs.GetFloat("Audio_Ambience", 100);

        SetGeneralVolume(GeneralVolumeValue);
        SetSFXVolume(SFXvolumeValue);
        SetBGMVolume(BGMvolumeValue);
        SetAmbienceVolume(ambienceVolumeValue);

        if(musicSourceA != null) activeMusicSource = musicSourceA;
        if(musicSourceB != null) inactiveMusicSource = musicSourceB;
        if(ambienceSourceA != null) activeAmbienceSource = ambienceSourceA;
        if(ambienceSourceB != null) inactiveAmbienceSource = ambienceSourceB;


        if (UI_Game.Instance != null)
        {
            UI_Game.Instance.OnGamePause += HandlePauseState;
        }
    }

    public bool lastDangerState = false; // Tracks previous state

    private void Update()
    {
        if (player == null)
            return;

        // Detect danger state changes
        if (player.inDanger != lastDangerState)
        {
            lastDangerState = player.inDanger;
            HandleDangerMusic(player.inDanger);
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
    public void PlayArchiveFilterSFX()
    {
        sfxSource.PlayOneShot(archiveFilterSFX);
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

    #region SET VOLUME
    public void SetGeneralVolume(float newValue)
    {
        GeneralVolumeValue = newValue; ;
        AudioListener.volume = Mathf.Clamp01(newValue);
        OnGeneralVolumeChanged?.Invoke(newValue);
    }
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

    public void SetAmbienceVolume(float newValue)
    {
        ambienceVolumeValue = newValue;
        OnAmbienceVolumeChanged?.Invoke(newValue);
    }
    #endregion

    #region AUDIO ZONES

    public Coroutine bgmCoroutine;
    public Coroutine ambienceCoroutine;

    public void LockMusic(bool state)
    {
        lockMusic = state;
    }
    public void HandleDangerMusic(bool isInDanger)
    {
        /* if (dangerFadeCoroutine != null)
             StopCoroutine(dangerFadeCoroutine);

         // Always allow dynamic transitions
         lockMusic = false;

         AudioClip dangerClip = isInDanger ? inDangerClip : currentMusic;

         dangerFadeCoroutine = StartCoroutine(CrossFadeMusic(dangerClip));*/

        lockMusic = false;

        AudioClip dangerClip = isInDanger ? inDangerClip : currentMusic;
        FadeMusic(dangerClip);
    }

    public void FadeMusic(AudioClip newClip)
    {
        if (activeMusicSource.clip == newClip) return;

        if (bgmCoroutine != null)
            StopCoroutine(bgmCoroutine);

        bgmCoroutine = StartCoroutine(CrossFadeMusic(newClip));
    }

    public void FadeAmbience(AudioClip newClip)
    {
        if (activeAmbienceSource.clip == newClip) return;

        if (ambienceCoroutine != null)
            StopCoroutine(ambienceCoroutine);

        ambienceCoroutine = StartCoroutine(CrossFadeAmbience(newClip));
    }

    public IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        if (lockMusic) yield break;
        //if (activeMusicSource.clip == newClip) yield break;

        if (inDangerClip == newClip)
        {
            lockMusic = true;
        }

        AudioWrapper wrapper = inactiveMusicSource.GetComponent<AudioWrapper>();
        float targetVolume = Mathf.Lerp(0, wrapper.maxVolume, Mathf.Clamp01(BGMvolumeValue));

        // Prepare inactive source
        inactiveMusicSource.clip = newClip;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.Play();

        // Perform crossfade
        float timer = 0f;
        float startVolume = activeMusicSource.volume;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            float t = timer / 1;

            activeMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            inactiveMusicSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // Swap roles
        activeMusicSource.Stop();

        var temp = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = temp;

        activeMusicSource.volume = targetVolume;
        if (newClip != inDangerClip)
            currentMusic = newClip;
    }

    public IEnumerator CrossFadeAmbience(AudioClip newClip)
    {
        if (lockMusic) yield break;
        //if (activeAmbienceSource.clip == newClip) yield break;

        AudioWrapper wrapper = inactiveAmbienceSource.GetComponent<AudioWrapper>();
        float targetVolume = Mathf.Lerp(0, wrapper.maxVolume, Mathf.Clamp01(ambienceVolumeValue));

        inactiveAmbienceSource.clip = newClip;
        inactiveAmbienceSource.volume = 0f;
        inactiveAmbienceSource.Play();

        float timer = 0f;
        float startVolume = activeAmbienceSource.volume;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            float t = timer / 1;

            activeAmbienceSource.volume = Mathf.Lerp(startVolume, 0f, t);
            inactiveAmbienceSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        activeAmbienceSource.Stop();

        var temp = activeAmbienceSource;
        activeAmbienceSource = inactiveAmbienceSource;
        inactiveAmbienceSource = temp;

        activeAmbienceSource.volume = targetVolume;
    }
    
    #endregion
}

