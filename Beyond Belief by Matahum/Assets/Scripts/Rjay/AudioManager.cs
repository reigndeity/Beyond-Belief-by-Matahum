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

    [Header("NPC Audio")]
    public AudioSource npcSource;
    public AudioClip tupasInteractClip;
    public AudioClip deerInteractClip;
    public AudioClip monkeyInteractClip;

    [Header("Music Audio")]
    public AudioSource musicSource;
    public AudioClip currentMusic;
    public AudioClip inDangerClip;
    public bool lockMusic = false;

    [Header("Ambience Audio")]
    public AudioSource ambienceSource;

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
    #endregion

    #region AUDIO ZONES

    public void LockMusic(bool state)
    {
        lockMusic = state;
    }
    public void PlayInDangerCue(bool isInDanger)
    {
        if (isInDanger)
        {
            StartCoroutine(FadeMusic(inDangerClip));
        }
        else
        {
            StartCoroutine(FadeMusic(currentMusic));
        }

    }
    private void HandleDangerMusic(bool isInDanger)
    {
        if (dangerFadeCoroutine != null)
            StopCoroutine(dangerFadeCoroutine);

        dangerFadeCoroutine = StartCoroutine(FadeMusic(isInDanger ? inDangerClip : currentMusic));
        lockMusic = !isInDanger;
    }

    public IEnumerator FadeMusic(AudioClip bgmClip)
    {
        if (musicSource.clip == bgmClip) yield break;
        if (lockMusic) yield break;

        float currentAudioVolume = musicSource.volume;
        yield return FadeOut(musicSource);
        yield return FadeIn(musicSource, bgmClip, currentAudioVolume);
    }

    public IEnumerator FadeAmbience(AudioClip sfxClip)
    {
        if (ambienceSource.clip == sfxClip) yield break;
        if (lockMusic) yield break;

        float currentAudioVolume = ambienceSource.volume;
        yield return FadeOut(ambienceSource);
        yield return FadeIn(ambienceSource, sfxClip, currentAudioVolume);
    }
    IEnumerator FadeIn(AudioSource source, AudioClip clip, float targetVolume)
    {
        if (clip != inDangerClip) currentMusic = clip;
        source.clip = clip;
        source.Play();

        float i = 0;
        float duration = 2;
        while (i < duration)
        {
            i += Time.deltaTime;

            source.volume = Mathf.Lerp(0, targetVolume, i / duration);

            yield return null;
        }
        source.volume = targetVolume;
    }

    IEnumerator FadeOut(AudioSource source)
    {
        float startVolume = source.volume;

        float i = 0;
        float duration = 2;
        while (i < duration)
        {
            i += Time.deltaTime;

            source.volume = Mathf.Lerp(source.volume, 0, i / duration);

            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // reset for reuse
    }
    #endregion
}

