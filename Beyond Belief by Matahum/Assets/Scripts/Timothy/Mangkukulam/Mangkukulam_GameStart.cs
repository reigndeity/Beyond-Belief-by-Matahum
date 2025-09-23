using System.Threading.Tasks;
using UnityEngine;

public class Mangkukulam_GameStart : MonoBehaviour
{
    [SerializeField] UI_CanvasGroup mangkukulamCanvasGroup;

    private async void Start()
    {
        await CustomLoadPlayer();
    }

    private async Task CustomLoadPlayer()
    {
        // Fade in
        //questHudCanvasGroup.FadeOut(0.25f);
        await Task.Delay(1000);

        // Setup
        TutorialManager.instance.TutorialCheck();
        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
        await GameManager.instance.LoadPlayerNoQuest();

        //DisableUI();

        // Fade out
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
        BattleStart();
        await Task.Delay(500);
        mangkukulamCanvasGroup.FadeIn(1f);

    }

    public void BattleStart()
    {
        Mangkukulam.instance.isBattleStart = true;
    }

    public void DisableUI()
    {
        TutorialManager.instance.HideMinimap();
        TutorialManager.instance.HideCharacterDetails();
        TutorialManager.instance.HideQuestJournal();
        TutorialManager.instance.HideInventory();
        TutorialManager.instance.DisableFullscreenMap();
    }
}
