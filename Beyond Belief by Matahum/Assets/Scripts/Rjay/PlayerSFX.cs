using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("Player General Audio")]
    public AudioSource playerAudioSource;
    public AudioClip jumpSFX;
    public AudioClip landingSFX;

    [Header("Player Slash Audio")]
    public AudioSource playerSwordAudioSource;
    public AudioClip[] swordSlashSFX;

    [Header("Player Footsteps Audio")]
    public AudioSource playerFootstepAudioSource;
    public AudioClip[] walkStepSFX;
    public int walkCount = 0;
    public AudioClip[] footStepSFX;



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
}
