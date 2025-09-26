using System.Threading.Tasks;
using UnityEngine;

public class FakeBaleteManager : MonoBehaviour
{
    public string missionID;
    public string claimRewardsID;
    public string acceptQuestID;

    public string enemyMissionID;
    public GameObject enemies;

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
        await GameManager.instance.LoadPlayerNoQuest();
        await Task.Delay(100);
        BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        await Task.Delay(100);
        BB_QuestManager.Instance.ClaimRewardsByID(claimRewardsID);
        await Task.Delay(100);
        BB_QuestManager.Instance.AcceptQuestByID(acceptQuestID);
        // Fade out
        await Task.Delay(1000);
        enemies.SetActive(true);
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

    public void EnemyMissionUpdate()
    {
        BB_QuestManager.Instance.UpdateMissionProgressOnce(enemyMissionID);
    }
}
