using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
    public Transform questRewardList;
    public GameObject questRewardGroupTemplate;
    public Image questRewardIcon;
    public TextMeshProUGUI questRewardQuantity;
    public TextMeshProUGUI questRewardTitle;

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
        //questGroupTemplate.SetActive(false);
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
        newQuestButton.onClick.AddListener(() => ShowQuestDetails(quest));

        // Add onClick to set inactive Quest List if it hasnt set up yet
        Button actButton = actGroupContainer.Find(quest.actNumber).GetComponent<Button>();
        if (!initializedActButtons.Contains(actKey))
        {
            actButton.onClick.AddListener(() => ToggleActNumberGroup(questList.gameObject));
            initializedActButtons.Add(actKey);
        }

    }

    public void ShowQuestDetails(BB_Quest quest)
    {
        questDetailsQuestTitle.text = quest.questTitle;
        questDetailsBodyText.text = quest.description;

        // Clear old rewards
        foreach (Transform child in questRewardList)
            Destroy(child.gameObject);

        // Populate rewards
        foreach (var reward in quest.rewards)
        {
            GameObject rewardObj = Instantiate(questRewardGroupTemplate, questRewardList);
            rewardObj.SetActive(true);

            // Assign icon, quantity, title
            rewardObj.transform.Find("Quest Reward Icon").GetComponent<Image>().sprite = reward.rewardIcon;
            rewardObj.transform.Find("Quest Reward Quantity").GetComponent<TextMeshProUGUI>().text = reward.rewardQuantity.ToString();
            rewardObj.transform.Find("Quest Reward Name").GetComponent<TextMeshProUGUI>().text = reward.rewardName;
        }
    }

    public void ToggleActNumberGroup(GameObject isObjectActive)
    {
        bool isActive = isObjectActive.gameObject.activeSelf;
        isObjectActive.gameObject.SetActive(!isActive);
    }
}
