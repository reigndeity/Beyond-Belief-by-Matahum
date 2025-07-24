using UnityEngine;
using System.Collections.Generic;
using System;

public class BB_QuestManager : MonoBehaviour
{
    public static BB_QuestManager Instance;

    // All available quests (e.g., from ScriptableObjects or hardcoded)
    [Header("All available Quests")]
    public List<BB_Quest> allQuests = new List<BB_Quest>();

    [Header("Accepted Quests")]
    public List<BB_Quest> activeMainQuests = new List<BB_Quest>();
    public List<BB_Quest> activeSideQuests = new List<BB_Quest>();

    public event Action OnQuestUpdate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
            allQuests = new List<BB_Quest>(Resources.LoadAll<BB_Quest>("Quests")); //Loads all available quests
        }
        else Destroy(gameObject);
    }

    public void AcceptQuest(BB_Quest quest)
    {
        if (quest == null) return;

        // Prevent duplicate acceptance
        if (activeMainQuests.Contains(quest) || activeSideQuests.Contains(quest)) return;

        switch (quest.questType)
        {
            case BB_QuestType.Main:
                activeMainQuests.Add(quest);
                break;
            case BB_QuestType.Side:
                activeSideQuests.Add(quest);
                break;
        }

        quest.state = QuestState.Active;
        foreach (var missions in quest.missions)
        {
            missions.currentAmount = 0;
        }

        OrganizeAcceptedQuests();
        BB_QuestJournalUI.Instance.AddQuestToJournal(quest);
    }

    public void AcceptQuestByID(string questID)
    {
        var quest = allQuests.Find(q => q.questID == questID);
        AcceptQuest(quest);
    }


    public Dictionary<BB_QuestType, Dictionary<string, List<BB_Quest>>> groupedQuests
           = new Dictionary<BB_QuestType, Dictionary<string, List<BB_Quest>>>();

    public void OrganizeAcceptedQuests()
    {
        groupedQuests.Clear();

        // Combine active quests into one list
        List<BB_Quest> allActive = new List<BB_Quest>();
        allActive.AddRange(activeMainQuests);
        allActive.AddRange(activeSideQuests);

        foreach (var quest in allActive)
        {
            if (!groupedQuests.ContainsKey(quest.questType))
            {
                groupedQuests[quest.questType] = new Dictionary<string, List<BB_Quest>>();
            }

            var actDict = groupedQuests[quest.questType];

            if (!actDict.ContainsKey(quest.actNumber))
            {
                actDict[quest.actNumber] = new List<BB_Quest>();
            }

            actDict[quest.actNumber].Add(quest);
        }
    }

    public void UpdateMissionProgress(string targetID, int amount = 1)
    {
        foreach (var quest in activeMainQuests)
        {
            UpdateQuestMissions(quest, targetID, amount);
        }

        foreach (var quest in activeSideQuests)
        {
            UpdateQuestMissions(quest, targetID, amount);
        }
    }

    private void UpdateQuestMissions(BB_Quest quest, string targetID, int amount)
    {
        bool questChanged = false;

        foreach (var mission in quest.missions)
        {
            if (!mission.isCompleted && mission.targetID == targetID)
            {
                mission.currentAmount += amount;

                if (mission.currentAmount >= mission.requiredAmount)
                {
                    mission.currentAmount = mission.requiredAmount;
                }

                questChanged = true;
            }
        }

        // Optionally notify the UI or system
        if (questChanged)
        {
            if (quest.IsCompleted)
            {
                quest.state = QuestState.Completed;
                Debug.Log($"Quest {quest.questTitle} is now COMPLETED!");
                // You can invoke a UnityEvent or notify UI here
            }
        }

        OnQuestUpdate?.Invoke();
    }

    public void ClaimRewards(BB_Quest quest)
    {
        if (quest.state == QuestState.Completed)
        {
            foreach (var reward in quest.rewards)
            {
                reward.GiveReward();
            }

            quest.state = QuestState.Claimed;

            OnQuestUpdate?.Invoke();
        }
    }
}
