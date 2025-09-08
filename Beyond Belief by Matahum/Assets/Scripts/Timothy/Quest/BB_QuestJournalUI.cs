using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public enum QuestScrollView
{
    all,
    main,
    side
}

public class BB_QuestJournalUI : MonoBehaviour
{
    public static BB_QuestJournalUI instance;

    public QuestScrollView questContentScrollView;

    [Header("Quest Button Manager")]
    public BB_Quest_ButtonManager buttonManager;

    [Header("Main Quest Category")]
    public Transform mainQuestSelectionScrollContent;  

    [Header("Side Quest Category")]
    public Transform SideQuestSelectionScrollContent;

    [Header("Completed Quest Category")]
    public Transform completedQuestSelectionScrollContent;

    [Header("Quest Group Template")]
    public GameObject questGroupTemplate;
    public Button questTitleTemplate;

    [Header("Quest Details")]
    public TextMeshProUGUI questDetailsQuestTitle;
    public GameObject questDetailsScrollViewContent;
    public TextMeshProUGUI questDetailsBodyText;
    public TextMeshProUGUI questRewardText;
    public Transform questRewardList;
    public GameObject questRewardGroupTemplate;
    public BB_QuestMissionTemplate questMissionTemplate;

    [Header("Quest Selected Indicator")] 
    public Sprite defaultSprite;
    public Sprite selectedSprite; // Assign this manually or load it
    private Button currentlySelectedButton;
    private Image currentlySelectedImage;
    
    [Header("Quest Tracker")]
    public BB_Quest currentSelectedQuest;

    // Holds instantiated Act groups for reuse
    private Dictionary<string, Transform> actGroups = new Dictionary<string, Transform>();
    private HashSet<string> initializedActButtons = new HashSet<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        //BB_QuestManager.Instance.OnQuestUpdate += () => ShowQuestDetails(currentSelectedQuest);
        BB_QuestManager.Instance.OnQuestUpdate += HandleQuestUpdate;
    }

    public void RefreshJournalUI()
    {
        foreach (BB_Quest quest in BB_QuestManager.Instance.allQuests)
        {
            // Skip inactive quests (they shouldn't be in the journal UI)
            if (quest.state == QuestState.Inactive)
                continue;

            // If claimed but not already in completed panel → move it
            if (quest.state == QuestState.Claimed && !IsInCompletedPanel(quest))
            {
                MoveQuestToCompletedPanel(quest);
                continue; // it's already handled
            }

            // If quest is still active/completed but in main/side lists → update its position
            if (quest.state == QuestState.Active || quest.state == QuestState.Completed)
            {
                ChangeSiblingArrangement(quest);
            }

            // Update buttons only for the currently selected quest
            if (quest == currentSelectedQuest)
            {
                ChangeTrackerButtonDisplay(quest);
            }
        }
    }

    public void OnOpenJournal(params Transform[] questPanels)
    {
        List<Transform> allQuestLists = new List<Transform>();

        // Collect ALL "Quest List" transforms under both panels
        for (int i = 0; i < questPanels.Length; i++)
        {
            foreach (var list in questPanels[i].GetComponentsInChildren<Transform>(true))
            {
                if (list.name == "Quest List")
                    allQuestLists.Add(list);
            }
        }
        
        if (allQuestLists.Count == 0)
        {
            currentSelectedQuest = null;
            ClearDetails();
            ChangeTrackerButtonDisplay(currentSelectedQuest);
            return;
        }

        // Find the button of the tracked quest
        Button trackedButton = null;
        foreach (Transform questList in allQuestLists)
        {
            trackedButton = FindTrackedQuestButton(questList);
            if (trackedButton != null)
                break;
        }

        // If found, invoke it. Otherwise, fallback to first available quest button.
        if (trackedButton != null)
        {
            trackedButton.onClick.Invoke();
        }
        else
        {
            foreach (Transform questList in allQuestLists)
            {
                if (questList.childCount > 0)
                {
                    Button fallbackButton = questList.GetChild(0).GetComponent<Button>();
                    if (fallbackButton != null)
                    {
                        fallbackButton.onClick.Invoke();
                        break;
                    }
                }
            }
        }
    }

    private Button FindTrackedQuestButton(Transform questList)
    {
        if (questList == null) return null;

        foreach (Transform child in questList)
        {
            BB_QuestMetaData meta = child.GetComponent<BB_QuestMetaData>();
            if (meta != null)
            {
                BB_Quest matchingQuest = BB_QuestManager.Instance.allQuests
                    .FirstOrDefault(q => q.questTitle == child.name);

                if (matchingQuest != null && matchingQuest.isBeingTracked)
                {
                    return child.GetComponent<Button>();
                }
            }
        }

        return null;
    }

    public void AddQuestToJournal(BB_Quest quest)
    {
        // Determine panel and content holder based on quest type
        Transform parentScroll = quest.questType == BB_QuestType.Main ? mainQuestSelectionScrollContent : SideQuestSelectionScrollContent;
        //Transform parentScroll = allQuestSelectionScrollContent;

        string actKey = $"{quest.questType}_{quest.actNumber}";

        Transform actGroupContainer;

        // Check if group for this act already exists
        if (!actGroups.TryGetValue(actKey, out actGroupContainer))
        {
            // Instantiate new Act Group
            GameObject questGroupTemplateObj = Instantiate(questGroupTemplate, parentScroll);
            questGroupTemplateObj.SetActive(true);

            // Set act number text
            var actNumber = questGroupTemplateObj.transform.Find("Act Number").GetComponentInChildren<TextMeshProUGUI>();
            if (actNumber != null)
            {
                actNumber.text = quest.actNumber;
                actNumber.transform.parent.name = quest.actNumber;
            }
            actGroupContainer = questGroupTemplateObj.transform;
            actGroups[actKey] = actGroupContainer;

        }

        Transform questList = actGroupContainer.Find("Quest List").transform;

        // Instantiate the quest title button under the Act Group
        Button newQuestButton = Instantiate(questTitleTemplate, questList);
        newQuestButton.gameObject.SetActive(true);
        newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = quest.questTitle;
        newQuestButton.transform.name = quest.questTitle;

        // Add onClick to show details
        //newQuestButton.onClick.AddListener(() => ShowQuestDetails(quest));
        Button capturedButton = newQuestButton;
        Image capturedImage = newQuestButton.GetComponent<Image>();

        newQuestButton.onClick.AddListener(() =>
        {
            OnQuestButtonClicked(quest, capturedButton, capturedImage);
        });

        var meta = newQuestButton.gameObject.AddComponent<BB_QuestMetaData>();
        meta.questType = quest.questType;

        // Add onClick to set inactive Quest List if it hasnt set up yet
        Button actButton = actGroupContainer.Find(quest.actNumber).GetComponent<Button>();
        if (!initializedActButtons.Contains(actKey))
        {
            actButton.onClick.AddListener(() => ToggleActNumberGroup(questList.gameObject));
            initializedActButtons.Add(actKey);
        }
    }

    private void OnQuestButtonClicked(BB_Quest quest, Button clickedButton, Image clickedImage)
    {
        // Reset previous button to default sprite
        if (currentlySelectedImage != null)
        {
            currentlySelectedImage.sprite = defaultSprite;
        }

        // Set new button sprite to selected
        currentlySelectedButton = clickedButton;
        currentlySelectedImage = clickedImage;
        currentlySelectedImage.sprite = selectedSprite;

        // Show quest details
        ShowQuestDetails(quest);
    }

    public void ShowQuestDetails(BB_Quest quest)
    {
        currentSelectedQuest = quest;

        questDetailsQuestTitle.text = quest.questTitle;

        //Delete old Mission Details
        foreach (Transform child in questDetailsScrollViewContent.transform)
        {
            BB_QuestMissionTemplate missionTemplate = child.GetComponent<BB_QuestMissionTemplate>();
            if (missionTemplate != null)
            {
                Destroy(missionTemplate.gameObject);
            }
        }

        //Populate new Mission Details
        foreach (BB_Mission missions in quest.missions)
        {
            BB_QuestMissionTemplate missionHUD = Instantiate(questMissionTemplate, questDetailsScrollViewContent.transform);
            missionHUD.missionDescription.text = $"{missions.whatMustBeDone}";
            if (missions.hasCounter)
            {
                missionHUD.missionCounter.gameObject.SetActive(true);
                missionHUD.missionCounter.text = $"{missions.currentAmount}/{missions.requiredAmount}";
            }
            else missionHUD.missionCounter.gameObject.SetActive(false);
        }

        questDetailsBodyText.transform.SetAsLastSibling();

        if (quest.questJournalDescription != null)
        {
            switch (quest.state)
            {
                case QuestState.Active:
                    if (quest.questJournalDescription.Length > 0 && !string.IsNullOrEmpty(quest.questJournalDescription[0]))
                        questDetailsBodyText.text = quest.questJournalDescription[0];
                    break;

                case QuestState.Completed:
                    if (quest.questJournalDescription.Length > 1 && !string.IsNullOrEmpty(quest.questJournalDescription[1]))
                        questDetailsBodyText.text = quest.questJournalDescription[1];
                    break;

                case QuestState.Claimed:
                    if (quest.questJournalDescription.Length > 2 && !string.IsNullOrEmpty(quest.questJournalDescription[2]))
                        questDetailsBodyText.text = quest.questJournalDescription[2];
                    break;
            }      
        }

        questRewardText.text = "";

        // Clear old rewards
        foreach (Transform child in questRewardList)
            Destroy(child.gameObject);

        // Populate rewards
        foreach (BB_RewardSO reward in quest.rewards)
        {
            questRewardText.text = "Quest Rewards";

            GameObject rewardObj = Instantiate(questRewardGroupTemplate, questRewardList);
            rewardObj.SetActive(true);

            BB_IconUIGroup rewardUI = rewardObj.GetComponent<BB_IconUIGroup>();
            rewardUI.backgroundImage.sprite = reward.RewardBackground();
            rewardUI.icon.sprite = reward.RewardIcon();
            rewardUI.iconName.text = $"{reward.RewardQuantity().ToString()}  {reward.RewardName()}";
            rewardUI.quantity.text = reward.RewardQuantity().ToString();
        }
        ChangeTrackerButtonDisplay(quest);
    }

    public void ClearDetails()
    {
        questDetailsQuestTitle.text = "No Quest Selected";
        questDetailsBodyText.text = "You have not selected a quest";
        questRewardText.text = "";

        foreach (Transform child in questDetailsScrollViewContent.transform)
        {
            BB_QuestMissionTemplate missionTemplate = child.GetComponent<BB_QuestMissionTemplate>();
            if (missionTemplate != null)
            {
                Destroy(missionTemplate.gameObject);
            }
        }

        foreach (Transform child in questRewardList)
            Destroy(child.gameObject);
    }

    public void ToggleActNumberGroup(GameObject isObjectActive)
    {
        bool isActive = isObjectActive.gameObject.activeSelf;
        isObjectActive.gameObject.SetActive(!isActive);
    }

    public void ChangeSiblingArrangement(BB_Quest quest)
    {
        // Get the list container for the current quest
        string actKey = $"{quest.questType}_{quest.actNumber}";
        if (!actGroups.TryGetValue(actKey, out Transform actGroupContainer))
            return;

        Transform questList = actGroupContainer.Find("Quest List");
        if (questList == null) return;

        foreach (Transform child in questList)
        {
            if (child.name == quest.questTitle)
            {
                if (quest.state == QuestState.Claimed)
                {
                    child.SetAsLastSibling(); // move claimed quests to the bottom
                }
                else if (quest.isBeingTracked)
                {
                    child.SetAsFirstSibling(); // move tracked quests to the top
                }
                break;
            }
        }
    }

    public void TrackQuest(BB_Quest selectedQuest)
    {
        BB_Quest trackingQuest;

        if (selectedQuest != null)
            trackingQuest = selectedQuest;
        else
            trackingQuest = currentSelectedQuest;


        BB_QuestHUD.instance.trackedQuest = trackingQuest;

        foreach (BB_Quest quest in BB_QuestManager.Instance.activeMainQuests)
            quest.isBeingTracked = false;

        foreach (BB_Quest quest in BB_QuestManager.Instance.activeSideQuests)
            quest.isBeingTracked = false;


        trackingQuest.isBeingTracked = true;
        ChangeTrackerButtonDisplay(trackingQuest);
        ChangeSiblingArrangement(trackingQuest);
        BB_QuestHUD.instance.UpdateUI();
    }

    public void UnTrackQuest(BB_Quest selectedQuest)
    {
        BB_Quest trackingQuest;

        if (selectedQuest != null)
            trackingQuest = selectedQuest;
        else
            trackingQuest = currentSelectedQuest;

        BB_QuestHUD.instance.trackedQuest = null;

        trackingQuest.isBeingTracked = false;
        ChangeTrackerButtonDisplay(trackingQuest);
        BB_QuestHUD.instance.UpdateUI();
    }

    public void ChangeTrackerButtonDisplay(BB_Quest quest)
    {
        // Hide claim button by default
        buttonManager.claimRewardsButton.gameObject.SetActive(false);

        // If null, completed, or claimed → disable track/untrack, maybe show claim
        if (quest == null || quest.state == QuestState.Completed || quest.state == QuestState.Claimed)
        {
            buttonManager.questTrackButton.gameObject.SetActive(false);
            buttonManager.questUntrackButton.gameObject.SetActive(false);

            if (quest != null && quest.state == QuestState.Completed)
                buttonManager.claimRewardsButton.gameObject.SetActive(true);

            return;
        }

        // Toggle track/untrack
        buttonManager.questTrackButton.gameObject.SetActive(!quest.isBeingTracked);
        buttonManager.questUntrackButton.gameObject.SetActive(quest.isBeingTracked);

    }

    private void HandleQuestUpdate()
    {
        foreach (BB_Quest quest in BB_QuestManager.Instance.allQuests)
        {
            if (quest.state == QuestState.Claimed && !quest.isInCompletedPanel )//&& !IsInCompletedPanel(quest))
            {
                MoveQuestToCompletedPanel(quest);
            }
        }
    }

    public void MoveQuestToCompletedPanel(BB_Quest quest)
    {
        string actKey = $"{quest.questType}_{quest.actNumber}";

        quest.isInCompletedPanel = true ;

        // Try to get the Act Group in main or side quests
        if (!actGroups.TryGetValue(actKey, out Transform actGroup)) return;

        Transform questList = actGroup.Find("Quest List");
        Transform buttonToMove = null;

        // Find the quest button to move
        foreach (Transform child in questList)
        {
            if (child.name == quest.questTitle)
            {
                buttonToMove = child;
                break;
            }
        }

        if (buttonToMove != null)
        {
            bool willBeEmpty = (questList.childCount == 1);
            buttonToMove.SetParent(null);

            if (willBeEmpty)
            {
                Destroy(actGroup.gameObject);
                actGroups.Remove(actKey);
            }

            // === Handle Completed Quest Act Group ===

            Transform completedActGroup = null;
            Transform completedQuestList = null;

            // Check for an existing act group by inspecting the Act Number text
            foreach (Transform group in completedQuestSelectionScrollContent)
            {
                var actText = group.Find("Act Number")?.GetComponentInChildren<TextMeshProUGUI>();
                if (actText != null && actText.text == quest.actNumber)
                {
                    completedActGroup = group;
                    completedQuestList = group.Find("Quest List");
                    break;
                }
            }

            // If no existing group, create a new one
            if (completedActGroup == null)
            {
                GameObject newGroup = Instantiate(questGroupTemplate, completedQuestSelectionScrollContent);
                newGroup.SetActive(true);

                var actNumberText = newGroup.transform.Find("Act Number")?.GetComponentInChildren<TextMeshProUGUI>();
                if (actNumberText != null)
                {
                    actNumberText.text = quest.actNumber;
                }

                completedActGroup = newGroup.transform;
                completedQuestList = completedActGroup.Find("Quest List");
                // Make the Act group button clickable (for toggling quest list)
                Button actButton = completedActGroup.Find("Act Number")?.GetComponent<Button>();
                if (actButton != null)
                {
                    actButton.onClick.AddListener(() => ToggleActNumberGroup(completedQuestList.gameObject));
                }
            }


            // Move the button to the completed group's quest list
            buttonToMove.SetParent(completedQuestList, false);
            buttonToMove.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        OnOpenJournal(mainQuestSelectionScrollContent, SideQuestSelectionScrollContent);

        if (currentSelectedQuest == quest)
        {
            ClearDetails();
        }

        Debug.Log($"Added {quest.questID} to completed quest");
    }
    private bool IsInCompletedPanel(BB_Quest quest)
    {
        foreach (Transform child in completedQuestSelectionScrollContent)
        {
            if (child.name == quest.questTitle)
                return true;
        }
        return false;
    }

}