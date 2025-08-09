using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BB_DomainButtonManager : MonoBehaviour
{
    public static BB_DomainButtonManager instance;
    [Header("Enter Domain")]
    public Button enterDomainButton;

    [Header("Exit Details Button")]
    public GameObject domainDetailsPanel;
    public Button exitDetailsButton;

    [Header("Exit Domain")]
    public Button leaveDomainButton;
    public Button stayButton;
    public GameObject leaveConfirmationPanel;

    [Header("Claim Domain Rewards Button")]
    public GameObject claimRewardPanel;
    public Button claimRewardsButton;

    private void Awake()
    {
        if (instance == null)       
            instance = this;       
        else       
            Destroy(gameObject);
        
    }
    private void Start()
    {
        enterDomainButton.onClick.AddListener(EnterDomain);
        exitDetailsButton.onClick.AddListener(ExitDomainDetails);
        leaveDomainButton.onClick.AddListener(ExitDomain);
        stayButton.onClick.AddListener(StayInDomain);
        claimRewardsButton.onClick.AddListener(ClaimRewards);
    }
    public void EnterDomain()
    {
        ExitDomainDetails();
        BB_DomainManager.instance.EnterDomain();
    }

    public void OpenDomainDetails()
    {
        domainDetailsPanel.SetActive(true);
        BB_DomainDetailsUI.instance.OnOpenDomainDetails();
    }
    public void ExitDomainDetails()
    {
        domainDetailsPanel.SetActive(false);
    }

    public void ExitDomain()
    {
        Debug.Log("Returned to original position in main scene");
    }

    public void StayInDomain()
    {
        leaveConfirmationPanel.SetActive(false);
    }

    public void ClaimRewards()
    {
        BB_DomainManager.instance.ClaimRewards(BB_DomainManager.instance.selectedDomain);
        ExitDomain();
    }
}
