using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Player Audio")]
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
    

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

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

}

