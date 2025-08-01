using UnityEngine;
using UnityEngine.UI;

public class BB_Quest_ButtonManager : MonoBehaviour
{
    [Header("Quest GameObject References")]
    public GameObject questJournal;
    public GameObject questHud;

    [Header("Open Journal")]
    public Button openJournalButton;

    [Header("Main Quest Button and Panel")]
    public GameObject mainQuestPanel;
    public Transform mainQuestScrollView;
    public Button mainQuestButton;

    [Header("Side Quest Button and Panel")]
    public GameObject sideQuestPanel;
    public Transform sideQuestScrollView;
    public Button sideQuestButton;

    [Header("Exit Quest Journal")]
    public Button questTrackButton;
    public Button questUntrackButton;

    [Header("Exit Quest Journal")]
    public Button exitButton;

    private void Start()
    {
        openJournalButton.onClick.AddListener(OpenJournal);
        mainQuestButton.onClick.AddListener(OpenMainQuest);
        sideQuestButton.onClick.AddListener(OpenSideQuest);
        questTrackButton.onClick.AddListener(TrackQuest);
        questUntrackButton.onClick.AddListener(UnTrackQuest);

    }
    #region Open Journal
    public void OpenJournal()
    {
        questJournal.SetActive(true);
        questHud.SetActive(false);

        mainQuestPanel.SetActive(true);
        sideQuestPanel.SetActive(false);
        BB_QuestJournalUI.instance.OnOpenJournal(mainQuestScrollView);
    }
    #endregion
    #region Quest Category
    public void OpenMainQuest()
    {
        mainQuestPanel.SetActive(true);
        sideQuestPanel.SetActive(false);
        BB_QuestJournalUI.instance.OnOpenJournal(mainQuestScrollView);
    }
    public void OpenSideQuest()
    {
        mainQuestPanel.SetActive(false);
        sideQuestPanel.SetActive(true);
        BB_QuestJournalUI.instance.OnOpenJournal(sideQuestScrollView);
    }
    #endregion
    #region Tracker
    public void TrackQuest()
    {
        if (BB_QuestJournalUI.instance.currentSelectedQuest.questType == BB_QuestType.Main)
        {
            BB_QuestHUD.instance.trackedMainQuest = BB_QuestJournalUI.instance.currentSelectedQuest;

            foreach (BB_Quest quest in BB_QuestManager.Instance.activeMainQuests)
                quest.isBeingTracked = false;
        }
        else
        {
            BB_QuestHUD.instance.trackedSideQuest = BB_QuestJournalUI.instance.currentSelectedQuest;

            foreach (BB_Quest quest in BB_QuestManager.Instance.activeSideQuests)
                quest.isBeingTracked = false;
        }

        BB_QuestJournalUI.instance.currentSelectedQuest.isBeingTracked = true;
        BB_QuestJournalUI.instance.ChangeTrackerButtonDisplay(BB_QuestJournalUI.instance.currentSelectedQuest);
        BB_QuestJournalUI.instance.ChangeSiblingArrangement();
        BB_QuestHUD.instance.UpdateUI();
    }

    public void UnTrackQuest()
    {
        if (BB_QuestJournalUI.instance.currentSelectedQuest.questType == BB_QuestType.Main)
            BB_QuestHUD.instance.trackedMainQuest = null;
        else
            BB_QuestHUD.instance.trackedSideQuest = null;

        BB_QuestJournalUI.instance.currentSelectedQuest.isBeingTracked = false;
        BB_QuestJournalUI.instance.ChangeTrackerButtonDisplay(BB_QuestJournalUI.instance.currentSelectedQuest);
        BB_QuestHUD.instance.UpdateUI();
    }
    #endregion
    #region Exit Journal
    public void ExitJournal()
    {
        questJournal.SetActive(false);
        questHud.SetActive(true);
    }
    #endregion
}