using UnityEngine;
using UnityEngine.UI;

public class BB_DomainEntranceInteractable : Interactable
{
    public BB_DomainSO domainSO;

    [Header("If inside this domain")]
    public Transform spawnPoint;

    public override void OnInteract()
    {
        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(true);

        BB_DomainManager.instance.spawnPoint = spawnPoint.transform.position;
        BB_DomainManager.instance.spawnRotation = spawnPoint.transform.rotation;

        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        uiGame.PauseGame();

        BB_DomainManager.instance.selectedDomain = domainSO;
        BB_DomainButtonManager.instance.OpenDomainDetails();
    }
}
