using UnityEngine;
using System.Threading.Tasks;
public class CustomLoader : MonoBehaviour
{
    private async void Start()
    {
        await CustomLoadPlayer();
    }

    private async Task CustomLoadPlayer()
    {
        DisableUI();
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));  
        await Task.Delay(1000);
        TutorialManager.instance.TutorialCheck();
        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
        await GameManager.instance.LoadPlayerNoQuest();
    }

    public void DisableUI()
    {
        TutorialManager.instance.HideMinimap();
        TutorialManager.instance.HideCharacterDetails();
        TutorialManager.instance.HideQuestJournal();
        TutorialManager.instance.HideInventory();
        TutorialManager.instance.DisableFullscreenMap();
        TutorialManager.instance.HideArchives();
    }
}
