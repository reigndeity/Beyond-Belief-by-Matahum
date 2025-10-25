using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
{
    private Player m_player;
    public int sceneIndex;
    public string updateQuest = "A1_Q5_NunoMound";
    public string claimQuest = "A1_Q5_TimeToRest";

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
    public async Task Loading()
    {
        m_player.SetPlayerLocked(true);
        m_player.ForceIdleOverride();
        PlayerCamera.Instance.HardLockCamera();
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await GameManager.instance.SaveAllExceptPlayerPosition();
        await Task.Delay(1000);
        BB_QuestManager.Instance.UpdateMissionProgressOnce(updateQuest);
        await Task.Delay(100);
        BB_QuestManager.Instance.ClaimRewardsByID(claimQuest);
        await Task.Delay(100);
        Loader.Load(sceneIndex);
        
    }
}
