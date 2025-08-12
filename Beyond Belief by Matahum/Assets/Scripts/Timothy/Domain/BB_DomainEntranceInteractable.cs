using UnityEngine;
using UnityEngine.UI;

public class BB_DomainEntranceInteractable : Interactable
{
    public BB_DomainSO domainSO;

    [Header("If inside this domain")]
    public Transform spawnPoint;

    public override void OnInteract()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        uiGame.PauseGame();

        BB_DomainManager.instance.selectedDomain = domainSO;
        BB_DomainButtonManager.instance.OpenDomainDetails();
    }
}
