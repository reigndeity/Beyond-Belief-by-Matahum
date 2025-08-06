using UnityEngine;

public class DialogueQuestLinker : MonoBehaviour
{
    public DialogueStateHolder cubeboloy;
    private string lastTrackedQuestID = "";

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
                case "Q0_Question":
                    cubeboloy.SetDialogueState("Q0_Question");
                    break;

                // Add more as needed
            }
        }
    }
}
