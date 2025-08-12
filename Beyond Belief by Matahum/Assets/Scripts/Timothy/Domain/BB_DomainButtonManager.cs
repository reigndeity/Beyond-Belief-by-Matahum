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
        HandleMouseVisibility(true);

        domainDetailsPanel.SetActive(true);
        BB_DomainDetailsUI.instance.OnOpenDomainDetails(true);
    }
    public void ExitDomainDetails()//When exiting a domain details in OpenWorldScene
    {
        domainDetailsPanel.SetActive(false);

        UI_Game uiGame = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        uiGame.ResumeGame();

        HandleMouseVisibility(false);
    }

    #region EXITING DOMAIN
    public void ExitDomain()
    {
        UI_Game uI_Game = FindFirstObjectByType<UI_Game>(FindObjectsInactive.Include);
        if (uI_Game != null) uI_Game.ResumeGame();

        domainScenePanel.SetActive(false);
        BB_DomainManager.instance.ResetDomain();
        BB_DomainManager.instance.selectedDomain = null;   

        SceneManager.sceneLoaded += OnMainSceneLoaded; // subscribe to event
        //SceneManager.LoadScene("Tim New World Tester");
        Loader.Load(3);
    }

    private void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Tim New World Tester")
        {
            Invoke(nameof(SpawnToOpenWorldWrapping),1.6f);

            SceneManager.sceneLoaded -= OnMainSceneLoaded; // unsubscribe so it doesn’t run every time   
        }
    }

    void SpawnToOpenWorldWrapping()
    {
        BB_DomainManager.instance.SpawnToOpenWorldDomainEntrance();
    }

    #endregion

    public void StayInDomain()//When choosing to stay at the domain
    {
        leaveConfirmationPanel.SetActive(false);
        HandleMouseVisibility(false);
    }

    public void ClaimRewards()//Claiming of rewards in domain
    { 
        BB_DomainManager.instance.ClaimRewards(BB_DomainManager.instance.selectedDomain);
        ExitDomain();
    }

    public void HandleMouseVisibility(bool isVisible)
    {
        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(isVisible);
    }
}
