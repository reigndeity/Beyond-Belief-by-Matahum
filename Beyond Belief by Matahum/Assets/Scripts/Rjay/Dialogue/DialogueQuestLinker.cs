using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueQuestLinker : MonoBehaviour
{
    
    private string lastTrackedQuestID = "";

    [Header("All NPCs Dialogue State Holders")]
    public DialogueStateHolder tupas;
    public DialogueStateHolder bakal;
    public DialogueStateHolder bangkaw;

    [Header("Act 0 Components")]
    [SerializeField] GameObject A0_Q0_InitialTalk_NQP;
    [SerializeField] private Transform SwordTrainingDummies;
    public TimelineAsset A0_Q3_BangkawTraning_P2_Cutscene;

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

                    ApplyStates(tupas, bakal, bangkaw);
                    break;
                case "A0_Q2_FindAndTalkToBangkaw":
                    tupas.SetDialogueState("A0_Q2_FindAndTalkToBangkaw");
                    bakal.SetDialogueState("Default");
                    bangkaw.SetDialogueState("A0_Q2_FindAndTalkToBangkaw");

                    ApplyStates(tupas, bakal, bangkaw);
                    break;
                case "A0_Q3_Bangkaw'sTraning_P1":
                    tupas.SetDialogueState("Default");
                    bakal.SetDialogueState("Default");
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraning_P1");

                    ApplyStates(tupas, bakal, bangkaw);
                    break;
                case "A0_Q3_Bangkaw'sTraning_P2":
                    bangkaw.SetDialogueState("A0_Q3_Bangkaw'sTraning_P2");
                    ApplyStates(bangkaw);
                    
                    TutorialManager.instance.tutorial_canAttack = false;
                    CutsceneManager.Instance.StartCutscene(A0_Q3_BangkawTraning_P2_Cutscene);
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
            BB_QuestManager.Instance.ClaimRewardsByID("A0_Q3_Bangkaw'sTraning_P1");
            BB_QuestManager.Instance.AcceptQuestByID("A0_Q3_Bangkaw'sTraning_P2");
            SwordTrainingDummies.gameObject.SetActive(false);
        }
    }
}
