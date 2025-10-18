using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Player Audio")]

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

    //===================================================================================================
    public event System.Action<float> OnSFXVolumeChanged;
    public event System.Action<float> OnBGMVolumeChanged;

    [Header("SFX and BGM Volume")]
    public float SFXvolumeValue;
    public float BGMvolumeValue;

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
}

