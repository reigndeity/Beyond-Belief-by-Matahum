using System.Collections;
using UnityEngine;

public class Nuno_GameStart : MonoBehaviour
{
    IEnumerator CustomLoadPlayer()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        yield return new WaitForSeconds(1f);
        //SaveManager.Instance.LoadPlayerEquipment();
        TutorialManager.instance.TutorialCheck();
        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
        yield return new WaitForSeconds(1f);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
        BattleStart();
    }

    public void BattleStart()
    {
        Nuno_AttackManager.Instance.isBattleStart = true;
    }
}
