using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueQuestLinker : MonoBehaviour
{
    
    private string lastTrackedQuestID = "";
    public bool isDelayAccept;

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
    public TimelineAsset A0_Q3_BangkawTraining_P2_Cutscene;
    [SerializeField] private Transform normalSkillTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraining_P3_Cutscene;
    [SerializeField] private Transform ultimateSkillTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraining_P4_Cutscene;
    private int dashAmount;
    public TimelineAsset A0_Q4_ReturnToTupas_Cutscene;

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
                case "A0_Q4_ReturnToTupas":
                    bangkaw.SetDialogueState("A0_Q4_ReturnToTupas");
                    tupas.SetDialogueState("A0_Q4_ReturnToTupas");
                    ApplyStates(bangkaw);

                    TutorialManager.instance.tutorial_canAttack = true;
                    TutorialManager.instance.tutorial_canNormalSkill = true;
                    TutorialManager.instance.tutorial_canUltimateSkill = true;
                    TutorialManager.instance.ShowNormalSkill();
                    TutorialManager.instance.ShowUltimateSkill();
                    CutsceneManager.Instance.StartCutscene(A0_Q4_ReturnToTupas_Cutscene);
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
            BB_QuestManager.Instance.ClaimRewardsByID("A0_Q3_Bangkaw'sTraining_P1");
            SwordTrainingDummies.gameObject.SetActive(false);
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P2"));
            }
            
        }
        if (normalSkillTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            BB_QuestManager.Instance.ClaimRewardsByID("A0_Q3_Bangkaw'sTraining_P2");
            normalSkillTrainingDummies.gameObject.SetActive(false);
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P3"));
                Debug.Log("WORK");
            }
        }
        if (ultimateSkillTrainingDummies.childCount == 0)
        {
            isDelayAccept = false;
            BB_QuestManager.Instance.ClaimRewardsByID("A0_Q3_Bangkaw'sTraining_P3");
            ultimateSkillTrainingDummies.gameObject.SetActive(false);
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuest("A0_Q3_Bangkaw'sTraining_P4"));
            }
        }
        if (dashAmount == 5)
        {
            isDelayAccept = false;
            BB_QuestManager.Instance.ClaimRewardsByID("A0_Q3_Bangkaw'sTraining_P4");
            if (isDelayAccept == false)
            {
                StartCoroutine(DelayAcceptQuest("A0_Q4_ReturnToTupas"));
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
    public void DashCounter()
    {
        if (dashAmount < 5)
        {
            dashAmount++;
            BB_QuestManager.Instance.UpdateMissionProgress("A0_Q3_DashCount", 1);
        }
    }
}
