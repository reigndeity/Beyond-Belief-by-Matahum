using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MangkukulamBossFightTrigger : MonoBehaviour
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
        m_player.SetPlayerLocked(true);
        m_player.ForceIdleOverride();
        PlayerCamera.Instance.HardLockCamera();
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await Task.Delay(500);
        await GameManager.instance.SaveAll();
        await Task.Delay(500);
        Loader.Load(sceneIndex);
    }

}
