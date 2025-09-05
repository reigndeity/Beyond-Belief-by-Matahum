using System.Threading.Tasks;
using UnityEngine;

public class Nuno_GameStart : MonoBehaviour
{
    [SerializeField] UI_CanvasGroup nunoCanvasGroup;
    private async void Start()
    {
        await CustomLoadPlayer();
    }

    private async Task CustomLoadPlayer()
    {
        // Fade in
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await Task.Delay(1000);

        // Setup
        TutorialManager.instance.TutorialCheck();
        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
        
        await GameManager.instance.LoadPlayerCoreData();
        await Task.Delay(100);
        BB_QuestManager.Instance.AcceptQuestByID("A1_Q6_NunoAnger");

        DisableUI();


        // Fade out
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
        BattleStart();
        await Task.Delay(500);
        nunoCanvasGroup.FadeIn(1f);

    }

    public void BattleStart()
    {
        Nuno_AttackManager.Instance.isBattleStart = true;
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
