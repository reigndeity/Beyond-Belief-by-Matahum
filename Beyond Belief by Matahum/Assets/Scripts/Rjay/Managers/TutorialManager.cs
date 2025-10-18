using System;
using System.Collections;
using System.Threading.Tasks;
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
    [SerializeField] private LewenriGate lewenriGate;

    [Header("Script References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private R_Inventory inventory;
    [SerializeField] private R_AgimatPanel agimatPanel;
    [SerializeField] private R_PamanaPanel pamanaPanel;

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
    public GameObject saveStatue;

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

    // tutorial for later
    public bool tutorial_canArchives = false;

    [Header("Other Variables")]
    public bool tutorial_isFirstStatueInteract = true;
    public bool tutorial_isGateOpen = true;
    public bool tutorial_isFirstSaveStatueInteract = true;

    [Header("UI Tutorial")]
    public TutorialFadeImage tutorialFadeImage; // default smoothness is 0.0005
    public GameObject journalTutorialArrow;
    public GameObject characterDetailsTutorialArrow;
    public GameObject inventoryTutorialArrow;

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
    [Header("Character Details UI Tutorial")]
    public Button weaponButtonTH;
    public Button closeCharacterDetailButton;
    public Button confirmSwitchButton;
    public TutorialHighlight confirmTextTH;

    [Header("Agimat Tutorial")]
    public Button agimatButtonTH;
    public GameObject agimatTutorial;
    public Button agimatOneTH;
    public Button agimatTwoTH;
    public Button unequipAgimatButtonTH;
    public Button equipAgimatButtonTH;
    public TutorialHighlight agimatInventoryTH;
    public TutorialHighlight agimatItemImageTH;
    public TutorialHighlight agimatItemDescriptionTH;
    public int currentAgimatTutorial = 0;
    public TextMeshProUGUI agimatTutorialText;
    public Button firstAgimatSlot;
    public Button nextAgimatTutorialButton;

    [Header("Pamana Tutorial")]
    public Button pamanaButtonTH;
    public Button attributesButtonTH;
    public int currentPamanaTutorial = 0;
    public Button firstPamanaSlot;
    public TextMeshProUGUI pamanaTutorialText;
    public GameObject pamanaTutorial;
    public Button diwataSlotButtonTH;
    public TutorialHighlight pamanaInventoryTH;
    public TutorialHighlight pamanaItemImageTH;
    public TutorialHighlight pamanaItemDescriptionTH;
    public Button equipPamanaButtonTH;
    public Button nextPamanaTutorialButtonTH;
    public TutorialHighlight attributeBackgroundTH;

    [Header("Inventory Tutorial")]
    public GameObject inventoryTutorial;
    public TextMeshProUGUI inventoryTutorialText;
    public GameObject nonInteractableInventory;
    public TutorialHighlight sortButtonsTH;
    public TutorialHighlight currentFilterTextTH;
    public TutorialHighlight inventorySlotTH;
    public Button nextInventoryTutorial;
    public int currentInventoryTutorial;
    public TutorialHighlight inventoryItemImageTH;
    public TutorialHighlight inventoryDescriptionTH;
    public Button closeInventoryButtonTH;

    [Header("Save Tutorial")]
    public Button noSaveButton;
    public Button closeSaveButton;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        PlayerCamera.Instance.HardLockCamera();
    }

    public void AllowTemporaryBooleans()
    {
        isTutorialDone = true;
        tutorial_canCameraDirection = true;
        tutorial_canCameraZoom = true;
        tutorial_canMovementToggle = true;
        tutorial_canJump = true;
        tutorial_canSprintAndDash = true;
        tutorial_canAttack = true;
        tutorial_canNormalSkill = true;
        tutorial_canUltimateSkill = true;
        tutorial_canOpenMap = true;
        ShowNormalSkill();
        ShowUltimateSkill();
        ShowHealth();
        ShowQuestJournal();
        ShowMinimap();
        ShowCharacterDetails();
        ShowAgimatOne();
        ShowAgimatTwo();
        ShowInventory();

        PlayerCamera.Instance.HardUnlockCamera();
        PlayerCamera.Instance.AdjustCamera();
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
    public void TutorialCheck()
    {
        if (!isTutorialDone)
        {
            StartTutorial();
        }
        else
        {
            cutsceneBakalNPC.SetActive(false);
            tutorial_isFirstStatueInteract = false;
            tutorial_isFirstSaveStatueInteract = false;
            PlayerCamera.Instance.HardUnlockCamera();
            PlayerCamera.Instance.AdjustCamera();
            if (tutorial_isGateOpen == true)
            {
                lewenriGate.Open();
            }
            else
            {
                lewenriGate.Close();
            }

            // Tupas House
            tupasHouseStairs.SetActive(true);
            temporaryCollider.SetActive(false);
            tupasHouseDoor.interactCooldown = 1;
            cutsceneBakalNPC.SetActive(false);

            saveStatue.gameObject.layer = LayerMask.NameToLayer("Save Statue");

        }

        nextJournalTutorialButton.onClick.AddListener(QuestJournalTutorial);

        if (tutorial_canArchives)
        {
            archiveButton.FadeIn(0.5f);
            archiveButton.GetComponent<Button>().enabled = true;
        }
        else
        {
            archiveButton.FadeOut(0);
            archiveButton.GetComponent<Button>().enabled = false;
        }
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

        // Button State
        characterDetailsButton.GetComponent<Button>().enabled = false;
        inventoryButton.GetComponent<Button>().enabled = false;
        archiveButton.GetComponent<Button>().enabled = false;


        // Tupas House
        tupasHouseStairs.SetActive(false);
        temporaryCollider.SetActive(true);
        tupasHouseDoor.interactCooldown = 9999f;
        cutsceneBakalNPC.SetActive(false);

        // Misc
        lewenriSacredStatue.gameObject.layer = LayerMask.NameToLayer("Default");
        saveStatue.gameObject.layer = LayerMask.NameToLayer("Default");


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
        tutorial_canArchives = false;
        playerMovement.ToggleWalk();

        // Other Variables
        tutorial_isFirstStatueInteract = false;
        tutorial_isGateOpen = false;

        lewenriGate.Close();
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
    public void HideHealth() => health.FadeOut(0.5f);

    public void ShowMinimap() => minimap.FadeIn(0.5f);
    public void HideMinimap() => minimap.FadeOut(0.5f);

    public void AllowFirstStatueInteraction() => tutorial_isFirstStatueInteract = true;
    public void AllowFullscreenMap() => tutorial_canOpenMap = true;
    public void DisableFullscreenMap() => tutorial_canOpenMap = false;
    public void ShowQuestJournal()
    {
        questButton.GetComponent<Button>().enabled = true;
        questButton.FadeIn(0.5f);
    }
    public void HideQuestJournal()
    {
        questButton.GetComponent<Button>().enabled = false;
        questButton.FadeOut(0.5f);
    }

    public void ShowAgimatOne() => agimatOne.FadeIn(0.5f);
    public void HideAgimatOne() => agimatOne.FadeOut(0.5f);
    public void ShowAgimatTwo() => agimatTwo.FadeIn(0.5f);
    public void HideAgimatTwo() => agimatTwo.FadeOut(0.5f);


    public void HideCharacterDetails()
    {
        characterDetailsButton.GetComponent<Button>().enabled = false;
        characterDetailsButton.FadeOut(0.5f);
    }

    public void ShowCharacterDetails()
    {
        characterDetailsButton.GetComponent<Button>().enabled = true;
        characterDetailsButton.FadeIn(0.5f);
    }

    public void ShowInventory()
    {
        inventoryButton.GetComponent<Button>().enabled = true;
        inventoryButton.FadeIn(0.5f);
    }
    public void HideInventory()
    {
        inventoryButton.GetComponent<Button>().enabled = false;
        inventoryButton.FadeOut(0.5f);
    }

    #endregion

    #region QUEST JOURNAL UI TUTORIAL

    public void EnableQuestJournalTutorial()
    {
        questJournalTutorial.SetActive(true);
        tutorialFadeImage.enabled = true;
        nonInteractablePanel.gameObject.SetActive(true);
        questJournalTextTutorial.text = "this is where you can filter your quest and even view completed ones";
        questJournalTextTutorial.GetComponent<TutorialHighlight>().enabled = true;
        questButtonFiltersTH.enabled = true;

        journalTutorialArrow.SetActive(false);
    }
    public void QuestJournalTutorial()
    {
        switch (currentQuestJournalTutorial)
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
                BB_QuestJournalUI.instance.ChangeTrackerButtonDisplay(BB_QuestJournalUI.instance.currentSelectedQuest);
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
            questJournalTextTutorial.GetComponent<TutorialHighlight>().enabled = false;
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

    #region AGIMAT UI TUTORIAL
    public void EnableAgimatSlotOneTutorial()
    {
        agimatTutorial.SetActive(true);
        tutorialFadeImage.enabled = true;
        agimatTutorialText.text = "Click on the button";
        agimatButtonTH.GetComponent<TutorialHighlight>().enabled = true;
        agimatButtonTH.onClick.AddListener(AgimatTutorial);

        characterDetailsTutorialArrow.SetActive(false);
    }
    public void EnableAgimatSlotTwoTutorial()
    {
        agimatTutorial.SetActive(true);
        tutorialFadeImage.enabled = true;
        agimatTutorialText.text = "Click on the button";
        agimatButtonTH.GetComponent<TutorialHighlight>().enabled = true;
        agimatButtonTH.onClick.AddListener(AgimatTutorial);

        characterDetailsTutorialArrow.SetActive(false);
    }
    public void AgimatTutorial()
    {
        switch (currentAgimatTutorial)
        {
            case 0:
                agimatButtonTH.onClick.RemoveListener(AgimatTutorial);
                agimatButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                m_uiGame.characterDetailsButton.onClick.RemoveListener(TutorialManager.instance.EnableAgimatSlotOneTutorial);

                agimatTutorialText.text = "Now select the first empty agimat slot";
                agimatOneTH.GetComponent<TutorialHighlight>().enabled = true;
                agimatOneTH.onClick.AddListener(AgimatTutorial);
                break;
            case 1:
                agimatOneTH.GetComponent<TutorialHighlight>().enabled = false;
                agimatOneTH.onClick.RemoveListener(AgimatTutorial);
                agimatTutorialText.text = "Select the ngipin ng kidlat agimat";
                agimatInventoryTH.enabled = true;
                agimatPanel.RefreshAgimatList();
                StartCoroutine(AttachAgimatTutorialToFirstAgimatSlot());
                break;
            case 2:
                agimatTutorialText.text = "Here you can see the type of agimat that you have";
                firstAgimatSlot.onClick.RemoveListener(AgimatTutorial);
                agimatInventoryTH.enabled = false;
                nextAgimatTutorialButton.gameObject.SetActive(true);
                StartCoroutine(ShowNextAgimatTutorialButton());
                nextAgimatTutorialButton.onClick.AddListener(AgimatTutorial);
                agimatItemDescriptionTH.enabled = true;
                agimatItemImageTH.enabled = true;
                agimatOneTH.GetComponent<TutorialHighlight>().enabled = false;
                break;
            case 3:
                agimatItemDescriptionTH.enabled = false;
                agimatItemImageTH.enabled = false;
                agimatTutorialText.text = "Try equipping it to slot 1";
                equipAgimatButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                equipAgimatButtonTH.onClick.AddListener(AgimatTutorial);

                nextAgimatTutorialButton.gameObject.SetActive(false);
                break;
            case 4:
                agimatTutorialText.text = "Now select this button to continue";
                equipAgimatButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                equipAgimatButtonTH.onClick.RemoveListener(AgimatTutorial);
                closeCharacterDetailButton.GetComponent<TutorialHighlight>().enabled = true;
                closeCharacterDetailButton.onClick.AddListener(CloseAndAcceptAgimatTrainingP2);
                break;
            case 5:
                agimatButtonTH.onClick.RemoveListener(AgimatTutorial);
                agimatButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                m_uiGame.characterDetailsButton.onClick.RemoveListener(TutorialManager.instance.EnableAgimatSlotTwoTutorial);

                agimatTutorialText.text = "Now select the second empty agimat slot";
                agimatTwoTH.GetComponent<TutorialHighlight>().enabled = true;
                agimatTwoTH.onClick.AddListener(AgimatTutorial);
                break;
            case 6:
                agimatTwoTH.GetComponent<TutorialHighlight>().enabled = false;
                agimatTwoTH.onClick.RemoveListener(AgimatTutorial);
                agimatTutorialText.text = "Select the ngipin ng kidlat agimat";
                agimatInventoryTH.enabled = true;
                agimatPanel.RefreshAgimatList();
                StartCoroutine(AttachAgimatTutorialToFirstAgimatSlot());
                break;
            case 7:
                agimatInventoryTH.enabled = false;
                firstAgimatSlot.onClick.RemoveListener(AgimatTutorial);
                agimatTutorialText.text = "Try equipping it to slot 2";
                equipAgimatButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                equipAgimatButtonTH.onClick.AddListener(AgimatTutorial);
                break;
            case 8:
                agimatTutorialText.text = "Click on the confirm button";
                equipAgimatButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                equipAgimatButtonTH.onClick.RemoveListener(AgimatTutorial);
                confirmSwitchButton.GetComponent<TutorialHighlight>().enabled = true;
                confirmSwitchButton.onClick.AddListener(AgimatTutorial);
                confirmTextTH.enabled = true;
                break;
            case 9:
                agimatTutorialText.text = "Now click on the close button";
                confirmSwitchButton.GetComponent<TutorialHighlight>().enabled = false;
                confirmSwitchButton.onClick.RemoveListener(AgimatTutorial);
                confirmTextTH.enabled = false;
                closeCharacterDetailButton.GetComponent<TutorialHighlight>().enabled = true;
                closeCharacterDetailButton.onClick.AddListener(CloseAndAcceptAgimatTrainingP4);
                break;

        }
        currentAgimatTutorial++;
    }

    private IEnumerator AttachAgimatTutorialToFirstAgimatSlot()
    {
        yield return null; // wait one frame so RefreshAgimatList is done

        firstAgimatSlot = agimatPanel.GetSlotButton(0);

        if (firstAgimatSlot != null)
        {
            firstAgimatSlot.onClick.AddListener(AgimatTutorial);
        }
        else
        {
            Debug.LogWarning("⚠️ First Agimat Slot not found.");
        }
    }
    private IEnumerator ShowNextAgimatTutorialButton()
    {
        // wait 1 second first
        yield return new WaitForSecondsRealtime(1f);

        CanvasGroup cg = nextAgimatTutorialButton.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float duration = 0.25f;
        float elapsed = 0f;

        // start from current alpha (maybe 0)
        float startAlpha = cg.alpha;
        float endAlpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // use unscaled so it ignores Time.timeScale
            float t = Mathf.Clamp01(elapsed / duration);
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        nextAgimatTutorialButton.GetComponent<TutorialHighlight>().enabled = true;
        cg.alpha = 1f; // make sure it ends at 1
    }

    private void CloseAndAcceptAgimatTrainingP2()
    {
        closeCharacterDetailButton.GetComponent<TutorialHighlight>().enabled = false;
        agimatTutorial.SetActive(false);
        tutorialFadeImage.enabled = false;
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q11_AgimatSkillOne");
        BB_QuestManager.Instance.ClaimRewardsByID("A0_Q11_AgimatTraining_P1");
        closeCharacterDetailButton.onClick.RemoveListener(CloseAndAcceptAgimatTrainingP2);
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q11_AgimatTraining_P2");
        HideCharacterDetails();
    }
    private void CloseAndAcceptAgimatTrainingP4()
    {
        closeCharacterDetailButton.GetComponent<TutorialHighlight>().enabled = false;
        agimatTutorial.SetActive(false);
        tutorialFadeImage.enabled = false;
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q11_AgimatSkillTwo");
        BB_QuestManager.Instance.ClaimRewardsByID("A0_Q11_AgimatTraining_P3");
        closeCharacterDetailButton.onClick.RemoveListener(CloseAndAcceptAgimatTrainingP4);
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q11_AgimatTraining_P4");
        HideCharacterDetails();
        m_uiGame.UnBlur();
    }
    #endregion

    #region PAMANA TUTORIAL
    public void EnablePamanaTutorial()
    {
        pamanaTutorial.SetActive(true);
        tutorialFadeImage.enabled = true;
        pamanaTutorialText.text = "Here you can see your current stats";
        attributesButtonTH.GetComponent<TutorialHighlight>().enabled = true;
        attributeBackgroundTH.enabled = true;
        nextPamanaTutorialButtonTH.onClick.AddListener(PamanaTutorial);
        characterDetailsTutorialArrow.SetActive(false);
    }

    public void PamanaTutorial()
    {
        switch (currentPamanaTutorial)
        {
            case 0:
                attributeBackgroundTH.enabled = false;
                attributesButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                nextPamanaTutorialButtonTH.gameObject.SetActive(false);
                nextPamanaTutorialButtonTH.onClick.RemoveListener(PamanaTutorial);
                nextPamanaTutorialButtonTH.GetComponent<CanvasGroup>().alpha = 0;
                nextPamanaTutorialButtonTH.GetComponent<TutorialHighlight>().enabled = false;


                pamanaTutorialText.text = "Now click on this button to check your pamana";
                pamanaButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                pamanaButtonTH.onClick.AddListener(PamanaTutorial);
                break;
            case 1:
                pamanaButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                pamanaButtonTH.onClick.RemoveListener(PamanaTutorial);

                pamanaTutorialText.text = "Click on the diwata slot";
                diwataSlotButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                diwataSlotButtonTH.onClick.AddListener(PamanaTutorial);

                break;
            case 2:
                diwataSlotButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                diwataSlotButtonTH.onClick.AddListener(PamanaTutorial);

                pamanaTutorialText.text = "Now click on the pamana";
                pamanaInventoryTH.enabled = true;
                StartCoroutine(AttachPamanaTutorialToFirstPamanaSlot());
                break;
            case 3:
                pamanaInventoryTH.enabled = false;
                firstPamanaSlot.onClick.RemoveListener(PamanaTutorial);

                pamanaTutorialText.text = "Here you can see the main stat that it has";
                pamanaItemImageTH.enabled = true;
                StartCoroutine(ShowNextPamanaTutorialButton());
                nextPamanaTutorialButtonTH.onClick.AddListener(PamanaTutorial);
                break;
            case 4:
                pamanaItemImageTH.enabled = false;

                pamanaTutorialText.text = "This shows what set it comes from and what bonuses you can unlock";
                pamanaItemDescriptionTH.enabled = true;
                break;
            case 5:
                pamanaItemDescriptionTH.enabled = false;
                nextPamanaTutorialButtonTH.gameObject.SetActive(false);
                nextPamanaTutorialButtonTH.onClick.RemoveListener(PamanaTutorial);
                nextPamanaTutorialButtonTH.GetComponent<CanvasGroup>().alpha = 0;
                nextPamanaTutorialButtonTH.GetComponent<TutorialHighlight>().enabled = false;

                pamanaTutorialText.text = "Now let's equip it";
                equipPamanaButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                equipPamanaButtonTH.onClick.AddListener(PamanaTutorial);
                break;
            case 6:
                equipPamanaButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                equipPamanaButtonTH.onClick.RemoveListener(PamanaTutorial);

                pamanaTutorialText.text = "Let's go back and see your stats";
                attributesButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                attributesButtonTH.onClick.AddListener(PamanaTutorial);
                break;
            case 7:
                attributesButtonTH.GetComponent<TutorialHighlight>().enabled = false;
                attributesButtonTH.onClick.RemoveListener(PamanaTutorial);

                pamanaTutorialText.text = "As you can see, your stats have increased";
                attributeBackgroundTH.enabled = true;
                StartCoroutine(ShowNextPamanaTutorialButton());
                nextPamanaTutorialButtonTH.onClick.AddListener(PamanaTutorial);
                break;
            case 8:
                attributeBackgroundTH.enabled = false;
                nextPamanaTutorialButtonTH.gameObject.SetActive(false);
                m_uiGame.characterDetailsButton.onClick.RemoveListener(PamanaTutorial);

                pamanaTutorialText.text = "Now click on this button to resume your journey";
                closeCharacterDetailButton.GetComponent<TutorialHighlight>().enabled = true;
                closeCharacterDetailButton.onClick.AddListener(ClosePamanaTutorial);

                break;

        }

        currentPamanaTutorial++;
    }
    private IEnumerator ShowNextPamanaTutorialButton()
    {
        // wait 1 second first
        nextPamanaTutorialButtonTH.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        CanvasGroup cg = nextPamanaTutorialButtonTH.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float duration = 0.25f;
        float elapsed = 0f;

        // start from current alpha (maybe 0)
        float startAlpha = cg.alpha;
        float endAlpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // use unscaled so it ignores Time.timeScale
            float t = Mathf.Clamp01(elapsed / duration);
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        nextPamanaTutorialButtonTH.GetComponent<TutorialHighlight>().enabled = true;
        cg.alpha = 1f; // make sure it ends at 1
    }

    private IEnumerator AttachPamanaTutorialToFirstPamanaSlot()
    {
        yield return null; // wait one frame so RefreshAgimatList is done

        firstPamanaSlot = pamanaPanel.GetSlotButton(0);

        if (firstPamanaSlot != null)
        {
            firstPamanaSlot.onClick.AddListener(PamanaTutorial);
        }
        else
        {
            Debug.LogWarning("⚠️ First Pamana Slot not found.");
        }
    }
    private void ClosePamanaTutorial()
    {
        pamanaTutorial.SetActive(false);
        tutorialFadeImage.enabled = false;
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q12_P1_Pamana");
        BB_QuestManager.Instance.ClaimRewardsByID("A0_Q12_PamanaTraining_P1");
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q12_PamanaTraining_P2");
        m_uiGame.UnBlur();
    }
    #endregion

    #region INVENTORY TUTORIAL
    public void EnableInventoryTutorial()
    {
        inventoryTutorial.SetActive(true);
        tutorialFadeImage.enabled = true;
        inventoryTutorialText.text = "These buttons filter out the types of the item";
        nonInteractableInventory.gameObject.SetActive(true);
        sortButtonsTH.enabled = true;

        nextInventoryTutorial.onClick.AddListener(InventoryTutorial);
        inventoryTutorialArrow.SetActive(false);
    }
    public void InventoryTutorial()
    {
        switch (currentInventoryTutorial)
        {
            case 0:
                sortButtonsTH.enabled = false;
                m_uiGame.OnClickAgimatFilter();
                inventoryTutorialText.text = "As of now, we are currently on the agimat filter";
                currentFilterTextTH.enabled = true;
                break;
            case 1:
                currentFilterTextTH.enabled = false;

                inventoryTutorialText.text = "You can see the details of the item you selected here";
                inventorySlotTH.enabled = true;
                inventoryDescriptionTH.enabled = true;
                inventoryItemImageTH.enabled = true;
                break;
            case 2:
                inventorySlotTH.enabled = false;
                inventoryDescriptionTH.enabled = false;
                inventoryItemImageTH.enabled = false;
                nextInventoryTutorial.gameObject.SetActive(false);
                nonInteractableInventory.gameObject.SetActive(false);

                inventoryTutorialText.text = "Now click on this button to resume your journey";
                closeInventoryButtonTH.GetComponent<TutorialHighlight>().enabled = true;
                closeInventoryButtonTH.onClick.AddListener(CloseAndAcceptMainQuest);
                break;
        }
        currentInventoryTutorial++;
    }

    public void CloseAndAcceptMainQuest()
    {
        inventoryTutorial.SetActive(false);
        tutorialFadeImage.enabled = false;
        m_uiGame.inventoryButton.onClick.RemoveListener(EnableInventoryTutorial);
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q13_Backpack");
        isTutorialDone = true;
        m_uiGame.UnBlur();
    }
    #endregion

    #region  SAVE TUTORIAL
    public void SaveTutorial()
    {
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q1.1_Statue");
    }
    public void ContinueQuestAfterSave()
    {
        StartCoroutine(ContinueQuestAfterSaving());
    }
    public IEnumerator ContinueQuestAfterSaving()
    {
        BB_QuestManager.Instance.ClaimRewardsByID("A1_Q1.1_Amihan'sOrder_P2");
        yield return new WaitForSeconds(1f);
        BB_QuestManager.Instance.AcceptQuestByID("A1_Q1_Tupas'Request_P2");
        yield return new WaitForSeconds(1f);
        _ = SaveProgressAsync();
    }
    async Task SaveProgressAsync()
    {
        await GameManager.instance.SaveAll();
    }
    #endregion
}
