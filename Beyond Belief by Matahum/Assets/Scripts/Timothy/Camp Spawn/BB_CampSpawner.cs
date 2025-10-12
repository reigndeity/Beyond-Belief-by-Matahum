using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BlazeAISpace;
using UnityEditor;

public class BB_CampSpawner : MonoBehaviour
{
    private PlayerStats playerStats;

    [Header("Spawn Timing")]
    public float spawnInterval;
    [Header("Spawn Distance Control")]
    [SerializeField] private float spawnDistanceThreshold = 50f;

    [Header("Enemy Level Mapping (enemy = player + offset, clamped)")]
    [SerializeField] private int minEnemyLevel = 5;
    [SerializeField] private int maxEnemyLevel = 50;
    [SerializeField] private int levelOffset = 5;

    [Header("Enemies To Spawn")]
    public List<EnemyCount> enemiesToSpawn = new List<EnemyCount>();

    private List<EnemyStats> enemyList = new List<EnemyStats>();
    private SphereCollider spawnArea;
    private bool spawningInProgress = false;

    // 🧠 NEW: Saved enemy data for persistence
    private List<EnemySaveData> savedEnemies = new();

    [Header("Random Possible Lootdrops")]
    public LootContent[] possibleLootDrops;


    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        spawnArea = GetComponent<SphereCollider>();

        // Load existing enemies if saved
        //LoadEnemies();

        // Only spawn if none saved and player is far enough
        var player = FindFirstObjectByType<Player>();
        if (enemyList.Count == 0 && (player == null || Vector3.Distance(player.transform.position, transform.position) > spawnDistanceThreshold))
            SpawnUnits();

        StartCoroutine(SpawnLoop());
        StartCoroutine(BoundaryLoop());
    }


    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            StartSpawning();
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator BoundaryLoop()
    {
        while (true)
        {
            StayWithinBoundaries();
            yield return new WaitForSeconds(1f);
        }
    }

    public void RemoveEnemyInList(EnemyStats enemyStats)
    {
        enemyList.Remove(enemyStats);
    }

    public void StartSpawning()
    {
        enemyList.RemoveAll(e => e == null); // ✅ cleanup destroyed ones

        // Spawn if empty and not currently waiting
        if (enemyList.Count == 0 && !spawningInProgress)
        {
            StartCoroutine(DelaySpawn());
        }
    }

    private void StayWithinBoundaries()
    {
        if (spawnArea == null) return;

        foreach (var enemy in enemyList)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(enemy.transform.position, transform.TransformPoint(spawnArea.center));
            if (distance <= spawnArea.radius) continue;

            BlazeAI blaze = enemy.GetComponent<BlazeAI>();
            if (blaze != null && blaze.state == BlazeAI.State.normal) // ✅ safer check
                blaze.MoveToLocation(GetRandomPositionInRadius(transform.TransformPoint(spawnArea.center), spawnArea.radius));
        }
    }

    IEnumerator DelaySpawn()
    {
        spawningInProgress = true;

        // Wait for the interval first
        yield return new WaitForSeconds(spawnInterval);

        // Wait until player is far enough away
        var player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            while (Vector3.Distance(player.transform.position, transform.position) < spawnDistanceThreshold)
                yield return new WaitForSeconds(1f); // check every second
        }

        SpawnUnits();
    }

    void SpawnUnits()
    {
        foreach (var enemy in enemiesToSpawn)
        {
            for (int i = 0; i < enemy.count; i++)
            {
                Vector3 pos = GetRandomPositionInRadius(transform.TransformPoint(spawnArea.center), spawnArea.radius);
                AdjustYToGround(ref pos);

                GameObject enemyGO = Instantiate(enemy.enemyPrefab, pos, Quaternion.identity, transform);
                EnemyStats enemyStats = enemyGO.GetComponent<EnemyStats>();
                UpdateEnemyStats(enemyStats);

                //AddLoots(enemyGO);
                Debug.Log($"Possible Loot Drop Length: {possibleLootDrops.Length}");
                if (possibleLootDrops.Length != 0)
                {
                    int randomLoot = UnityEngine.Random.Range(0, possibleLootDrops.Length);
                    int randomQuantity = UnityEngine.Random.Range(1, 4);
                    EnemyLootDrop loot = enemyGO.AddComponent<EnemyLootDrop>();

                    for (int j = 0; j < randomQuantity; j++)
                    {
                        loot.lootContent.Add(possibleLootDrops[randomLoot]);
                    }
                }

                // Setup notifier
                if (enemyGO.GetComponent<BB_SpawnNotifier>() == null)
                {
                    BB_SpawnNotifier notifier = enemyGO.AddComponent<BB_SpawnNotifier>();
                    notifier.spawner = this;
                    notifier.enemyStats = enemyStats;
                }

                // Add to list
                enemyList.Add(enemyStats);

                // Setup AI
                BlazeAI blazeAI = enemyGO.GetComponent<BlazeAI>();
                if (blazeAI != null)
                {
                    blazeAI.deathCallRadius = 10;
                    blazeAI.agentLayersToDeathCall = LayerMask.GetMask("Enemy");
                }

                HitStateBehaviour hitState = enemyGO.GetComponent<HitStateBehaviour>();
                if (hitState != null)
                {
                    hitState.callOthersRadius = 10;
                    hitState.agentLayersToCall = LayerMask.GetMask("Enemy");
                }
            }
        }

        spawningInProgress = false;
    }

    void AddLoots(GameObject enemyGO)
    {
        Debug.Log($"Possible Loot Drop Length: {possibleLootDrops.Length}");
        if (possibleLootDrops.Length != 0)
        {
            int randomLoot = UnityEngine.Random.Range(0, possibleLootDrops.Length);
            int randomQuantity  = UnityEngine.Random.Range(1, 4);
            EnemyLootDrop loot = enemyGO.AddComponent<EnemyLootDrop>();
            
            for(int i = 0; i < randomQuantity; i++)
            {
                loot.lootContent.Add(possibleLootDrops[randomLoot]);
            }
        }
    }

    // ------------------ CORE LEVEL LOGIC ------------------
    private void UpdateEnemyStats(EnemyStats enemyStats)
    {
        if (enemyStats == null || playerStats == null) return;

        int pLevel = Mathf.Clamp(playerStats.currentLevel, 1, 50);
        int eLevel = ComputeEnemyLevel(pLevel, minEnemyLevel, maxEnemyLevel, levelOffset);

        try
        {
            enemyStats.SetLevel(eLevel);
            enemyStats.RecalculateStats();
        }
        catch (MissingMethodException)
        {
            enemyStats.e_level = eLevel;
        }

        enemyStats.e_currentHealth = enemyStats.e_maxHealth;
    }

    private static int ComputeEnemyLevel(int playerLevel, int min, int max, int offset)
    {
        if (playerLevel >= max - offset) return max;
        if (playerLevel < min) return min;
        return Mathf.Clamp(playerLevel + offset, min, max);
    }
    // -------------------------------------------------------

    Vector3 GetRandomPositionInRadius(Vector3 center, float radius)
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);
    }

    private void AdjustYToGround(ref Vector3 pos)
    {
        if (Physics.Raycast(new Vector3(pos.x, pos.y + 10f, pos.z), Vector3.down, out RaycastHit hit, 50f, LayerMask.GetMask("Ground")))
        {
            pos.y = hit.point.y;
        }
    }

    // ------------------ SAVE / LOAD ------------------
    public void CaptureCurrentEnemies()
    {
        savedEnemies.Clear();

        foreach (var enemy in enemyList)
        {
            if (enemy == null) continue;

            EnemySaveData data = new EnemySaveData
            {
                prefabName = enemy.name.Replace("(Clone)", "").Trim(),
                position = enemy.transform.position,
                rotation = enemy.transform.rotation,
                level = enemy.e_level
            };

            savedEnemies.Add(data);
        }

        ES3.Save($"Camp_{name}_Enemies", savedEnemies);
    }

    public void LoadEnemies()
    {
        if (!ES3.KeyExists($"Camp_{name}_Enemies"))
            return;

        savedEnemies = ES3.Load<List<EnemySaveData>>($"Camp_{name}_Enemies");
        if (savedEnemies == null || savedEnemies.Count == 0)
            return;

        foreach (var data in savedEnemies)
        {
            GameObject prefab = GetPrefabByName(data.prefabName);
            if (prefab == null) continue;

            GameObject enemyGO = Instantiate(prefab, data.position, data.rotation, transform);
            EnemyStats stats = enemyGO.GetComponent<EnemyStats>();

            stats.e_level = data.level;
            stats.e_currentHealth = stats.e_maxHealth;
            enemyList.Add(stats);

            // Reconnect notifier
            if (enemyGO.GetComponent<BB_SpawnNotifier>() == null)
            {
                BB_SpawnNotifier notifier = enemyGO.AddComponent<BB_SpawnNotifier>();
                notifier.spawner = this;
                notifier.enemyStats = stats;
            }
        }
    }

    private GameObject GetPrefabByName(string prefabName)
    {
        foreach (var enemy in enemiesToSpawn)
        {
            if (enemy.enemyPrefab.name == prefabName)
                return enemy.enemyPrefab;
        }
        return null;
    }

    private void OnDisable()
    {
        //CaptureCurrentEnemies(); // Auto-save remaining enemies when scene unloads
    }
    // -------------------------------------------------------

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
        // 🔵 Draw spawn distance debug zone
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.15f); // soft transparent blue
        Gizmos.DrawSphere(transform.position, spawnDistanceThreshold);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnDistanceThreshold);
    }
    
}

[Serializable]
public class EnemyCount
{
    public GameObject enemyPrefab;
    public int count;
}

// 🧩 NEW: Simple struct for save/load
[Serializable]
public class EnemySaveData
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public int level;
}
