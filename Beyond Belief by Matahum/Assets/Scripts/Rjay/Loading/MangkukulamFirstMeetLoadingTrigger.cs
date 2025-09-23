using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MangkukulamFirstMeetLoadingLoadingTrigger : MonoBehaviour
{
    private Player m_player;
    public int sceneIndex;

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
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A2_Q1_Albularyo'sHut");
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        BB_QuestManager.Instance.ClaimRewardsByID("A2_Q1_FindAlbularyo'sHut");
        await Task.Delay(500);
        BB_QuestManager.Instance.AcceptQuestByID("A2_Q2_MysteriousWoman");
        await Task.Delay(500);
        await GameManager.instance.SavePlayerCoreData();
        await Task.Delay(500);
        Loader.Load(sceneIndex);
    }

}
