using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Domain", menuName = "Beyond Belief/Domain")]
public class BB_DomainSO : ScriptableObject
{
    public List<BB_EnemySet> enemySet = new List<BB_EnemySet>();
    public List<BB_RewardSO> rewards = new List<BB_RewardSO>();
}

[Serializable]
public class BB_DomainEnemy
{
    public GameObject enemyToSpawn;
    public float levelMultipler;
    public int howManyToSpawn;
}

[Serializable]
public class BB_EnemySet
{
    public List<BB_DomainEnemy> enemyList = new List<BB_DomainEnemy>();
    public float delayBeforeNextWave = 5f;
}