using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class BB_QuestJournalUI : MonoBehaviour
{
    public static BB_QuestJournalUI Instance;

    [Header("Main Quest Category")]
    public GameObject mainQuestPanel;
    public Transform mainQuestSelectionScrollContent;  

    [Header("Side Quest Category")]
    public GameObject SideQuestPanel;
    public Transform SideQuestSelectionScrollContent;

    [Header("Quest Group Template")]
    public GameObject questGroupTemplate;
    public Button questTitle;

    [Header("Quest Details")]
    public TextMeshProUGUI questDetailsQuestTitle;
    public TextMeshProUGUI questDetailsBodyText;
    public TextMeshProUGUI questRewardText;
    public Transform questRewardList;
    public GameObject questRewardGroupTemplate;

    [Header("Quest Selected Indicator")] 
    public Sprite defaultSprite;
    public Sprite selectedSprite; // Assign this manually or load it
    private Button currentlySelectedButton;
    private Image currentlySelectedImage;
    
    [Header("Quest Tracker")]
    public BB_Quest currentSelectedQuest;
    public Button questTrackButton;
    public Button questUntrackButton;

    // Holds instantiated Act groups for reuse
    private Dictionary<string, Transform> actGroups = new Dictionary<string, Transform>();
    private HashSet<string> initializedActButtons = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        questTrackButton.onClick.AddListener(TrackQuest);
        questUntrackButton.onClick.AddListener(UnTrackQuest);
    }

    public void OnOpenJournal(Transform questPanel) //This will auto select the first Quest when opening either the Journal, or switching between quest panels
    {
        Transform questList = questPanel
            .GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.name == "Quest List");

        if (questList == null || questList.childCount == 0)
        {
            currentSelectedQuest = null;
            ClearDetails();
            ChangeTrackerButtonDisplay(currentSelectedQuest);
        }
        else
        {
            Button questButton = questList.GetChild(0).GetComponent<Button>();
            if (questButton != null)
            {
                questButton.onClick.Invoke();
            }
        }     
    }

    public void AddQuestToJournal(BB_Quest quest)
    {
        // Determine panel and content holder based on quest type
        Transform parentScroll = quest.questType == BB_QuestType.Main ? mainQuestSelectionScrollContent : SideQuestSelectionScrollContent;

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
        Button newQuestButton = Instantiate(questTitle, questList);
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

            BB_QuestRewardUIGroup rewardUI = rewardObj.GetComponent<BB_QuestRewardUIGroup>();
            rewardUI.rewardIcon.sprite = reward.RewardIcon();
            rewardUI.rewardName.text = reward.RewardName();
            rewardUI.rewardQuantity.text = reward.RewardQuantity().ToString();

            Debug.Log("Theres a reward");
        }


        ChangeTrackerButtonDisplay(quest);
    }

    public void ClearDetails()
    {
        questDetailsQuestTitle.text = "You have no quest yet";
        questDetailsBodyText.text = "";
        questRewardText.text = "";

        foreach (Transform child in questRewardList)
            Destroy(child.gameObject);
    }

    public void ToggleActNumberGroup(GameObject isObjectActive)
    {
        bool isActive = isObjectActive.gameObject.activeSelf;
        isObjectActive.gameObject.SetActive(!isActive);
    }

    public void RewardSample()
    {
        Debug.Log("Received Reward");
    }

    public void TrackQuest()
    {
        if (currentSelectedQuest.questType == BB_QuestType.Main)
        {
            BB_QuestHUD.instance.trackedMainQuest = currentSelectedQuest;

            foreach (BB_Quest quest in BB_QuestManager.Instance.activeMainQuests)
                quest.isBeingTracked = false;
        }
        else
        {
            BB_QuestHUD.instance.trackedSideQuest = currentSelectedQuest;

            foreach (BB_Quest quest in BB_QuestManager.Instance.activeSideQuests)
                quest.isBeingTracked = false;
        }

        currentSelectedQuest.isBeingTracked = true;
        ChangeTrackerButtonDisplay(currentSelectedQuest);
        ChangeSiblingArrangement();
        BB_QuestHUD.instance.UpdateUI();
    }

    public void UnTrackQuest()
    {
        if (currentSelectedQuest.questType == BB_QuestType.Main)
            BB_QuestHUD.instance.trackedMainQuest = null;
        else
            BB_QuestHUD.instance.trackedSideQuest = null;

        currentSelectedQuest.isBeingTracked = false;
        ChangeTrackerButtonDisplay(currentSelectedQuest);
        BB_QuestHUD.instance.UpdateUI();
    }

    public void ChangeSiblingArrangement()
    {
        // Get the list container for the current quest
        string actKey = $"{currentSelectedQuest.questType}_{currentSelectedQuest.actNumber}";
        if (!actGroups.TryGetValue(actKey, out Transform actGroupContainer))
            return;

        Transform questList = actGroupContainer.Find("Quest List");
        if (questList == null) return;

        foreach (Transform child in questList)
        {
            if (child.name == currentSelectedQuest.questTitle)
            {
                if (currentSelectedQuest.state == QuestState.Claimed)
                {
                    child.SetAsLastSibling(); // move claimed quests to the bottom
                }
                else if (currentSelectedQuest.isBeingTracked)
                {
                    child.SetAsFirstSibling(); // move tracked quests to the top
                }
                break;
            }
        }
    }


    public void ChangeTrackerButtonDisplay(BB_Quest quest)
    {
        if (quest == null || quest.state == QuestState.Claimed)
        {
            questTrackButton.gameObject.SetActive(false);
            questUntrackButton.gameObject.SetActive(false);
            Debug.Log("Track and Untrack Button disabled");
            return;
        }

        if (quest.isBeingTracked)
        {
            questTrackButton.gameObject.SetActive(false);
            questUntrackButton.gameObject.SetActive(true);
        }
        else
        {
            questTrackButton.gameObject.SetActive(true);
            questUntrackButton.gameObject.SetActive(false);
        }
    }
}
