using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class BB_QuestManager : MonoBehaviour
{
    public static BB_QuestManager Instance;

    // All available quests (e.g., from ScriptableObjects or hardcoded)
    [Header("All available Quests")]
    public List<BB_Quest> allQuests = new List<BB_Quest>();

    [Header("Accepted Quests")]
    public List<BB_Quest> activeMainQuests = new List<BB_Quest>();
    public List<BB_Quest> activeSideQuests = new List<BB_Quest>();

    private BB_QuestSaveSystem saveSystem;

    public event Action OnQuestUpdate;

    [Header("For Clearing Quest Save Data(Dev Stuff)")]
    private string savePath;
    public bool clearQuestSaveData = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
            allQuests = new List<BB_Quest>(Resources.LoadAll<BB_Quest>("Quests")); //Loads all available quests
            saveSystem = gameObject.AddComponent<BB_QuestSaveSystem>();
            savePath = Path.Combine(Application.persistentDataPath, "quests.json");
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        saveSystem.LoadQuests(allQuests);
        //LoadQuestToJournal();
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
        quest.isBeingTracked = false;
        foreach (var missions in quest.missions)
        {
            missions.currentAmount = 0;
        }

        OrganizeAcceptedQuests();
        BB_QuestJournalUI.instance.AddQuestToJournal(quest);

        if (BB_QuestHUD.instance.trackedQuest == null)
            BB_QuestJournalUI.instance.TrackQuest(quest);
    }

    public void AcceptQuestByID(string questID)
    {
        var quest = allQuests.Find(q => q.questID == questID);
        AcceptQuest(quest);
    }


    public Dictionary<BB_QuestType, Dictionary<string, List<BB_Quest>>> groupedQuests
           = new Dictionary<BB_QuestType, Dictionary<string, List<BB_Quest>>>();

    private void OrganizeAcceptedQuests()
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

    public void UpdateMissionProgressOnce(string targetID)
    {
        UpdateMissionProgress(targetID, 1);
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
            BB_QuestHUD.instance.trackedQuest = null;
            OnQuestUpdate?.Invoke();
        }
    }

    public void ClaimRewardsByID(string questID)
    {
        var quest = allQuests.Find(q => q.questID == questID);
        if (quest != null)
        {
            ClaimRewards(quest);
        }
        else
        {
            Debug.LogWarning($"Quest with ID {questID} not found.");
        }
    }

    // DEV FUNCTIONS
    public void DebugCompleteAndClaimTrackedQuest()
    {
        var quest = BB_QuestHUD.instance.trackedQuest;
        if (quest == null)
        {
            Debug.LogWarning("⚠️ No quest is currently tracked.");
            return;
        }

        // Force complete all missions
        foreach (var mission in quest.missions)
        {
            mission.currentAmount = mission.requiredAmount;
        }

        quest.state = QuestState.Completed;

        // Claim instantly
        ClaimRewards(quest);

        Debug.Log($"✅ DEBUG: Force-completed and claimed quest: {quest.questTitle}");
    }


    #region Saving Quest Data
    public void SaveQuestData()
    {
        saveSystem.SaveQuests(allQuests);
    }
    #endregion

    #region Load Quest Data
    public void LoadQuestToJournal()
    {
        foreach (var quest in allQuests)
        {
            if (quest.state != QuestState.Inactive)
            {
                BB_QuestJournalUI.instance.AddQuestToJournal(quest);

                if (quest.state == QuestState.Claimed)
                    BB_QuestJournalUI.instance.MoveQuestToCompletedPanel(quest);

                if (quest.isBeingTracked)
                {
                    BB_QuestJournalUI.instance.TrackQuest(quest);
                }
            }
        }
    }
    public void LoadQuestData()
    {
        saveSystem.LoadQuests(allQuests);
        LoadQuestToJournal();
    }
    #endregion

    #region Clearing Quest Save Data
    /// <summary>
    /// Deletes quest save data from disk and resets quest states.
    /// </summary>
    public void ClearQuestSaveData()
    {
        // Delete the save file if it exists
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"Quest save data cleared at {savePath}");
        }
        else
        {
            Debug.Log("No quest save file found to clear.");
        }

        // Reset in-memory quest states
        foreach (BB_Quest quest in allQuests)
        {
            quest.state = QuestState.Inactive;
            quest.isBeingTracked = false;

            foreach (var mission in quest.missions)
            {
                mission.currentAmount = 0;
            }
        }

        // Tell the journal UI to refresh (so it hides everything)
        if (BB_QuestJournalUI.instance != null)
        {
            BB_QuestJournalUI.instance.ClearDetails();
            // You could also trigger a full rebuild here if you want
        }
    }
    #endregion

    public bool IsQuestDone(string questID)
    {
        var quest = BB_QuestManager.Instance.allQuests.Find(q => q.questID == questID);
        if (quest == null) return false;

        return quest.state == QuestState.Completed || quest.state == QuestState.Claimed;
    }
}
