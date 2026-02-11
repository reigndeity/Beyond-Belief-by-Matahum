using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BB_Quest_ButtonManager : MonoBehaviour
{
    [Header("Quest GameObject References")]
    public GameObject questJournal;
    public GameObject questHud;

    [Header("Main Quest Button and Panel")]
    public TextMeshProUGUI mainQuestTextDivider;
    public GameObject mainQuestPanel;
    public Transform mainQuestScrollView;
    public Button mainQuestButton;

    [Header("Side Quest Button and Panel")]
    public TextMeshProUGUI sideQuestTextDivider;
    public GameObject sideQuestPanel;
    public Transform sideQuestScrollView;
    public Button sideQuestButton;

    [Header("All Quest Button and Panel")]
    public Button allQuestButton;
    public TextMeshProUGUI questTitleText;

    [Header("Completed Quest BUtton and Panel")]
    public GameObject completedQuestPanel;
    public Transform completedQuestScrollView;
    public Button completedQuestButton;

    [Header("Exit Quest Journal")]
    public Button questTrackButton;
    public Button questUntrackButton;

    [Header("Claim Rewards")]
    public Button claimRewardsButton;


    private void Start()
    {
        mainQuestButton.onClick.AddListener(OpenMainQuest);
        sideQuestButton.onClick.AddListener(OpenSideQuest);
        allQuestButton.onClick.AddListener(OpenAllQuest);
        completedQuestButton.onClick.AddListener(OpenCompletedQuest);
        questTrackButton.onClick.AddListener(TrackQuest);
        questUntrackButton.onClick.AddListener(UnTrackQuest);
        claimRewardsButton.onClick.AddListener(ClaimRewards);
    }
    #region Open Journal
    public void OpenJournal()
    {
        questJournal.SetActive(true);
        questHud.SetActive(false);

        //mainQuestPanel.SetActive(true);
        //sideQuestPanel.SetActive(false);
        //BB_QuestJournalUI.instance.OnOpenJournal(mainQuestScrollView);

        //OpenAllQuest();
    }
    #endregion
    #region Quest Category
    public void OpenMainQuest()
    {
        mainQuestPanel.SetActive(true);
        sideQuestPanel.SetActive(false);
        completedQuestPanel.SetActive(false);

        mainQuestTextDivider.gameObject.SetActive(false);
        sideQuestTextDivider.gameObject.SetActive(false);

        BB_QuestJournalUI.instance.OnOpenJournal(mainQuestScrollView);
        /*BB_QuestJournalUI.instance.FilterQuests(QuestScrollView.main);
        BB_QuestJournalUI.instance.OnOpenJournal(allQuestScrollView);*/
        questTitleText.text = "Main Quests";
    }
    public void OpenSideQuest()
    {
        mainQuestPanel.SetActive(false);
        sideQuestPanel.SetActive(true);
        completedQuestPanel.SetActive(false);

        mainQuestTextDivider.gameObject.SetActive(false);
        sideQuestTextDivider.gameObject.SetActive(false);

        BB_QuestJournalUI.instance.OnOpenJournal(sideQuestScrollView);
        /*BB_QuestJournalUI.instance.FilterQuests(QuestScrollView.side);
        BB_QuestJournalUI.instance.OnOpenJournal(allQuestScrollView);*/
        questTitleText.text = "Side Quests";
    }
    public void OpenAllQuest()
    {
        mainQuestPanel.SetActive(true);
        sideQuestPanel.SetActive(true);
        completedQuestPanel.SetActive(false);

        mainQuestTextDivider.gameObject.SetActive(true);
        sideQuestTextDivider.gameObject.SetActive(true);

        //BB_QuestJournalUI.instance.FilterQuests(QuestScrollView.all);
        BB_QuestJournalUI.instance.OnOpenJournal(mainQuestScrollView, sideQuestScrollView);
        questTitleText.text = "All Quests";
    }

    public void OpenCompletedQuest()
    {
        mainQuestPanel.SetActive(false);
        sideQuestPanel.SetActive(false);
        completedQuestPanel.SetActive(true);

        mainQuestTextDivider.gameObject.SetActive(false);
        sideQuestTextDivider.gameObject.SetActive(false);

        BB_QuestJournalUI.instance.OnOpenJournal(completedQuestScrollView);
        questTitleText.text = "Completed Quests";
    }
    #endregion
    #region Tracker
    public void TrackQuest()
    {
        BB_QuestJournalUI.instance.TrackQuest(BB_QuestJournalUI.instance.currentSelectedQuest);

    }

    public void UnTrackQuest()
    {
        BB_QuestJournalUI.instance.UnTrackQuest(BB_QuestJournalUI.instance.currentSelectedQuest);

    }
    #endregion
    #region Claim Rewards
    public void ClaimRewards()
    {
        BB_QuestManager.Instance.ClaimRewards(BB_QuestJournalUI.instance.currentSelectedQuest);
        claimRewardsButton.gameObject.SetActive(false);

    }
    #endregion
    #region Exit Journal
    public void ExitJournal()
    {
        questJournal.SetActive(false);
        questHud.SetActive(true);
    }
    #endregion

    public bool IsJournalOpen()
    {
        return questJournal.activeSelf;
    }
}