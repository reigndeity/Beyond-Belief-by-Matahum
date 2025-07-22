using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public enum BB_QuestType { Main, Side }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Beyond Belief/Quests/Quest")]
public class BB_Quest : ScriptableObject
{
    public string questID; //Quest Unique ID to call
    public string questTitle; //Quest name
    public string actNumber; //Quest's Act Group
    public string description; //Description for the Quest
    public BB_QuestType questType; // e.g., Main, Side
    public List<BB_Mission> missions = new List<BB_Mission>();
    public List<BB_QuestReward> rewards = new List<BB_QuestReward>();

    public bool IsCompleted => missions.TrueForAll(m => m.isCompleted);
}

[Serializable]
public class BB_Mission
{
    public string description;
    public string targetID; // EnemyID, ItemID, etc.
    public int requiredAmount;
    public int currentAmount;
    public bool isCompleted;
}

[Serializable]
public class BB_QuestReward
{
    public Sprite rewardIcon;
    public string rewardName;
    public int rewardQuantity;
    public UnityEvent rewardEvent;
}