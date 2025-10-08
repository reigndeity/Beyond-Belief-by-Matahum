using System.Threading.Tasks;
using UnityEngine;

public class ReapirFakeBaleteManager : MonoBehaviour
{
    public string goToLocationMission;
    public string goToLocationQuest;
    public string acceptQuestID;
    public string nextQuestID;
    private bool isDone;

    private async void Start()
    {
        await CustomLoadPlayer();
    }

    private async Task CustomLoadPlayer()
    {
        DisableUI();
        await Task.Delay(1000);

        // Setup
        TutorialManager.instance.TutorialCheck();
        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
        await GameManager.instance.LoadPlayerCoreData();
        await Task.Delay(100);
        BB_QuestManager.Instance.UpdateMissionProgressOnce(goToLocationMission);
        await Task.Delay(100);
        BB_QuestManager.Instance.ClaimRewardsByID(goToLocationQuest);
        await Task.Delay(100);
        BB_QuestManager.Instance.AcceptQuestByID(acceptQuestID);
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
        await Task.Delay(500);
    }


    public void DisableUI()
    {
        TutorialManager.instance.HideMinimap();
        TutorialManager.instance.HideCharacterDetails();
        TutorialManager.instance.HideQuestJournal();
        TutorialManager.instance.HideInventory();
        TutorialManager.instance.DisableFullscreenMap();
    }

    public void QuestCheck()
    {
        _ = CompleteQuestCheck();
    }

    public async Task CompleteQuestCheck()
    {
        if (BB_QuestManager.Instance.IsQuestDone(acceptQuestID) && isDone == false)
        {
            isDone = true;
            await Task.Delay(6000);
            StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
            await Task.Delay(500);
            BB_QuestManager.Instance.ClaimRewardsByID(acceptQuestID);
            await Task.Delay(500);
            BB_QuestManager.Instance.AcceptQuestByID(nextQuestID);
            await Task.Delay(1000);
            await GameManager.instance.SavePlayerCoreData();
            await Task.Delay(1000);
            Loader.Load(4);
        }
        else
        {
            return;
        }
    }
}
