using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BB_QuestSaveSystem : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "quests.json");
    }

    public void SaveQuests(List<BB_Quest> allQuests)
    {
        QuestSaveWrapper wrapper = new QuestSaveWrapper();

        foreach (var quest in allQuests)
        {
            BB_QuestSaveData questData = new BB_QuestSaveData
            {
                questID = quest.questID,
                state = quest.state,
                isBeingTracked = quest.isBeingTracked
            };

            foreach (var mission in quest.missions)
            {
                questData.missions.Add(new MissionSaveData
                {
                    targetID = mission.targetID,
                    currentAmount = mission.currentAmount,
                    isCompleted = mission.isCompleted
                });
            }

            wrapper.savedQuests.Add(questData);
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Saved quests to {savePath}");
    }

    public void LoadQuests(List<BB_Quest> allQuests)
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No quest save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        QuestSaveWrapper wrapper = JsonUtility.FromJson<QuestSaveWrapper>(json);

        foreach (var saved in wrapper.savedQuests)
        {
            BB_Quest quest = allQuests.Find(q => q.questID == saved.questID);
            if (quest != null)
            {
                quest.state = saved.state;
                quest.isBeingTracked = saved.isBeingTracked;

                // Restore mission progress
                foreach (var savedMission in saved.missions)
                {
                    var mission = quest.missions.Find(m => m.targetID == savedMission.targetID);
                    if (mission != null)
                    {
                        mission.currentAmount = savedMission.currentAmount;
                        // mission.isCompleted is derived automatically from currentAmount >= requiredAmount
                    }
                }
            }
        }

        Debug.Log("Loaded quests from save.");
    }
}
