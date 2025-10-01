using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MangkukulamLoadingTrigger : MonoBehaviour
{
    private Player m_player;
    public int sceneIndex;

    public string missionID = "A2_Q1_Albularyo'sHut";
    public string rewardsID = "A2_Q1_FindAlbularyo'sHut";
    public string acceptquestID = "A2_Q2_MysteriousWoman";

    void Start()
    {
        m_player = FindFirstObjectByType<Player>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.gameObject.tag == "Player")
        {
            LoadingToCutscene();
        }
    }

    public void LoadingToCutscene()
    {
         _ = Loading();
    }
    private async Task Loading()
    {
        m_player.SetPlayerLocked(true);
        m_player.ForceIdleOverride();
        PlayerCamera.Instance.HardLockCamera();
        BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        BB_QuestManager.Instance.ClaimRewardsByID(rewardsID);
        await Task.Delay(500);
        BB_QuestManager.Instance.AcceptQuestByID(acceptquestID);
        await Task.Delay(500);
        await GameManager.instance.SavePlayerCoreData();
        await Task.Delay(500);
        Loader.Load(sceneIndex);
    }

}
