using UnityEngine;

public class DialogueQuestLinker : MonoBehaviour
{
    
    private string lastTrackedQuestID = "";

    [Header("All NPCs Dialogue State Holders")]
    public DialogueStateHolder tupas;

    void Update()
    {
        var tracked = BB_QuestHUD.instance?.trackedQuest;
        if (tracked == null) return;

        string currentQuestID = tracked.questID;

        if (currentQuestID != lastTrackedQuestID)
        {
            lastTrackedQuestID = currentQuestID;

            switch (currentQuestID)
            {
                case "A0_Q0_InitialTalk":
                    tupas.SetDialogueState("A0_Q0_InitialTalk");
                    break;

                // Add more as needed
            }
        }
    }
}
