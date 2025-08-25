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

    [Header("All NPCs Dialogue State Holders")]
    public DialogueStateHolder tupas;
    public DialogueStateHolder bakal;
    public DialogueStateHolder bangkaw;
    [Header("All NPCs Quest Tracker Icon")]
    public UI_CanvasGroup tupasTracker;
    public UI_CanvasGroup bakalTracker;
    public UI_CanvasGroup bangkawTracker;

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
    [SerializeField] private MinimapItem lewenriStatueMinimapItem;
    [SerializeField] GameObject fullscreenMapPopUp;
    

    void OnEnable()
    {
        PlayerMovement.OnDashStarted += DashCounter;
    }

    void OnDisable()
    {
        PlayerMovement.OnDashStarted -= DashCounter;
    }

    void Update()
    {
        var tracked = BB_QuestHUD.instance?.trackedQuest;
        if (tracked == null) return;

        string currentQuestID = tracked.questID;

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
                    tupasTracker.FadeIn(0.25f);
                    ApplyStates(tupas, bakal, bangkaw);
                    break;
                case "A0_Q2_FindAndTalkToBangkaw":
                    tupas.SetDialogueState("A0_Q2_FindAndTalkToBangkaw");
                    bakal.SetDialogueState("Default");
                    bangkaw.SetDialogueState("A0_Q2_FindAndTalkToBangkaw");
                    bangkawTracker.FadeIn(0.25f);
                    ApplyStates(tupas, bakal, bangkaw);
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
                    tupasTracker.FadeIn(0.25f);
                    ApplyStates(bangkaw,tupas);

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

                    TutorialManager.instance.lewenriSacredStatue.gameObject.layer = LayerMask.NameToLayer("Teleporter");
                    playerMinimapRenderer.AddMinimapItemToBeHighlighted((lewenriStatueMinimapItem));
                    lewenriStatueMinimapItem.particlesHighlightMode = MinimapItem.ParticlesHighlightMode.WavesIncrease;
                    m_uiGame.closeMapButton.onClick.AddListener(FirstStatueInteraction);
                    break;
                case "A0_Q7_KeepingTrack":
                    tupas.SetDialogueState("A0_Q7_KeepingTrack");
                    tupasTracker.FadeIn(0.25f);
                    ApplyStates(tupas);

                    m_uiGame.closeMapButton.onClick.RemoveListener(FirstStatueInteraction);
                    break;
                // Add more as needed
            }
        }

        Debug.Log("The Focus now is: " + currentQuestID);
        
        GeneralQuestProgressCheck();
    }

    private void ApplyStates(params DialogueStateHolder[] holders)
    {
        foreach (var holder in holders)
        {
            holder?.ApplyQueuedStateSilently();
        }
    }

    public void GeneralQuestProgressCheck()
    {
        if (SwordTrainingDummies.childCount == 0)
        {
            SwordTrainingDummies.gameObject.SetActive(false);
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P1"));
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P2"));
            }
            
        }
        if (normalSkillTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            normalSkillTrainingDummies.gameObject.SetActive(false);
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P2"));
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P3"));
                Debug.Log("WORK");
            }
        }
        if (ultimateSkillTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            ultimateSkillTrainingDummies.gameObject.SetActive(false);
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P3"));
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P4"));
            }
        }
        if (dashAmount == 5)
        {
            isDelayAccept = false;
            if (isDelayAccept == false)
            {
                dashAmount = 0;
                StartCoroutine(DelayAcceptQuestReward("A0_Q3_Bangkaw'sTraining_P4"));
                StartCoroutine(DelayAcceptQuest("A0_Q4_TrainingWithBangkaw"));
            }
        }
        if (TutorialManager.instance.tutorial_isFirstStatueInteract == true) // This is set up in close fullscreen map button once
        {
            isDelayAccept = false;
            if (isDelayAccept == false)
            {
                TutorialManager.instance.tutorial_isFirstStatueInteract = false;
                BB_QuestManager.Instance.UpdateMissionProgressOnce("A0_Q6_SacredStatue");
                StartCoroutine(DelayAcceptQuestReward("A0_Q6_SacredStatue"));
                StartCoroutine(DelayAcceptQuest("A0_Q7_KeepingTrack"));
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
        playerMinimapRenderer.RemoveMinimapItemOfHighlight((lewenriStatueMinimapItem));
        lewenriStatueMinimapItem.particlesHighlightMode = MinimapItem.ParticlesHighlightMode.Disabled;
        fullscreenMapPopUp.SetActive(true);
    }
}
