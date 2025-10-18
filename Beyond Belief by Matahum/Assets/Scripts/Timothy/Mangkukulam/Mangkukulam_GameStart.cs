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
        await GameManager.instance.LoadPlayerCoreData();
        await Task.Delay(1000);
        TutorialManager.instance.TutorialCheck();
        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A2_Q8_MangkukulamHut");
        await Task.Delay(100);
        BB_QuestManager.Instance.ClaimRewardsByID("A2_Q8_GoToMangkukulamWithAlbularyo");
        await Task.Delay(500);
        BB_QuestManager.Instance.AcceptQuestByID("A2_Q9_DefeatMangkukulam");
        DisableUI();
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
        await Task.Delay(1000);
        Invoke("BattleStart", 3);
        await Task.Delay(500);
        mangkukulamCanvasGroup.FadeIn(1f);

    }

    public void BattleStart()
    {
        Mangkukulam.instance.BattleStart();
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
