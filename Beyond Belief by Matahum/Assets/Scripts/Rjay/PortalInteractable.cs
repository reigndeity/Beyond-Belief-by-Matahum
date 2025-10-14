using System.Threading.Tasks;
using UnityEngine;

public class PortalInteractable : Interactable
{
    private Player m_player;
    private bool isCreditsDone;
    void Awake()
    {
        m_player = FindFirstObjectByType<Player>();
    }
    public override void OnInteract()
    {
        if (useInteractCooldown && IsOnCooldown()) return;
        ExitGame();
        base.OnInteract();
    }

    public void ExitGame()
    {
        _ = Exiting();
    }

    public async Task Exiting()
    {
        m_player.SetPlayerLocked(true);
        m_player.ForceIdleOverride();
        PlayerCamera.Instance.HardLockCamera();
        await Task.Delay(1000);
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await Task.Delay(500);
        if (isCreditsDone == false)
        {
            isCreditsDone = true;
            await Task.Delay(500);
            await GameManager.instance.SaveAll();
            await Task.Delay(500);
            Loader.Load(20);
        }
        else
        {
            await Task.Delay(500);
            await GameManager.instance.SaveAll();
            await Task.Delay(500);
            Loader.Load(0);
        }
    }
}
