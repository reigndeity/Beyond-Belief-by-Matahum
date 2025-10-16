using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public void PlaySwordSlash(int audio)
    {
        AudioManager.instance.PlaySwordSlash(audio);
    }

    public void PlayWalkStepSFX()
    {
        AudioManager.instance.PlayWalkStepSFX();
    }
    public void PlayFootStepSFX()
    {
        AudioManager.instance.PlayFootStepSFX();
    }

    public void PlayJumpSFX()
    {
        AudioManager.instance.PlayJumpSFX();
    }

    public void PlayLandSFX()
    {
        AudioManager.instance.PlayLandSFX();
    }
    public void PlayDashSFX()
    {
        AudioManager.instance.PlayDashSFX();
    }

    public void PlayNormalSkillSFX()
    {
        AudioManager.instance.PlayNormalSkillSFX();
    }
    public void PlayUltimateSkillSFX()
    {
        AudioManager.instance.PlayUltimateSkillSFX();
    }

    public void PlayJumpVoiceSFX()
    {
        AudioManager.instance.PlayJumpVoiceSFX();
    }

    public void PlayDashVoiceSFX()
    {
        AudioManager.instance.PlayDashVoiceSFX();
    }

    public void PlayHurtVoiceSFX(int audio)
    {
        AudioManager.instance.PlayHurtVoiceSFX(audio);
    }
    public void PlayDeathVoiceSFX()
    {
        AudioManager.instance.PlayDeathVoiceSFX();
    }
}
