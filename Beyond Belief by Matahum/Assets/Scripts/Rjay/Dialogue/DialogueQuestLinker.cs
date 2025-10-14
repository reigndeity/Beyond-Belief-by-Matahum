using System.Collections;
using System.Threading.Tasks;
using MTAssets.EasyMinimapSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueQuestLinker : MonoBehaviour
{
    private Player m_player;
    private string lastTrackedQuestID = "";
    public bool isDelayAccept;
    [SerializeField] private UI_Game m_uiGame;
    private PlayerInput m_playerInput;
    [SerializeField] R_Inventory inventory;
    [SerializeField] R_InventoryUI inventoryUI;
    private PlayerSkills m_playerSkills;

    [Header("All NPCs Dialogue State Holders")]
    public DialogueStateHolder layag;
    public DialogueSequence layagSequence;
    public DialogueStateHolder amihanGuard;
    public DialogueStateHolder bakal;
    public DialogueStateHolder bangkaw;
    public DialogueStateHolder besik;
    public DialogueStateHolder tupas;
    public DialogueStateHolder nunoSaPunso;
    public DialogueStateHolder mangkukulam;
    public DialogueStateHolder albularyo;
    
    [Header("Quest Minimap Item")]
    [SerializeField] private GameObject questMinimapItem; // ✅ single persistent prefab
    private Transform currentTrackedTransform;

    [Header("Quest Marker")]
    [SerializeField] private QuestMarker markerPrefab;       // Prefab of quest marker (UI only)
    [SerializeField] private Transform uiMarkerParent;       // Parent under PlayerHUD UI
    private QuestMarker activeMarker;                        // Currently active marker
    [SerializeField] private Sprite mainQuestSprite;
    [SerializeField] private Sprite sideQuestSprite;
    [Header("Marker Cooldown")]
    [SerializeField] private float markerCooldown = 5f; // seconds
    private float lastMarkerTime = -999f;

    [Header("Act 0 Components")]
    [SerializeField] GameObject movementTutorialPopUp;
    [SerializeField] GameObject A0_Q0_InitialTalk_NQP;
    [SerializeField] private Transform SwordTrainingDummies;
    [SerializeField] Collider playerWeaponCollider;
    [SerializeField] GameObject normalTrainingAreaMode;
    [SerializeField] GameObject trainingAreaMode;
    public TimelineAsset A0_Q3_BangkawTraining_P2_Cutscene;
    [SerializeField] private Transform normalSkillTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraining_P3_Cutscene;
    [SerializeField] private Transform ultimateSkillTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraining_P4_Cutscene;
    private int dashAmount;
    public TimelineAsset A0_Q4_TrainingWithBangkaw_Cutscene;
    [SerializeField] private MinimapRenderer playerMinimapRenderer;
    [SerializeField] GameObject fullscreenMapPopUp;
    public R_ItemData[] ngipinNgKidlatAgimat;
    public TimelineAsset A0_Q11_AgimatTraining_P1_Cutscene;
    public TimelineAsset A0_Q11_AgimatTraining_P2_Cutscene;
    [SerializeField] private Transform agimatSkillOneTrainingDummies;
    public TimelineAsset A0_Q11_AgimatTraining_P3_Cutscene;
    public TimelineAsset A0_Q11_AgimatTraining_P4_Cutscene;
    [SerializeField] private Transform agimatSkillTwoTrainingDummies;
    public TimelineAsset A0_Q12_PamanaTraining_P1_Cutscene;
    public R_ItemData[] adlaoPamana;
    public TimelineAsset A0_Q12_PamanaTraining_P2_Cutscene;
    [SerializeField] GameObject inventoryPopUp;
    [Header("Act 1 Components")]
    [SerializeField] SaveInteractable lewenriSaveInteractable;
    public GameObject duwendeCamp;
    public GameObject garlicParent;
    public MinimapItem garlicHighlight;
    public FogAreaReveal duwendeAreaMap;
    public GameObject mapPinPopUp;
    public GameObject besikNPC;
    public FogAreaReveal nunoSaPunsoAreaMap;
    public GameObject nunoMound;
    public GameObject nunoBossFightTrigger;
    public GameObject nunoSaveTrigger;
    public GameObject nunoSaPunsoCharacter;
    [Header("Act 2 Components")]
    public GameObject mangkukulamHut;
    public GameObject mangkukulamFirstTriggers;
    public GameObject mangkukulamNpc;
    public GameObject firstFakeBaleteTree;
    public GameObject secondFakeBaleteTree;
    public GameObject thirdFakeBaleteTree;
    public GameObject mangkukulamSecondTriggers;
    private bool isAlbularyoUnlocked;
    public GameObject albularyoNpc;
    public GameObject albularyoCanvas;
    public Transform a2_q5_playerTransform;
    public GameObject a2_q5_questNotify;
    public GameObject mangkukulamThirdTriggers;

    public Transform a2_q10_playerTransform;
    public Transform a2_q10_albularyoTransform;
    public GameObject repairFirstBalete;
    public GameObject repairSecondBalete;
    public GameObject repairThirdBalete;

    public Transform a2_q12_albularyoTransform;
    public GameObject portalInteractable;
    void OnEnable()
    {
        PlayerMovement.OnDashStarted += DashCounter;
    }

    void OnDisable()
    {
        PlayerMovement.OnDashStarted -= DashCounter;
    }

    void Awake()
    {
        m_playerInput = FindFirstObjectByType<PlayerInput>();
        m_playerSkills = FindFirstObjectByType<PlayerSkills>();
        m_player = FindFirstObjectByType<Player>();
    }

    void Start()
    {
        Invoke("QuestPropertiesCheck", 0.5f);
    }

    void Update()
    {
        var tracked = BB_QuestHUD.instance?.trackedQuest;
        if (tracked == null) return;

        string currentQuestID = tracked.questID;

        // ==============================
        // Press V → Spawn / Replace quest marker
        // ==============================
        if (Input.GetKeyDown(m_playerInput.questGuideKey))
        {
            AddActiveMarker(currentQuestID, tracked);
        }

        // === ✅ Quest Minimap Item follow logic ===
        Transform questTarget = GetQuestTargetTransform(currentQuestID);

        if (questMinimapItem != null)
        {
            if (questTarget != null)
            {
                if (currentTrackedTransform != questTarget)
                {
                    questMinimapItem.transform.SetParent(questTarget);
                    questMinimapItem.transform.localPosition = Vector3.zero;
                    questMinimapItem.SetActive(true);
                    currentTrackedTransform = questTarget;
                }
            }
            else
            {
                questMinimapItem.SetActive(false);
                currentTrackedTransform = null;
            }
        }

        // ==============================
        // Quest tracking state changes
        // ==============================
        if (currentQuestID != lastTrackedQuestID)
        {
            lastTrackedQuestID = currentQuestID;

            // disable all quest-related objects first
            A0_Q0_InitialTalk_NQP.SetActive(false);

            switch (currentQuestID)
            {
                #region ACT 0 QUESTS
                case "A0_Q0_InitialTalk":
                    bakal.SetDialogueState("A0_Q0_InitialTalk");
                    amihanGuard.SetDialogueState("A0_Q0_InitialTalk");
                    ApplyStates(bakal, amihanGuard);
                    A0_Q0_InitialTalk_NQP.SetActive(true);
                    TutorialManager.instance.cutsceneTriggerOne.SetActive(true);
                    movementTutorialPopUp.SetActive(true);
                    break;

                case "A0_Q1_FindAndTalkToTupas":
                    tupas.SetDialogueState("A0_Q1_FindAndTalkToTupas");
                    bakal.SetDialogueState("A0_Q1_FindAndTalkToTupas");
                    bangkaw.SetDialogueState("A0_Q1_FindAndTalkToTupas");
                    ApplyStates(tupas, bakal, bangkaw);
                    break;

                case "A0_Q2_FindAndTalkToBangkaw":
                    tupas.SetDialogueState("A0_Q2_FindAndTalkToBangkaw");
                    bakal.SetDialogueState("Default");
                    bangkaw.SetDialogueState("A0_Q2_FindAndTalkToBangkaw");
                    ApplyStates(tupas, bakal, bangkaw);
                    AddActiveMarker(currentQuestID, tracked);

                    break;

                case "A0_Q3_Bangkaw'sTraining_P1":
                    tupas.SetDialogueState("Default");
                    bakal.SetDialogueState("Default");
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraining_P1");
                    ApplyStates(tupas, bakal, bangkaw);
                    break;

                case "A0_Q3_Bangkaw'sTraining_P2":
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraining_P2");
                    playerWeaponCollider.enabled = false;
                    ApplyStates(bangkaw);

                    TutorialManager.instance.tutorial_canAttack = false;
                    CutsceneManager.Instance.StartCutscene(A0_Q3_BangkawTraining_P2_Cutscene);

                    m_playerSkills.normalSkillCooldown = 5;
                    break;

                case "A0_Q3_Bangkaw'sTraining_P3":
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraining_P3");
                    ApplyStates(bangkaw);

                    TutorialManager.instance.tutorial_canAttack = false;
                    TutorialManager.instance.tutorial_canNormalSkill = false;
                    TutorialManager.instance.HideNormalSkill();
                    CutsceneManager.Instance.StartCutscene(A0_Q3_BangkawTraining_P3_Cutscene);

                    m_playerSkills.ultimateSkillCooldown = 5;
                    break;

                case "A0_Q3_Bangkaw'sTraining_P4":
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraining_P4");
                    ApplyStates(bangkaw);

                    TutorialManager.instance.tutorial_canUltimateSkill = false;
                    TutorialManager.instance.HideUltimateSkill();
                    CutsceneManager.Instance.StartCutscene(A0_Q3_BangkawTraining_P4_Cutscene);
                    break;

                case "A0_Q4_TrainingWithBangkaw":
                    bangkaw.SetDialogueState("A0_Q4_TrainingWithBangkaw");
                    ApplyStates(bangkaw);
                    CutsceneManager.Instance.StartCutscene(A0_Q4_TrainingWithBangkaw_Cutscene);

                    m_playerSkills.normalSkillCooldown = 12;
                    m_playerSkills.ultimateSkillCooldown = 30;
                    break;

                case "A0_Q5_ReturnToTupas":
                    bangkaw.SetDialogueState("Default");
                    tupas.SetDialogueState("A0_Q5_ReturnToTupas");
                    AddActiveMarker(currentQuestID, tracked);
                    ApplyStates(bangkaw, tupas);

                    TutorialManager.instance.tutorial_canAttack = true;
                    TutorialManager.instance.tutorial_canNormalSkill = true;
                    TutorialManager.instance.tutorial_canUltimateSkill = true;
                    TutorialManager.instance.ShowNormalSkill();
                    TutorialManager.instance.ShowUltimateSkill();
                    TutorialManager.instance.ShowHealth();
                    break;

                case "A0_Q6_SacredStatue":
                    tupas.SetDialogueState("A0_Q6_SacredStatue");
                    ApplyStates(tupas);
                    AddActiveMarker(currentQuestID, tracked);

                    TutorialManager.instance.lewenriSacredStatue.gameObject.layer = LayerMask.NameToLayer("Teleporter");
                    m_uiGame.closeMapButton.onClick.AddListener(FirstStatueInteraction);
                    if (questMinimapItem != null)
                    { 
                        questMinimapItem.SetActive(false);
                    }
                    break;

                case "A0_Q7_KeepingTrack":
                    tupas.SetDialogueState("A0_Q7_KeepingTrack");
                    ApplyStates(tupas);
                    AddActiveMarker(currentQuestID, tracked);
                    m_uiGame.closeMapButton.onClick.RemoveListener(FirstStatueInteraction);
                    break;
                case "A0_Q8_QuestJournal":
                    tupas.SetDialogueState("A0_Q8_QuestJournal");
                    ApplyStates(tupas);
                    m_uiGame.questButton.onClick.AddListener(TutorialManager.instance.EnableQuestJournalTutorial);
                    break;
                case "A0_Q9_OneMoreThing":
                    tupas.SetDialogueState("A0_Q9_OneMoreThing");
                    ApplyStates(tupas);
                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A0_Q10_ReturnToBangkaw":
                    tupas.SetDialogueState("A0_Q10_ReturnToBangkaw");
                    bangkaw.SetDialogueState("A0_Q10_ReturnToBangkaw");
                    ApplyStates(tupas, bangkaw);
                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A0_Q11_AgimatTraining_P1":
                    bangkaw.SetDialogueState("A0_Q11_AgimatTraining_P1");
                    ApplyStates(bangkaw);
                    R_GeneralItemSpawner.instance.SpawnSingleAgimat(ngipinNgKidlatAgimat);

                    inventoryUI.RefreshUI();
                    m_uiGame.characterDetailsButton.onClick.AddListener(TutorialManager.instance.EnableAgimatSlotOneTutorial);
                    CutsceneManager.Instance.PlayCutscene(A0_Q11_AgimatTraining_P1_Cutscene);


                    TutorialManager.instance.tutorial_canAttack = false;
                    TutorialManager.instance.tutorial_canNormalSkill = false;
                    TutorialManager.instance.tutorial_canUltimateSkill = false;
                    TutorialManager.instance.tutorial_canOpenMap = false;
                    TutorialManager.instance.HideNormalSkill();
                    TutorialManager.instance.HideUltimateSkill();
                    TutorialManager.instance.HideHealth();
                    TutorialManager.instance.HideQuestJournal();
                    TutorialManager.instance.HideMinimap();
                    break;
                case "A0_Q11_AgimatTraining_P2":
                    bangkaw.SetDialogueState("A0_Q11_AgimatTraining_P2");
                    ApplyStates(bangkaw);
                    CutsceneManager.Instance.PlayCutscene(A0_Q11_AgimatTraining_P2_Cutscene);
                    TutorialManager.instance.ShowAgimatOne();
                    break;
                case "A0_Q11_AgimatTraining_P3":
                    bangkaw.SetDialogueState("A0_Q11_AgimatTraining_P3");
                    ApplyStates(bangkaw);
                    CutsceneManager.Instance.PlayCutscene(A0_Q11_AgimatTraining_P3_Cutscene);
                    TutorialManager.instance.HideAgimatOne();
                    m_uiGame.characterDetailsButton.onClick.AddListener(TutorialManager.instance.EnableAgimatSlotTwoTutorial);
                    break;
                case "A0_Q11_AgimatTraining_P4":
                    bangkaw.SetDialogueState("A0_Q11_AgimatTraining_P4");
                    ApplyStates(bangkaw);
                    CutsceneManager.Instance.PlayCutscene(A0_Q11_AgimatTraining_P4_Cutscene);
                    TutorialManager.instance.ShowAgimatTwo();
                    break;
                case "A0_Q12_PamanaTraining_P1":
                    bangkaw.SetDialogueState("A0_Q12_PamanaTraining_P1");
                    ApplyStates(bangkaw);
                    CutsceneManager.Instance.PlayCutscene(A0_Q12_PamanaTraining_P1_Cutscene);
                    TutorialManager.instance.HideAgimatTwo();

                    m_uiGame.characterDetailsButton.onClick.AddListener(TutorialManager.instance.EnablePamanaTutorial);
                    R_GeneralItemSpawner.instance.SpawnSinglePamana(adlaoPamana);
                    inventoryUI.RefreshUI();
                    break;
                case "A0_Q12_PamanaTraining_P2":
                    bangkaw.SetDialogueState("A0_Q12_PamanaTraining_P2");
                    tupas.SetDialogueState("A0_Q12_PamanaTraining_P2");
                    ApplyStates(bangkaw, tupas);
                    CutsceneManager.Instance.PlayCutscene(A0_Q12_PamanaTraining_P2_Cutscene);
                    m_uiGame.characterDetailsButton.onClick.RemoveListener(TutorialManager.instance.EnablePamanaTutorial);


                    TutorialManager.instance.tutorial_canAttack = true;
                    TutorialManager.instance.tutorial_canNormalSkill = true;
                    TutorialManager.instance.tutorial_canUltimateSkill = true;
                    TutorialManager.instance.tutorial_canOpenMap = true;
                    TutorialManager.instance.ShowNormalSkill();
                    TutorialManager.instance.ShowUltimateSkill();
                    TutorialManager.instance.ShowHealth();
                    TutorialManager.instance.ShowQuestJournal();
                    TutorialManager.instance.ShowMinimap();
                    TutorialManager.instance.ShowCharacterDetails();
                    TutorialManager.instance.ShowAgimatOne();
                    TutorialManager.instance.ShowAgimatTwo();

                    normalTrainingAreaMode.SetActive(true);
                    trainingAreaMode.SetActive(false);
                    break;
                case "A0_Q13_BackpackTraining":
                    tupas.SetDialogueState("A0_Q13_BackpackTraining");
                    ApplyStates(tupas);
                    RemoveActiveMarker();
                    TutorialManager.instance.ShowInventory();
                    inventoryPopUp.SetActive(true);
                    m_uiGame.inventoryButton.onClick.AddListener(TutorialManager.instance.EnableInventoryTutorial);
                    break;
                #endregion

                #region ACT 1 QUESTS
                case "A1_Q1_Tupas'Request_P1":
                    tupas.SetDialogueState("A1_Q1_Tupas'Request_P1");
                    ApplyStates(tupas);
                    AddActiveMarker(currentQuestID, tracked);
                    FindFirstObjectByType<ChestManager>().OnLewenriQuestComplete();
                    break;
                case "A1_Q1.1_Amihan'sOrder_P1":
                    tupas.SetDialogueState("A1_Q1_Tupas'Request_P2");
                    amihanGuard.SetDialogueState("A1_Q1.1_Amihan'sOrder_P1");
                    ApplyStates(tupas, amihanGuard);
                    break;
                case "A1_Q1.1_Amihan'sOrder_P2":
                    amihanGuard.SetDialogueState("A1_Q1.1_Amihan'sOrder_P2");
                    ApplyStates(amihanGuard);
                    TutorialManager.instance.saveStatue.gameObject.layer = LayerMask.NameToLayer("Save Statue");
                    lewenriSaveInteractable.onInteract.AddListener(TutorialManager.instance.SaveTutorial);
                    TutorialManager.instance.closeSaveButton.onClick.AddListener(TutorialManager.instance.ContinueQuestAfterSave);
                    TutorialManager.instance.noSaveButton.onClick.AddListener(TutorialManager.instance.ContinueQuestAfterSave);

                    if (questMinimapItem != null)
                    { 
                        questMinimapItem.SetActive(false);
                    }
                    break;
                case "A1_Q1_Tupas'Request_P2":
                    tupas.SetDialogueState("A1_Q1_Tupas'Request_P2");
                    amihanGuard.SetDialogueState("A1_Q1_Tupas'Request_P2");
                    ApplyStates(tupas, amihanGuard);
                    AddActiveMarker(currentQuestID, tracked);
                    duwendeAreaMap.RevealNow();
                    lewenriSaveInteractable.onInteract.RemoveListener(TutorialManager.instance.SaveTutorial);

                    TutorialManager.instance.closeSaveButton.onClick.RemoveListener(TutorialManager.instance.ContinueQuestAfterSave);
                    TutorialManager.instance.noSaveButton.onClick.RemoveListener(TutorialManager.instance.ContinueQuestAfterSave);
                    break;
                case "A1_Q1_Tupas'Request_P3": // SAVED POINT - must always setdialogue state of other npcs
                    tupas.SetDialogueState("A1_Q1_Tupas'Request_P3");
                    amihanGuard.SetDialogueState("Default");
                    ApplyStates(tupas, amihanGuard, bakal, bangkaw);
                    AddActiveMarker(currentQuestID, tracked);
                    duwendeCamp.SetActive(true);
                    garlicParent.SetActive(true);
                    playerMinimapRenderer.AddMinimapItemToBeHighlighted(garlicHighlight);
                    mapPinPopUp.SetActive(true);

                    bakal.SetDialogueState("Default");
                    bangkaw.SetDialogueState("Default");

                    if (questMinimapItem != null)
                    { 
                        questMinimapItem.SetActive(false);
                    }
                    break;
                case "A1_Q2_NewsFromTupas":
                    tupas.SetDialogueState("A1_Q2_NewsFromTupas");
                    ApplyStates(tupas);
                    AddActiveMarker(currentQuestID, tracked);
                    garlicParent.SetActive(false);
                    playerMinimapRenderer.RemoveMinimapItemOfHighlight(garlicHighlight);
                    break;
                case "A1_Q3_BesikTheScout":
                    besikNPC.SetActive(true);
                    tupas.SetDialogueState("A1_Q3_BesikTheScout");
                    besik.SetDialogueState("A1_Q3_BesikTheScout");
                    ApplyStates(tupas, besik);
                    AddActiveMarker(currentQuestID, tracked);
                    nunoSaPunsoAreaMap.RevealNow();
                    break;
                case "A1_Q4_Albularyo'sHut":
                    tupas.SetDialogueState("Default");
                    besik.SetDialogueState("A1_Q4_AlbularyoHut");
                    ApplyStates(tupas, besik);

                    RemoveActiveMarker();
                    break;
                case "A1_Q5_TimeToRest":
                    besik.SetDialogueState("Default");
                    layag.SetDialogueState("A1_Q5_TimeToRest");
                    DialogueManager.Instance.StartDialogue(layagSequence, layag);
                    ApplyStates(besik, layag);
                    nunoMound.SetActive(true);

                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A1_Q7_LessonFromNuno":
                    nunoBossFightTrigger.SetActive(false);
                    nunoSaveTrigger.SetActive(false);
                    nunoSaPunsoCharacter.SetActive(true);
                    nunoSaPunso.SetDialogueState("A1_Q7_LessonFromNuno");
                    ApplyStates(nunoSaPunso);

                    BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q5_NunoMound");
                    BB_QuestManager.Instance.ClaimRewardsByID("A1_Q5_TimeToRest");

                    BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q6_Nuno");
                    BB_QuestManager.Instance.ClaimRewardsByID("A1_Q6_NunoAnger");
                    ApplyStates(nunoSaPunso);
                    nunoSaPunsoCharacter.GetComponent<BlazeAI>().StayIdle();

                    AddActiveMarker(currentQuestID, tracked, new Vector3(0, 1, 0));
                    break;
                case "A1_Q8_ReturnToTheVillage":
                    tupas.SetDialogueState("A1_Q8_ReturnToTheVillage");
                    nunoSaPunso.SetDialogueState("Default");
                    ApplyStates(tupas, nunoSaPunso);
                    nunoSaPunsoCharacter.GetComponent<BlazeAI>().IgnoreStayIdle();

                    AddActiveMarker(currentQuestID, tracked);
                    break;
                #endregion

                #region ACT 2 QUESTS
                case "A2_Q1_FindAlbularyo'sHut":
                    tupas.SetDialogueState("A2_Q1_FindAlbularyo'sHut");
                    ApplyStates(tupas);
                    mangkukulamFirstTriggers.SetActive(true);
                    AddActiveMarker(currentQuestID, tracked);
                    break;

                case "A2_Q2_MysteriousWoman":
                    mangkukulamNpc.SetActive(true);
                    tupas.SetDialogueState("default");
                    mangkukulam.SetDialogueState("A2_Q2_MysteriousWoman");
                    ApplyStates(tupas, mangkukulam);
                    AddActiveMarker(currentQuestID, tracked);
                    break;

                case "A2_Q3_TheFirstPillar_P1":
                    mangkukulam.SetDialogueState("A2_Q3_TheFirstPillar_P1");
                    ApplyStates(mangkukulam);
                    AddActiveMarker(currentQuestID, tracked);
                    firstFakeBaleteTree.gameObject.layer = LayerMask.NameToLayer("Fake Balete Tree Domain");

                    mangkukulamNpc.GetComponent<NPC>().interactName = "Talk to Albularyo";
                    break;
                case "A2_Q3_TheSecondPillar_P1":
                    AddActiveMarker(currentQuestID, tracked);
                    secondFakeBaleteTree.gameObject.layer = LayerMask.NameToLayer("Fake Balete Tree Domain");
                    break;
                case "A2_Q3_TheThirdPillar_P1":
                    AddActiveMarker(currentQuestID, tracked);
                    thirdFakeBaleteTree.gameObject.layer = LayerMask.NameToLayer("Fake Balete Tree Domain");
                    break;
                case "A2_Q4_ReturnToTheAlbularyo":
                    mangkukulamNpc.SetActive(false);
                    mangkukulamSecondTriggers.SetActive(true);
                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A2_Q5_WhatHappened":
                    albularyoNpc.SetActive(true);

                    // queue forced teleport
                    GameManager.instance.forcedTeleportTarget = a2_q5_playerTransform;
                    GameManager.instance.hasForcedTeleport = true;

                    PlayerCamera.Instance.AdjustCamera();
                    a2_q5_questNotify.SetActive(true);
                    tupas.GetComponent<BlazeAI>().StayIdle();
                    albularyoNpc.GetComponent<BlazeAI>().StayIdle();
                    break;
                case "A2_Q6_AreYouOkay":
                    a2_q5_questNotify.SetActive(false);
                    tupas.SetDialogueState("A2_Q6_AreYouOkay");
                    ApplyStates(tupas);
                    albularyoNpc.gameObject.layer = LayerMask.NameToLayer("Default");
                    AddActiveMarker(currentQuestID, tracked);

                    albularyoNpc.SetActive(true);
                    break;
                case "A2_Q7_TalkToTheWoman":
                    tupas.SetDialogueState("A2_Q7_TalkToTheWoman");
                    albularyo.SetDialogueState("A2_Q7_TalkToTheWoman");
                    ApplyStates(tupas, albularyo);
                    albularyoNpc.gameObject.layer = LayerMask.NameToLayer("NPC");
                    AddActiveMarker(currentQuestID, tracked);

                    albularyoNpc.SetActive(true);
                    break;
                case "A2_Q8_GoToMangkukulamWithAlbularyo":
                    tupas.SetDialogueState("Default");
                    albularyo.SetDialogueState("A2_Q8_GoToMangkukulamWithAlbularyo");
                    ApplyStates(tupas, albularyo);
                    AddActiveMarker(currentQuestID, tracked);
                    albularyoNpc.GetComponent<NPC>().interactName = "Talk to Albularyo";
                    albularyoNpc.SetActive(true);
                    albularyoCanvas.SetActive(true);

                    mangkukulamNpc.SetActive(false);
                    mangkukulamThirdTriggers.SetActive(true);
                    break;
                case "A2_Q10_HowDoIgetHome":
                    GameManager.instance.forcedTeleportTarget = a2_q10_playerTransform;
                    GameManager.instance.hasForcedTeleport = true;

                    albularyoNpc.transform.SetPositionAndRotation(a2_q10_albularyoTransform.position, a2_q10_albularyoTransform.rotation);
                    albularyo.SetDialogueState("A2_Q10_HowDoIgetHome");
                    albularyoNpc.GetComponent<BlazeAI>().StayIdle();    
                    ApplyStates(albularyo);
                    PlayerCamera.Instance.AdjustCamera();

                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A2_Q11_RepairFirstBalete_P1":
                    albularyo.SetDialogueState("A2_Q11_RepairFirstBalete_P1");
                    ApplyStates(albularyo);
                    repairFirstBalete.gameObject.layer = LayerMask.NameToLayer("Fake Balete Tree Domain");
                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A2_Q11_RepairSecondBalete_P1":
                    repairSecondBalete.gameObject.layer = LayerMask.NameToLayer("Fake Balete Tree Domain");
                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A2_Q11_RepairThirdBalete_P1":
                    repairThirdBalete.gameObject.layer = LayerMask.NameToLayer("Fake Balete Tree Domain");
                    AddActiveMarker(currentQuestID, tracked);
                    break;
                case "A2_Q12_TimeToGoHome":
                    AddActiveMarker(currentQuestID, tracked);
                    albularyo.SetDialogueState("A2_Q12_TimeToGoHome");
                    ApplyStates(albularyo);
                    albularyoNpc.transform.SetPositionAndRotation(a2_q12_albularyoTransform.position, a2_q12_albularyoTransform.rotation);
                    albularyoNpc.GetComponent<BlazeAI>().StayIdle();
                    break;
                #endregion
            }
        }

        Debug.Log("The Focus now is: " + currentQuestID);
        GeneralQuestProgressCheck();
    }


    #region QUEST FUNCTIONS
    private void ApplyStates(params DialogueStateHolder[] holders)
    {
        foreach (var holder in holders)
            holder?.ApplyQueuedStateSilently();
    }

    public void GeneralQuestProgressCheck()
    {
        if (SwordTrainingDummies.childCount == 0)
        {
            SwordTrainingDummies.gameObject.SetActive(false);
            if (!isDelayAccept)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P1"));
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P2"));
            }
        }

        if (normalSkillTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            normalSkillTrainingDummies.gameObject.SetActive(false);
            if (!isDelayAccept)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P2"));
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P3"));
            }
        }

        if (ultimateSkillTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            ultimateSkillTrainingDummies.gameObject.SetActive(false);
            if (!isDelayAccept)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P3"));
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P4"));
            }
        }

        if (dashAmount == 5)
        {
            // isDelayAccept = false;
            // if (!isDelayAccept)
            // {
            //     dashAmount = 0;
            //     StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P4"));
            //     StartCoroutine(DelayAcceptQuest("A0_Q4_TrainingWithBangkaw"));
            // }

            isDelayAccept = false;

            if (!isDelayAccept)
            {

                if (!BB_QuestManager.Instance.HasQuest("A0_Q4_TrainingWithBangkaw"))
                {
                    StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P4"));
                    StartCoroutine(DelayAcceptQuest("A0_Q4_TrainingWithBangkaw"));
                }
            }
        }

        if (TutorialManager.instance.tutorial_isFirstStatueInteract == true)
        {
            isDelayAccept = false;
            if (!isDelayAccept)
            {
                RemoveActiveMarker();
                TutorialManager.instance.tutorial_isFirstStatueInteract = false;
                BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q6_SacredStatue");
                StartCoroutine(DelayAcceptQuestReward("A0_Q6_SacredStatue"));
                StartCoroutine(DelayAcceptQuest("A0_Q7_KeepingTrack"));
            }
        }
        if (agimatSkillOneTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            if (!isDelayAccept)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q11_AgimatTraining_P2"));
                StartCoroutine(DelayAcceptQuest("A0_Q11_AgimatTraining_P3"));
            }
        }
        if (agimatSkillTwoTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            if (!isDelayAccept)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q11_AgimatTraining_P4"));
                StartCoroutine(DelayAcceptQuest("A0_Q12_PamanaTraining_P1"));
            }
        }
        if (BB_QuestManager.Instance.IsQuestDone("A0_Q13_BackpackTraining"))
        {
            isDelayAccept = false;

            if (!isDelayAccept)
            {
                // ✅ Only add Tupas' Request if it's not already in progress/completed/claimed
                if (!BB_QuestManager.Instance.HasQuest("A1_Q1_Tupas'Request_P1"))
                {
                    StartCoroutine(DelayAcceptQuestReward("A0_Q13_BackpackTraining"));
                    StartCoroutine(DelayAcceptQuest("A1_Q1_Tupas'Request_P1"));
                }

                inventoryPopUp.SetActive(false);
                m_uiGame.inventoryButton.onClick.RemoveListener(TutorialManager.instance.EnableInventoryTutorial);
            }
        }
        if (BB_QuestManager.Instance.IsQuestDone("A1_Q1_Tupas'Request_P3"))
        {
            isDelayAccept = false;

            if (!isDelayAccept)
            {

                if (!BB_QuestManager.Instance.HasQuest("A1_Q2_NewsFromTupas"))
                {
                    StartCoroutine(DelayAcceptQuestReward("A1_Q1_Tupas'Request_P3"));
                    StartCoroutine(DelayAcceptQuest("A1_Q2_NewsFromTupas"));
                }
            }
        }
    }

    IEnumerator DelayAcceptQuest(string questID)
    {
        isDelayAccept = true;
        yield return new WaitForSeconds(2f);
        isDelayAccept = false;
        BB_QuestManager.Instance.AcceptQuestByID(questID);
    }

    IEnumerator DelayAcceptQuestReward(string questID)
    {
        isDelayAccept = true;
        yield return new WaitForSeconds(2f);
        isDelayAccept = false;
        BB_QuestManager.Instance.ClaimRewardsByID(questID);
    }
    IEnumerator DelayAcceptQuestWithTimer(string questID, float delaySeconds)
    {
        Debug.Log($"⏱ Timer started for {questID} ({delaySeconds}s)");
        yield return new WaitForSeconds(delaySeconds);
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q4_AlbularyoHut");
        BB_QuestManager.Instance.ClaimRewardsByID("A1_Q4_Albularyo'sHut");
        yield return new WaitForSeconds(0.25f);
        
        if (!BB_QuestManager.Instance.HasQuest(questID))
        {
            BB_QuestManager.Instance.AcceptQuestByID(questID);
            Debug.Log($"✅ Quest {questID} auto-accepted after {delaySeconds} seconds.");
        }
    }

    public void DelayAcceptA1Q5()
    {
        StartCoroutine(DelayAcceptQuestWithTimer("A1_Q5_TimeToRest", 10f));
    }

    public void DashCounter()
    {
        if (dashAmount < 5)
        {
            dashAmount++;
            BB_QuestManager.Instance.UpdateMissionProgress("A0_Q3_DashCount", 1);
        }
    }

    public void EnableTrainingAreaMode()
    {
        normalTrainingAreaMode.SetActive(false);
        trainingAreaMode.SetActive(true);
    }
    public void DisableTrainingAreaMode()
    {
        normalTrainingAreaMode.SetActive(true);
        trainingAreaMode.SetActive(false);
    }
    public void FirstStatueInteraction()
    {
        TutorialManager.instance.AllowFirstStatueInteraction();
        fullscreenMapPopUp.SetActive(true);
    }

    private void TeleportPlayerTo(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Teleport failed: target transform is null!");
            return;
        }

        // Find the player object (or cache it in Awake for performance)
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Teleport failed: no player object with tag 'Player' found!");
            return;
        }

        // Move player to target position and rotation
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            // Disable controller to avoid unwanted physics issues
            controller.enabled = false;
            player.transform.SetPositionAndRotation(target.position, target.rotation);
            controller.enabled = true;
        }
        else
        {
            player.transform.SetPositionAndRotation(target.position, target.rotation);
        }

        Debug.Log($"✅ Player teleported to {target.name} at {target.position}");
    }

    public void PlayAlbularyoCastingPortalCutscene()
    {
        _ = CutsceneTransition();
    }
    public async Task CutsceneTransition()
    {
        m_player.SetPlayerLocked(true);
        m_player.ForceIdleOverride();
        PlayerCamera.Instance.HardLockCamera();
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await Task.Delay(1000);
        await GameManager.instance.SaveAll();
        await Task.Delay(1500);
        Loader.Load(19);
    }
    public void QuestPropertiesCheck()
    {
        if (BB_QuestManager.Instance.IsQuestDone("A2_Q8_GoToMangkukulamWithAlbularyo"))
        {
            albularyoNpc.SetActive(true);
            albularyoNpc.GetComponent<NPC>().interactName = "Talk to Albularyo";
            albularyoCanvas.SetActive(true);
        }

        if (BB_QuestManager.Instance.IsQuestDone("A2_Q12_TimeToGoHome"))
        {
            portalInteractable.SetActive(true);
            albularyoNpc.transform.SetPositionAndRotation(a2_q12_albularyoTransform.position, a2_q12_albularyoTransform.rotation);
            albularyoNpc.GetComponent<BlazeAI>().StayIdle();
            albularyo.SetDialogueState("Default");
            ApplyStates(albularyo);
            RemoveActiveMarker();
        }
    }
    #endregion

    #region NAVIGATION FUNCTIONS
    public void RemoveActiveMarker()
    {
        if (activeMarker != null)
        {
            activeMarker.FadeOutAndDestroyMarker();
            activeMarker = null;
        }
    }
    public void AddActiveMarker(string currentQuestID, BB_Quest tracked, Vector3? customOffset = null)
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePlaying())
        {
            Debug.Log("Cannot spawn marker while in dialogue!");
            return;
        }
        if (Time.time - lastMarkerTime < markerCooldown)
        {
            Debug.Log("Quest marker is on cooldown!");
            return;
        }

        lastMarkerTime = Time.time;
        Debug.Log("Spawn Quest Marker");

        if (activeMarker != null)
        {
            activeMarker.FadeOutAndDestroyMarker();
            activeMarker = null;
        }

        Transform questTarget = GetQuestTargetTransform(currentQuestID);
        if (questTarget != null)
        {
            activeMarker = Instantiate(markerPrefab, uiMarkerParent);
            activeMarker.target = questTarget;
            activeMarker.mainCamera = Camera.main;

            // ✅ If custom offset is provided, override it
            if (customOffset.HasValue)
                activeMarker.SetOffset(customOffset.Value);

            activeMarker.SetQuestType(
                tracked.questType == BB_QuestType.Main,
                mainQuestSprite,
                sideQuestSprite
            );
        }
    }
    private Transform GetQuestTargetTransform(string questID)
    {
        switch (questID)
        {
            case "A0_Q1_FindAndTalkToTupas": return tupas.transform;
            case "A0_Q2_FindAndTalkToBangkaw": return bangkaw.transform;
            case "A0_Q5_ReturnToTupas": return tupas.transform;
            case "A0_Q6_SacredStatue": return TutorialManager.instance.lewenriSacredStatue.transform;
            case "A0_Q7_KeepingTrack": return tupas.transform;
            case "A0_Q9_OneMoreThing": return tupas.transform;
            case "A0_Q10_ReturnToBangkaw": return bangkaw.transform;
            case "A0_Q12_PamanaTraining_P2": return tupas.transform;
            case "A1_Q1_Tupas'Request_P1": return tupas.transform;
            case "A1_Q1.1_Amihan'sOrder_P1": return amihanGuard.transform;
            case "A1_Q1.1_Amihan'sOrder_P2": return lewenriSaveInteractable.transform;
            case "A1_Q1_Tupas'Request_P2": return amihanGuard.transform;
            case "A1_Q1_Tupas'Request_P3": return garlicParent.transform;
            case "A1_Q2_NewsFromTupas": return tupas.transform;
            case "A1_Q3_BesikTheScout": return besik.transform;
            case "A1_Q5_TimeToRest": return nunoMound.transform;
            case "A1_Q7_LessonFromNuno": return nunoSaPunsoCharacter.transform;
            case "A1_Q8_ReturnToTheVillage": return tupas.transform;
            case "A2_Q1_FindAlbularyo'sHut": return mangkukulamHut.transform;
            case "A2_Q2_MysteriousWoman": return mangkukulam.transform;
            case "A2_Q3_TheFirstPillar_P1": return firstFakeBaleteTree.transform;
            case "A2_Q3_TheSecondPillar_P1": return secondFakeBaleteTree.transform;
            case "A2_Q3_TheThirdPillar_P1": return thirdFakeBaleteTree.transform;
            case "A2_Q4_ReturnToTheAlbularyo": return mangkukulamHut.transform;
            case "A2_Q6_AreYouOkay": return tupas.transform;
            case "A2_Q7_TalkToTheWoman": return albularyo.transform;
            case "A2_Q8_GoToMangkukulamWithAlbularyo": return mangkukulamHut.transform;
            case "A2_Q10_HowDoIgetHome": return albularyoNpc.transform;
            case "A2_Q11_RepairFirstBalete_P1": return repairFirstBalete.transform;
            case "A2_Q11_RepairSecondBalete_P1": return repairSecondBalete.transform;
            case "A2_Q11_RepairThirdBalete_P1": return repairThirdBalete.transform;
            case "A2_Q12_TimeToGoHome": return albularyoNpc.transform;
        }
        return null;
    }
    #endregion

}
