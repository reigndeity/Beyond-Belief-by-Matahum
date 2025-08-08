using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BB_CampSpawner : MonoBehaviour
{
    private PlayerStats playerStats;
    [Tooltip("Calculation: baseStats * (multiplier * (playerStats.currentLevel / 10))")]
    public float enemyStatsMultiplier;
    public float spawnInterval;
    public int maxSpawnCount;
    public GameObject[] enemiesToSpawn;

    public List<EnemyStats> enemyList = new List<EnemyStats>();
    private SphereCollider spawnArea;
    private bool spawningInProgress = false;

    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        spawnArea = GetComponent<SphereCollider>();

        InvokeRepeating("StayWithinBoundaries", 0f, 1f);
        InvokeRepeating("StartSpawning", 0f, 1f);
    }

    public void StartSpawning()
    {
        // Check if all enemies are gone and not already waiting to respawn
        if (enemyList.Count == 0 && !spawningInProgress)
        {
            StartCoroutine(SpawnUnits());
        }
    }

    private void StayWithinBoundaries()
    {
        foreach (var enemy in enemyList)
        {
            if (Vector3.Distance(enemy.transform.position, transform.TransformPoint(spawnArea.center)) <= spawnArea.radius)
            {
                continue;
            }
            else
            {
                BlazeAI blaze = enemy.GetComponent<BlazeAI>();
                blaze.MoveToLocation(GetRandomPositionInRadius(transform.TransformPoint(spawnArea.center), spawnArea.radius));
            }
        }
    }

    IEnumerator SpawnUnits()
    {
        spawningInProgress = true;
        yield return new WaitForSeconds(spawnInterval);

        int spawnCount = Mathf.Min(maxSpawnCount - enemyList.Count, enemiesToSpawn.Length);

        for (int i = 0; i < maxSpawnCount; i++)
        {
            GameObject enemyGO = Instantiate(enemiesToSpawn[i % enemiesToSpawn.Length], 
                GetRandomPositionInRadius(transform.TransformPoint(spawnArea.center), spawnArea.radius), 
                Quaternion.identity, 
                transform);

            EnemyStats enemyStats = enemyGO.GetComponent<EnemyStats>();
            UpdateEnemyStats(enemyStats, enemyStatsMultiplier);

            if (enemyGO.GetComponent<BB_SpawnNotifier>() == null)
            {
                BB_SpawnNotifier notifier = enemyGO.AddComponent<BB_SpawnNotifier>();
                notifier.spawner = this;
                notifier.enemyStats = enemyStats;
            }
            
            enemyList.Add(enemyStats);
        }
        spawningInProgress = false;
    }

    private void UpdateEnemyStats(EnemyStats enemyStats, float multiplier)
    {
        enemyStats.e_level = Mathf.Max(1, playerStats.currentLevel / 10);
        float enemyLevel = Mathf.Max(1f, multiplier * enemyStats.e_level);

        enemyStats.e_baseMaxHealth = Mathf.RoundToInt(enemyStats.e_baseMaxHealth * enemyLevel);
        enemyStats.e_baseDefense = Mathf.RoundToInt(enemyStats.e_baseDefense * enemyLevel);
        enemyStats.e_baseAttack = Mathf.RoundToInt(enemyStats.e_baseAttack * enemyLevel);
    }

    Vector3 GetRandomPositionInRadius(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
    }
}
