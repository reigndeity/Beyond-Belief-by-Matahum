using System;
using UnityEngine;
using System.Collections.Generic;

public enum BB_QuestType
{
    Main,
    Side
}

public enum QuestState
{
    Inactive,
    Active,
    Completed,
    Claimed
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Beyond Belief/Quests/Quest")]
public class BB_Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questID;
    public string questTitle;
    public string actNumber;
    [TextArea(3, 10)]
    public string[] questJournalDescription;
    public string questHudDescription;
    public string questHudWhereToClaimQueDescription;
    public BB_QuestType questType;
    public QuestState state = QuestState.Inactive;

    [Header("Missions")]
    public List<BB_Mission> missions = new List<BB_Mission>();

    [Header("Rewards")]
    public List<BB_RewardSO> rewards = new List<BB_RewardSO>();

    public bool IsCompleted => missions.TrueForAll(m => m.isCompleted);
    public bool isBeingTracked = false;
}

[Serializable]
public class BB_Mission
{
    public string whatMustBeDone; //example "Killed Duwende" 
    public string targetID; // example "killedDuwende"
    public int requiredAmount = 1;
    public int currentAmount = 0;
    public bool isCompleted => currentAmount >= requiredAmount;
}