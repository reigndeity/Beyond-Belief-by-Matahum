    using System.Threading.Tasks;
using UnityEngine;

public class FakeBaleteTreeInteracatable : Interactable
{
    public int sceneIndex;
    public string missionID;
    public string claimRewardsID;
    public string acceptQuestID;
    public override void OnInteract()
    {
        // Check cooldown before doing anything
        if (useInteractCooldown && IsOnCooldown())
            return;

        base.OnInteract();   // triggers cooldown if enabled
        LoadingToCutscene();
    }


    public void LoadingToCutscene()
    {
         _ = Loading();
    }
    private async Task Loading()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await Task.Delay(1000);
        BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        await Task.Delay(100);
        BB_QuestManager.Instance.ClaimRewardsByID(claimRewardsID);
        await Task.Delay(100);
        BB_QuestManager.Instance.AcceptQuestByID(acceptQuestID);
        await Task.Delay(100);
        await GameManager.instance.SaveAll();
        await Task.Delay(500);
        Loader.Load(sceneIndex);
    }
}
