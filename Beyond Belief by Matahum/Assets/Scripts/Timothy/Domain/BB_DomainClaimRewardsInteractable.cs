using UnityEngine;

public class BB_DomainClaimRewardsInteractable : Interactable
{
    public override void OnInteract()
    {
        BB_DomainManager.instance.ClaimPanel(BB_DomainManager.instance.selectedDomain);
    }
}
