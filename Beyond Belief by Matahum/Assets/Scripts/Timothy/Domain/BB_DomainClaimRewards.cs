using UnityEngine;

public class BB_DomainClaimRewards : Interactable
{
    public GameObject claimRewardPanel;
    public Transform rewardHolder;
    private BB_DomainSO domain;

    private void Start()
    {
        domain = BB_DomainManager.instance.selectedDomain;
        claimRewardPanel = BB_DomainButtonManager.instance.claimRewardPanel;
    }

    public override void OnInteract()
    {
        BB_DomainManager.instance.ClaimPanel(domain, claimRewardPanel, rewardHolder);
        claimRewardPanel.SetActive(true);
    }
}
