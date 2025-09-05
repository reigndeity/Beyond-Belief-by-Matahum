using System.Collections;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
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
            StartCoroutine(LoadingToCutscene());
        }
    }

    public IEnumerator LoadingToCutscene()
    {
        m_player.SetPlayerLocked(true);
        m_player.ForceIdleOverride();
        PlayerCamera.Instance.HardLockCamera();
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        yield return new WaitForSeconds(1f);
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q5_NunoMound");
        yield return new WaitForSeconds(0.1f);
        BB_QuestManager.Instance.ClaimRewardsByID("A1_Q5_TimeToRest");
        
        yield return new WaitForSeconds(1f);
        Loader.Load(sceneIndex);
    }
}
