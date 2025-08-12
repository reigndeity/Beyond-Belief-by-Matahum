using UnityEngine;

public class BB_DomainClaimRewardsInteractable : Interactable
{
    public override void OnInteract()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        BB_DomainManager.instance.ClaimPanel(BB_DomainManager.instance.selectedDomain);

        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        uiGame.PauseGame();

        Destroy(this);
    }
}
