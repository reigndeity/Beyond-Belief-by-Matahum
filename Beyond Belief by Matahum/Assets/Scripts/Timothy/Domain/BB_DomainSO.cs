using UnityEngine;
using System;
using System.Collections.Generic;

public enum DomainSpawnMode
{
    TimeBased,   // Spawn each set after delay
    ClearBased   // Spawn next set only when current set is cleared
}

[CreateAssetMenu(fileName = "New Domain", menuName = "Beyond Belief/Domain")]
public class BB_DomainSO : ScriptableObject
{
    [Header("Domain Info")]
    public Sprite domainImage;
    public string domainName;
    [TextArea(2, 3)]
    public string domainDescription;

    [Header("Next Level Access Properties")]
    public int levelAccessIndex = 0;

    [Header("Enemy Waves")]
    public List<BB_EnemySet> enemySets = new List<BB_EnemySet>();
    public DomainSpawnMode spawnMode = DomainSpawnMode.TimeBased;

    [Header("Rewards")]
    public RewardSet[] rewards;
    public int levelMultiplier = 1;

    [Header("Timers")]
    public float totalTime = 300f; // total time to complete the domain

    [Header("Quest Related Domain")]
    public bool isDomainClearedForQuest = false;
    public string questTargetID;    
}

[Serializable]
public class BB_DomainEnemy
{
    public GameObject enemyToSpawn;
    public int howManyToSpawn;
}

[Serializable]
public class BB_EnemySet
{
    public List<BB_DomainEnemy> enemyList = new List<BB_DomainEnemy>();

    [Tooltip("For TimeBased mode: Delay before this wave starts")]
    public float delayBeforeNextWave = 5f;
}

[Serializable]
public class RewardSet
{
    public List<BB_RewardSO> rewards = new List<BB_RewardSO>();
}
