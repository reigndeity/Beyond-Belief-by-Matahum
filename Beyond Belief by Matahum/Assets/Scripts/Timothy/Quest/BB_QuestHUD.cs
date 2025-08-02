using UnityEngine;
using TMPro;

public class BB_QuestHUD : MonoBehaviour
{
    public static BB_QuestHUD instance;

    /*[Header("Main Quest HUD")]
    public TextMeshProUGUI mainQuestTitle;
    public TextMeshProUGUI mainQuestDescription;
    public Transform mainQuestMissionList;

    [Header("Side Quest HUD")]
    public TextMeshProUGUI sideQuestTitle;
    public TextMeshProUGUI sideQuestDescription;
    public Transform sideQuestMissionList;*/

    [Header("Quest HUD")]
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questDescription;
    public Transform questMissionList;

    [Header("Quest Mission HUD Template")]
    public BB_QuestMissionTemplate questHUDTemplate;

    /*public BB_Quest trackedMainQuest;
    public BB_Quest trackedSideQuest;*/

    public BB_Quest trackedQuest;

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
        UpdateQuestHud();
    }

    public void UpdateQuestHud()
    {
        foreach (Transform child in questMissionList)
        {
            Destroy(child.gameObject);
        }

        if (trackedQuest != null)
        {
            if (trackedQuest.state == QuestState.Active)
            {
                questTitle.text = trackedQuest.questTitle;
                questDescription.text = trackedQuest.questHudDescription;
                questDescription.color = Color.black;

                foreach (BB_Mission mission in trackedQuest.missions)
                {
                    BB_QuestMissionTemplate missionHUD = Instantiate(questHUDTemplate, questMissionList);
                    missionHUD.missionDescription.text = $"{mission.whatMustBeDone}";
                    if (mission.hasCounter)
                    {
                        missionHUD.missionCounter.gameObject.SetActive(true);
                        missionHUD.missionCounter.text = $"{mission.currentAmount}/{mission.requiredAmount}";
                    }
                }
            }
            else if (trackedQuest.state == QuestState.Completed)
            {
                questTitle.text = trackedQuest.questTitle;
                questDescription.text = "Quest Completed";
                questDescription.color = Color.green;
            }
        }
        else
        {
            questTitle.text = "";
            questDescription.text = "";
        }
    }

    /*public void UpdateMainQuestHud()
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
                    BB_QuestMissionTemplate missionHUD = Instantiate(questHUDTemplate, mainQuestMissionList);
                    missionHUD.missionDescription.text = $"{mission.whatMustBeDone}";
                    if (mission.hasCounter)
                    {
                        missionHUD.missionCounter.gameObject.SetActive(true);
                        missionHUD.missionCounter.text = $"{mission.currentAmount}/{mission.requiredAmount}";
                    }
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
                    BB_QuestMissionTemplate missionHUD = Instantiate(questHUDTemplate, sideQuestMissionList);
                    missionHUD.missionDescription.text = $"{mission.whatMustBeDone}";
                    if (mission.hasCounter)
                    {
                        missionHUD.missionCounter.gameObject.SetActive(true);
                        missionHUD.missionCounter.text = $"{mission.currentAmount}/{mission.requiredAmount}";
                    }
                    else missionHUD.missionCounter.gameObject.SetActive(false);
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
    }*/
}
