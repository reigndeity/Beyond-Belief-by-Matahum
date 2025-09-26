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
        base.OnInteract();
        LoadingToCutscene();
    }

    public void LoadingToCutscene()
    {
         _ = Loading();
    }
    private async Task Loading()
    {
        //BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        //BB_QuestManager.Instance.ClaimRewardsByID(claimRewardsID);
        await Task.Delay(500);
        await GameManager.instance.SaveAll();
        await Task.Delay(500);
        Loader.Load(sceneIndex);
    }
}
