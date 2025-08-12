using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BB_DomainButtonManager : MonoBehaviour
{
    public static BB_DomainButtonManager instance;
    [Header("Enter Domain")]
    public Button enterDomainButton;
    public GameObject domainScenePanel;

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

    [Header("Defeated Properties")]
    public Button defeated_LeaveDomainButton;

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
        defeated_LeaveDomainButton.onClick.AddListener(ExitDomain);
    }
    public void EnterDomain() //When pressing Enter Domain in domain details in OpenWorldScene
    {        
        StartCoroutine(DelayedEnterDomain());
    }
    IEnumerator DelayedEnterDomain()
    {
        ExitDomainDetails();
        domainScenePanel.SetActive(true);

        SceneAutoSaveController autoSave = FindFirstObjectByType<SceneAutoSaveController>(FindObjectsInactive.Include);
        autoSave.SaveAll();

        yield return null; // delay in seconds

        BB_DomainManager.instance.EnterDomain();
    }

    public void OpenDomainDetails()//When interacting with a domain entrance in OpenWorldScene
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        domainDetailsPanel.SetActive(true);
        BB_DomainDetailsUI.instance.OnOpenDomainDetails(true);
    }
    public void ExitDomainDetails()//When exiting a domain details in OpenWorldScene
    {
        domainDetailsPanel.SetActive(false);

        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        uiGame.ResumeGame();
    }

    public void ExitDomain()
    {
        domainScenePanel.SetActive(false);
        BB_DomainManager.instance.ResetDomain();
        BB_DomainManager.instance.selectedDomain = null;   

        SceneManager.sceneLoaded += OnMainSceneLoaded; // subscribe to event
        SceneManager.LoadScene("Tim New World Tester");
    }

    private void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Tim New World Tester")
        {       
            BB_DomainManager.instance.SpawnToOpenWorldDomainEntrance();
            SceneManager.sceneLoaded -= OnMainSceneLoaded; // unsubscribe so it doesn’t run every time   
        }
    }

    public void StayInDomain()//When choosing to stay at the domain
    {
        leaveConfirmationPanel.SetActive(false);
    }

    public void ClaimRewards()//Claiming of rewards in domain
    {
        UI_Game uI_Game = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        if (uI_Game != null) uI_Game.ResumeGame();

        BB_DomainManager.instance.ClaimRewards(BB_DomainManager.instance.selectedDomain);
        ExitDomain();
    }
}
