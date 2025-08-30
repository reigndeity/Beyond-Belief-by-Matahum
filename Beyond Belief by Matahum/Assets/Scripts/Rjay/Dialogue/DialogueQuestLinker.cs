using System.Collections;
using MTAssets.EasyMinimapSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueQuestLinker : MonoBehaviour
{
    private string lastTrackedQuestID = "";
    public bool isDelayAccept;
    [SerializeField] private UI_Game m_uiGame;
    private PlayerInput m_playerInput;
    [SerializeField] R_Inventory inventory;
    [SerializeField] R_InventoryUI inventoryUI;

    [Header("All NPCs Dialogue State Holders")]
    public DialogueStateHolder tupas;
    public DialogueStateHolder bakal;
    public DialogueStateHolder bangkaw;

    [Header("All NPCs Quest Tracker Icon")]
    public UI_CanvasGroup tupasTracker;
    public UI_CanvasGroup bakalTracker;
    public UI_CanvasGroup bangkawTracker;

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
    [SerializeField] GameObject A0_Q0_InitialTalk_NQP;
    [SerializeField] private Transform SwordTrainingDummies;
    [SerializeField] Collider playerWeaponCollider;
    public TimelineAsset A0_Q3_BangkawTraining_P2_Cutscene;
    [SerializeField] private Transform normalSkillTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraining_P3_Cutscene;
    [SerializeField] private Transform ultimateSkillTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraining_P4_Cutscene;
    private int dashAmount;
    public TimelineAsset A0_Q4_TrainingWithBangkaw_Cutscene;
    [SerializeField] private MinimapRenderer playerMinimapRenderer;
    [SerializeField] GameObject fullscreenMapPopUp;
    public R_ItemData ngipinNgKidlatAgimat;
    public TimelineAsset A0_Q11_AgimatTraining_P1_Cutscene;
    public TimelineAsset A0_Q11_AgimatTraining_P2_Cutscene;
    [SerializeField] private Transform agimatSkillOneTrainingDummies;
    public TimelineAsset A0_Q11_AgimatTraining_P3_Cutscene;
    public TimelineAsset A0_Q11_AgimatTraining_P4_Cutscene;

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
                case "A0_Q0_InitialTalk":
                    bakal.SetDialogueState("A0_Q0_InitialTalk");
                    ApplyStates(bakal);
                    A0_Q0_InitialTalk_NQP.SetActive(true);
                    TutorialManager.instance.cutsceneTriggerOne.SetActive(true);
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
                    break;

                case "A0_Q3_Bangkaw'sTraining_P3":
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraining_P3");
                    ApplyStates(bangkaw);

                    TutorialManager.instance.tutorial_canAttack = false;
                    TutorialManager.instance.tutorial_canNormalSkill = false;
                    TutorialManager.instance.HideNormalSkill();
                    CutsceneManager.Instance.StartCutscene(A0_Q3_BangkawTraining_P3_Cutscene);
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
                    inventory.AddItem(ngipinNgKidlatAgimat, 1);
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
            isDelayAccept = false;
            if (!isDelayAccept)
            {
                dashAmount = 0;
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P4"));
                StartCoroutine(DelayAcceptQuest("A0_Q4_TrainingWithBangkaw"));
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

    public void DashCounter()
    {
        if (dashAmount < 5)
        {
            dashAmount++;
            BB_QuestManager.Instance.UpdateMissionProgress("A0_Q3_DashCount", 1);
        }
    }

    public void FirstStatueInteraction()
    {
        TutorialManager.instance.AllowFirstStatueInteraction();
        fullscreenMapPopUp.SetActive(true);
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
            case "A0_Q10_ReturnToBangkaw" : return bangkaw.transform;
        }
        return null;
    }
    #endregion

}
