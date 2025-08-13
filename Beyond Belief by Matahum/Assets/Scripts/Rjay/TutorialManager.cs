using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool isTutorialDone;

    [Header("Tutorial Components")]
    public UI_CanvasGroup characterDetailsButton;
    public UI_CanvasGroup inventoryButton;
    public UI_CanvasGroup archiveButton;
    public UI_CanvasGroup questButton;
    public UI_CanvasGroup minimap;

    void Start()
    {
        if (!isTutorialDone)
        {
            StartTutorial();
        } 
    }


    void StartTutorial()
    {
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q0_InitialTalk");
    }
}
