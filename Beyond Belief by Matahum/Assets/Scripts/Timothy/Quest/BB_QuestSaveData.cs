using System.Collections.Generic;

[System.Serializable]
public class MissionSaveData
{
    public string targetID;
    public int currentAmount;
    public bool isCompleted;
}

[System.Serializable]
public class BB_QuestSaveData
{
    public string questID;
    public QuestState state;
    public bool isBeingTracked;
    public List<MissionSaveData> missions = new List<MissionSaveData>();
}

[System.Serializable]
public class QuestSaveWrapper
{
    public List<BB_QuestSaveData> savedQuests = new List<BB_QuestSaveData>();
}