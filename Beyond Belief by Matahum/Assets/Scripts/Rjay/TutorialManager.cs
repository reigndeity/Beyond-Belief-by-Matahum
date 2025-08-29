using System;
using System.Collections;
using Abu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [SerializeField] private UI_Game m_uiGame;
    public bool isTutorialDone;

    [Header("Script References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Tutorial Components")]
    public UI_CanvasGroup characterDetailsButton;
    public UI_CanvasGroup inventoryButton;
    public UI_CanvasGroup archiveButton;
    public UI_CanvasGroup questButton;
    public UI_CanvasGroup minimap;
    public UI_CanvasGroup normalSkill;
    public UI_CanvasGroup ultimateSkill;
    public UI_CanvasGroup agimatOne;
    public UI_CanvasGroup agimatTwo;
    public UI_CanvasGroup health;

    [Header("Tupas House")]
    public GameObject temporaryCollider;
    public GameObject tupasHouseStairs;
    public DoorInteractable tupasHouseDoor;
    public GameObject cutsceneTriggerOne;
    public GameObject cutsceneBakalNPC;
    public GameObject lewenriSacredStatue;

    [Header("Player Variables")]
    public bool tutorial_canMovementToggle = true;
    public bool tutorial_canJump = true;
    public bool tutorial_canCameraZoom = true;
    public bool tutorial_canCameraDirection = true;
    public bool tutorial_canAttack;
    public bool tutorial_canSprintAndDash = true;
    public bool tutorial_canNormalSkill = true;
    public bool tutorial_canUltimateSkill = true;
    public bool tutorial_canOpenMap = true;
    public bool tutorial_canToggleMouse = true;

    [Header("Other Variables")]
    public bool tutorial_isFirstStatueInteract = true;

    [Header("UI Tutorial")]
    public TutorialFadeImage tutorialFadeImage; // default smoothness is 0.0005

    [Header("Quest Journal UI Tutorial")]
    [SerializeField] private BB_Quest_ButtonManager m_questButtonManager;
    public GameObject questJournalTutorial;
    public TutorialHighlight mainQuestViewportTH;
    public TutorialHighlight questSelectionPanelTH;
    public TutorialHighlight questDetailsPanelTH;
    public TutorialHighlight questButtonFiltersTH;
    public TutorialHighlight claimQuestButtonTH;
    public TutorialHighlight closeQuestJournalButtonTH;
    public Button claimQuestButton;
    public Button closeQuestButton;
    public TextMeshProUGUI questJournalTextTutorial;
    public Button nextJournalTutorialButton;
    public UI_CanvasGroup nextJournalTutorialCanvasGroup;
    public TutorialHighlight nextJournalTutorialTH;
    public GameObject nonInteractablePanel;
    public int currentQuestJournalTutorial = 0;
    public GameObject claimThisTextHelper;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (!isTutorialDone)
        {
            StartTutorial();
        }
        else
        {
            cutsceneBakalNPC.SetActive(false);
        }

        nextJournalTutorialButton.onClick.AddListener(QuestJournalTutorial);
        
    }

    void StartTutorial()
    {
        PlayerCamera.Instance.HardLockCamera();
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q0_InitialTalk");

        // UI Visibility
        characterDetailsButton.FadeOut(0);
        inventoryButton.FadeOut(0);
        archiveButton.FadeOut(0);
        questButton.FadeOut(0);
        minimap.FadeOut(0);
        normalSkill.FadeOut(0);
        ultimateSkill.FadeOut(0);
        agimatOne.FadeOut(0);
        agimatTwo.FadeOut(0);   
        health.FadeOut(0);

        // UI Active State
        characterDetailsButton.gameObject.SetActive(false);
        inventoryButton.gameObject.SetActive(false);
        archiveButton.gameObject.SetActive(false);
        

        // Tupas House
        tupasHouseStairs.SetActive(false);
        temporaryCollider.SetActive(true);
        tupasHouseDoor.interactCooldown = 9999f;
        cutsceneBakalNPC.SetActive(false);

        // Misc
        lewenriSacredStatue.gameObject.layer = LayerMask.NameToLayer("Default");

        // Player Variables
        tutorial_canMovementToggle = false;
        tutorial_canJump = false;
        tutorial_canCameraDirection = false;
        tutorial_canCameraZoom = false;
        tutorial_canAttack = false;
        tutorial_canSprintAndDash = false;
        tutorial_canNormalSkill = false;
        tutorial_canUltimateSkill = false;
        tutorial_canOpenMap = false;
        tutorial_canToggleMouse = false;
        playerMovement.ToggleWalk();  

        // Other Variables
        tutorial_isFirstStatueInteract = false;
    }

    #region PLAYER BOOLEANS TUTORIAL
    public void AllowCameraDirection() => tutorial_canCameraDirection = true;
    public void AllowCameraZoom() => tutorial_canCameraZoom = true;
    public void AllowMovementToggle() => tutorial_canMovementToggle = true;
    public void AllowJump() => tutorial_canJump = true;
    public void AllowAttack() => tutorial_canAttack = true;
    public void AllowDash() => tutorial_canSprintAndDash = true;
    public void AllowNormalSkill() => tutorial_canNormalSkill = true;
    public void AllowUltimateSkill() => tutorial_canUltimateSkill = true;

    public void ShowNormalSkill() => normalSkill.FadeIn(0.5f);
    public void HideNormalSkill() => normalSkill.FadeOut(0.5f);
    public void ShowUltimateSkill() => ultimateSkill.FadeIn(0.5f);
    public void HideUltimateSkill() => ultimateSkill.FadeOut(0.5f);
    public void ShowHealth() => health.FadeIn(0.5f);

    public void ShowMinimap() => minimap.FadeIn(0.5f);
    public void HideMinimap() => minimap.FadeOut(0.5f);

    public void AllowFirstStatueInteraction() => tutorial_isFirstStatueInteract = true;
    public void AllowFullscreenMap() => tutorial_canOpenMap = true;
    public void ShowQuestJournal() => questButton.FadeIn(0.5f);
    #endregion

    #region QUEST JOURNAL UI TUTORIAL

    public void EnableQuestJournalTutorial()
    {
        questJournalTutorial.SetActive(true);
        tutorialFadeImage.enabled = true;
        nonInteractablePanel.gameObject.SetActive(true);
        questJournalTextTutorial.text = "this is where you can filter your quest and even view completed ones";
        questButtonFiltersTH.enabled = true;
    }
    public void QuestJournalTutorial()
    {
        switch(currentQuestJournalTutorial)
        {
            case 0:
                m_uiGame.questButton.onClick.RemoveListener(TutorialManager.instance.EnableQuestJournalTutorial);
                questJournalTextTutorial.text = "Here you can see your main quests and your side quest";
                questButtonFiltersTH.enabled = false;
                questSelectionPanelTH.enabled = true;
                break;
            case 1:
                questJournalTextTutorial.text = "You can also see the quest details here";
                questSelectionPanelTH.enabled = false;
                questDetailsPanelTH.enabled = true;
                break;
            case 2:
                BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q8_QuestJournal");
                nextJournalTutorialCanvasGroup.FadeOut(0.25f);
                nextJournalTutorialTH.enabled = false;
                nextJournalTutorialButton.enabled = false;

                nonInteractablePanel.SetActive(false);
                claimThisTextHelper.SetActive(true);
                questJournalTextTutorial.text = "Here you can claim, track, or untrack your current selected quest";
                
                questDetailsPanelTH.enabled = false;
                mainQuestViewportTH.enabled = true;
                claimQuestButtonTH.enabled = true;
                claimQuestButton.onClick.AddListener(QuestJournalTutorial);
                break;
            case 3:
                questJournalTextTutorial.text = "Now click on this button to resume your journey";
                mainQuestViewportTH.enabled = false;
                claimQuestButtonTH.enabled = false;
                claimThisTextHelper.SetActive(false);
                claimQuestButton.onClick.RemoveListener(QuestJournalTutorial);
                closeQuestJournalButtonTH.enabled = true;
                closeQuestButton.onClick.AddListener(QuestJournalTutorial);
                break;
            case 4:
                closeQuestJournalButtonTH.enabled = false;
                closeQuestButton.onClick.RemoveListener(QuestJournalTutorial);
                questJournalTutorial.SetActive(false);
                tutorialFadeImage.enabled = false;
                BB_QuestManager.Instance.AcceptQuestByID("A0_Q9_OneMoreThing");
                break;
        }
        currentQuestJournalTutorial++;
    }

    #endregion


}
