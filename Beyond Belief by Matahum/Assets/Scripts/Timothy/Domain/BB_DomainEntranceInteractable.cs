using UnityEngine;
using UnityEngine.UI;

public class BB_DomainEntranceInteractable : Interactable
{
    public BB_DomainSO domainSO;

    public override void OnInteract()
    {
        BB_DomainManager.instance.selectedDomain = domainSO;
        BB_DomainButtonManager.instance.OpenDomainDetails();
    }
}
