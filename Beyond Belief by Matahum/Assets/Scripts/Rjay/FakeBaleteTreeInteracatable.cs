    using System.Threading.Tasks;
using UnityEngine;

public class FakeBaleteTreeInteracatable : Interactable
{
    public int sceneIndex;
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
        await GameManager.instance.SaveAll();
        await Task.Delay(1000);
        Loader.Load(sceneIndex);
    }
}
