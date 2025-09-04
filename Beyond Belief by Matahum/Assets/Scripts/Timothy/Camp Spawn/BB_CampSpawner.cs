using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BB_CampSpawner : MonoBehaviour
{
    private PlayerStats playerStats;

    [Header("Spawn Timing")]
    public float spawnInterval;

    [Header("Enemy Level Mapping (enemy = player + offset, clamped)")]
    [SerializeField] private int minEnemyLevel = 5;
    [SerializeField] private int maxEnemyLevel = 50;
    [SerializeField] private int levelOffset   = 5;

    public List<EnemyCount> enemiesToSpawn = new List<EnemyCount>();

    private List<EnemyStats> enemyList = new List<EnemyStats>();
    private SphereCollider spawnArea;
    private bool spawningInProgress = false;

    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        spawnArea = GetComponent<SphereCollider>();

        InvokeRepeating(nameof(StayWithinBoundaries), 0f, 1f);
        InvokeRepeating(nameof(StartSpawning), 0f, 1f);
        SpawnUnits();
    }

    public void RemoveEnemyInList(EnemyStats enemyStats)
    {
        enemyList.Remove(enemyStats);
    }

    public void StartSpawning()
    {
        // Check if all enemies are gone and not already waiting to respawn
        if (enemyList.Count == 0 && !spawningInProgress)
        {
            StartCoroutine(DelaySpawn());
        }
    }

    private void StayWithinBoundaries()
    {
        foreach (var enemy in enemyList)
        {
            if (enemy == null) continue;

            if (Vector3.Distance(enemy.transform.position, transform.TransformPoint(spawnArea.center)) <= spawnArea.radius)
                continue;

            BlazeAI blaze = enemy.GetComponent<BlazeAI>();
            if (blaze != null)
                blaze.MoveToLocation(GetRandomPositionInRadius(transform.TransformPoint(spawnArea.center), spawnArea.radius));
        }
    }

    IEnumerator DelaySpawn()
    {
        spawningInProgress = true;

        yield return new WaitForSeconds(spawnInterval);
        SpawnUnits();
    }
    void SpawnUnits()
    {
        foreach (var enemy in enemiesToSpawn)
        {
            for (int i = 0; i < enemy.count; i++)
            {
                Vector3 pos = GetRandomPositionInRadius(transform.TransformPoint(spawnArea.center), spawnArea.radius);

                GameObject enemyGO = Instantiate(enemy.enemyPrefab, pos, Quaternion.identity, transform);

                EnemyStats enemyStats = enemyGO.GetComponent<EnemyStats>();
                UpdateEnemyStats(enemyStats); // ✅ set correct level and stats here

                if (enemyGO.GetComponent<BB_SpawnNotifier>() == null)
                {
                    BB_SpawnNotifier notifier = enemyGO.AddComponent<BB_SpawnNotifier>();
                    notifier.spawner = this;
                    notifier.enemyStats = enemyStats;
                }

                enemyList.Add(enemyStats);
            }
        }

        spawningInProgress = false;
    }

    // ------------------ CORE CHANGE ------------------
    // Sets enemy level using your rule and refreshes ALL stats cleanly.
    private void UpdateEnemyStats(EnemyStats enemyStats)
    {
        if (enemyStats == null || playerStats == null) return;

        int pLevel = Mathf.Clamp(playerStats.currentLevel, 1, 50);
        int eLevel = ComputeEnemyLevel(pLevel, minEnemyLevel, maxEnemyLevel, levelOffset);

        // If your EnemyStats has a Definition and SetLevel/RecalculateStats (recommended)
        // then just use that to compute e_maxHealth/e_attack/e_defense/etc.
        // Otherwise, we still set level and try to recalc so you're not blocked.

        // Prefer SetLevel(...) if available (newer EnemyStats)
        try
        {
            enemyStats.SetLevel(eLevel);
            enemyStats.RecalculateStats();
        }
        catch (MissingMethodException)
        {
            // Legacy fallback if methods don't exist:
            enemyStats.e_level = eLevel;
            // If your legacy EnemyStats has base fields, you can add your old scaling here.
            // For safety, ensure current health resets to max.
        }

        // Start at full HP
        enemyStats.e_currentHealth = enemyStats.e_maxHealth;
    }
    // -------------------------------------------------

    private static int ComputeEnemyLevel(int playerLevel, int min, int max, int offset)
    {
        if (playerLevel >= max - offset) return max; // 45–50 -> 50
        if (playerLevel <  min)          return min; // 1–4   -> 5
        return Mathf.Clamp(playerLevel + offset, min, max);
    }

    Vector3 GetRandomPositionInRadius(Vector3 center, float radius)
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null)
            spawnArea = GetComponent<SphereCollider>();

        if (spawnArea != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // green, semi-transparent
            Gizmos.DrawSphere(transform.TransformPoint(spawnArea.center), spawnArea.radius);

            Gizmos.color = Color.green; // outline
            Gizmos.DrawWireSphere(transform.TransformPoint(spawnArea.center), spawnArea.radius);
        }
    }

}

[Serializable]
public class EnemyCount
{
    public GameObject enemyPrefab;
    public int count;
}
