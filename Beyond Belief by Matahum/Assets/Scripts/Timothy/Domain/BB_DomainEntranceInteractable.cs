using UnityEngine;
using UnityEngine.UI;

public class BB_DomainEntranceInteractable : Interactable
{
    public BB_DomainSO domainSO;

    [Header("If inside this domain")]
    public Transform spawnPoint;

    public override void OnInteract()
    {
        BB_DomainManager.instance.spawnPoint = spawnPoint.position;
        BB_DomainManager.instance.spawnRotation = spawnPoint.rotation;

        BB_DomainManager.instance.selectedDomain = domainSO;
        BB_DomainButtonManager.instance.OpenDomainDetails();
    }
}
