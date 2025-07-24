using UnityEngine;
using TMPro;

public class BB_QuestHUD : MonoBehaviour
{
    public static BB_QuestHUD instance;

    [Header("Main Quest HUD")]
    public TextMeshProUGUI mainQuestTitle;
    public TextMeshProUGUI mainQuestDescription;
    public Transform mainQuestMissionList;

    [Header("Side Quest HUD")]
    public TextMeshProUGUI sideQuestTitle;
    public TextMeshProUGUI sideQuestDescription;
    public Transform sideQuestMissionList;

    [Header("Quest Mission HUD Template")]
    public BB_QuestHUDUIGroupTemplate questHUDTemplate;

    public BB_Quest trackedMainQuest;
    public BB_Quest trackedSideQuest;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        BB_QuestManager.Instance.OnQuestUpdate += UpdateUI;
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateMainQuestHud();
        UpdateSideQuestHud();
    }

    public void UpdateMainQuestHud()
    {
        foreach (Transform child in mainQuestMissionList)
        {
            Destroy(child.gameObject);
        }

        if (trackedMainQuest != null)
        {
            if (trackedMainQuest.state == QuestState.Active)
            {
                mainQuestTitle.text = trackedMainQuest.questTitle;
                mainQuestDescription.text = trackedMainQuest.questHudDescription;

                foreach (BB_Mission mission in trackedMainQuest.missions)
                {
                    BB_QuestHUDUIGroupTemplate missionHUD = Instantiate(questHUDTemplate, mainQuestMissionList);
                    missionHUD.missionGoal.text = $"{mission.whatMustBeDone}:{mission.currentAmount}/{mission.requiredAmount}";
                }
            }
            else if (trackedMainQuest.state == QuestState.Completed)
            {
                mainQuestTitle.text = trackedMainQuest.questTitle;
                mainQuestDescription.text = trackedMainQuest.questHudWhereToClaimQueDescription;
            }
            else if (trackedMainQuest.state == QuestState.Claimed)
            {
                mainQuestTitle.text = trackedMainQuest.questTitle;
                mainQuestDescription.text = "Quest Completed";
                mainQuestDescription.color = Color.green;
            }
        }
        else
        {
            mainQuestTitle.text = "";
            mainQuestDescription.text = "";
        }
    }
    public void UpdateSideQuestHud()
    {
        foreach (Transform child in sideQuestMissionList)
        {
            Destroy(child.gameObject);
        }

        if (trackedSideQuest != null)
        {
            if (trackedSideQuest.state == QuestState.Active)
            {
                sideQuestTitle.text = trackedSideQuest.questTitle;
                sideQuestDescription.text = trackedSideQuest.questHudDescription;

                foreach (BB_Mission mission in trackedSideQuest.missions)
                {
                    BB_QuestHUDUIGroupTemplate missionHUD = Instantiate(questHUDTemplate, sideQuestMissionList);
                    missionHUD.missionGoal.text = $"{mission.whatMustBeDone}:{mission.currentAmount}/{mission.requiredAmount}";

                }
            }
            else if (trackedSideQuest.state == QuestState.Completed)
            {
                sideQuestTitle.text = trackedSideQuest.questTitle;
                sideQuestDescription.text = trackedSideQuest.questHudWhereToClaimQueDescription;
            }
            else if (trackedSideQuest.state == QuestState.Claimed)
            {
                sideQuestTitle.text = trackedSideQuest.questTitle;
                sideQuestDescription.text = "Quest Completed";
                sideQuestDescription.color = Color.green;
            }
        }
        else
        {
            sideQuestTitle.text = "";
            sideQuestDescription.text = "";
        }
    }
}
