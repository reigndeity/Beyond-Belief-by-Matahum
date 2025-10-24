using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BlazeAISpace;
using UnityEditor;

public class BB_CampSpawner : MonoBehaviour
{
    private PlayerStats playerStats;

    [Header("Spawn Distance Control")]
    [SerializeField] private float spawnDistanceThreshold = 50f;

    [Header("Enemy Level Mapping (enemy = player + offset, clamped)")]
    [SerializeField] private int minEnemyLevel = 5;
    [SerializeField] private int maxEnemyLevel = 50;
    [SerializeField] private int levelOffset = 5;

    [Header("Enemies To Spawn")]
    public List<EnemyCount> enemiesToSpawn = new List<EnemyCount>();

    public List<EnemyStats> enemyList = new List<EnemyStats>();
    private SphereCollider spawnArea;
    public bool spawningInProgress = false;

    [Header("Random Possible Lootdrops")]
    public LootContent[] possibleLootDrops;


    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        spawnArea = GetComponent<SphereCollider>();

        // Only spawn if none saved and player is far enough
        var player = FindFirstObjectByType<Player>();
        if (enemyList.Count == 0 && (player == null || Vector3.Distance(player.transform.position, transform.position) > spawnDistanceThreshold))
            SpawnUnits();

        StartCoroutine(BoundaryLoop());
        LoadSpawners();
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

                AddLoots(enemyGO);

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
        if (possibleLootDrops.Length == 0) return;

        int worldLevel;
        if (WorldLevelSetter.Instance != null)        
            worldLevel = WorldLevelSetter.Instance.worldLevel;       
        else worldLevel = 1;

        int randomQuantity = UnityEngine.Random.Range(0, 3);

        EnemyLootDrop loot = enemyGO.AddComponent<EnemyLootDrop>();

        for (int i = 0; i < randomQuantity; i++)
        {

            int randomLoot = UnityEngine.Random.Range(0, possibleLootDrops.Length);
            LootContent sourceLoot = possibleLootDrops[randomLoot];

            // 🟢 Create a new instance to avoid modifying the original
            int scaledLevel = Mathf.Clamp(worldLevel * 10, 1, Mathf.Min(maxEnemyLevel, 50));

            LootContent lootCopy = sourceLoot.Clone();
            lootCopy.level = scaledLevel;

            loot.lootContent.Add(lootCopy);
        }
    }

    // ------------------ CORE LEVEL LOGIC ------------------
    /*private void UpdateEnemyStats(EnemyStats enemyStats)
    {
        if (enemyStats == null || playerStats == null) return;

        //int pLevel = Mathf.Clamp(playerStats.currentLevel, 1, 50);
        int worldLevel = WorldLevelSetter.Instance.worldLevel;
        int eLevel = ComputeEnemyLevel(worldLevel, minEnemyLevel, maxEnemyLevel, levelOffset);

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
    }*/

    private void UpdateEnemyStats(EnemyStats enemyStats)
    {
        if (enemyStats == null) return;

        int worldLevel;
        if (WorldLevelSetter.Instance != null)
            worldLevel = WorldLevelSetter.Instance.worldLevel;
        else worldLevel = 1;

        int eLevel = ComputeEnemyLevelByWorld(worldLevel);

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

    /// <summary>
    /// Computes enemy level based on which world is active.
    /// Adjust the values per world as needed.
    /// </summary>
    private int ComputeEnemyLevelByWorld(int worldLevel)
    {
        switch (worldLevel)
        {
            case 1: // World 1: Early game
                    // Weakest enemies
                return Mathf.Clamp(minEnemyLevel + levelOffset, minEnemyLevel, maxEnemyLevel / 4);

            case 2: // World 2: Early-mid game
                return Mathf.Clamp((maxEnemyLevel / 4) + levelOffset, minEnemyLevel, maxEnemyLevel / 2);

            case 3: // World 3: Mid-late game
                return Mathf.Clamp((maxEnemyLevel / 2) + levelOffset, minEnemyLevel, (3 * maxEnemyLevel) / 4);

            case 4: // World 4: Endgame
                return Mathf.Clamp((3 * maxEnemyLevel / 4) + levelOffset, minEnemyLevel, maxEnemyLevel);

            default: // Fallback
                return Mathf.Clamp(minEnemyLevel + levelOffset, minEnemyLevel, maxEnemyLevel);
        }
    }



    // -------------------------------------------------------

    #region RESTOCK TIMER
    [Header("Item Restock Time")]
    private DateTime nextGlobalSpawnTime;
    [Tooltip("In Seconds")]
    public int spawnInterval = 60;
    public int currentTime;
    private const string GLOBAL_SPAWN_KEY = "GlobalCampRespawnTime";
    private void Update()
    {
        UpdateSpawnTimer();
    }

    private void LoadSpawners()
    {
        if (PlayerPrefs.HasKey(GLOBAL_SPAWN_KEY))
        {
            nextGlobalSpawnTime = DateTime.Parse(PlayerPrefs.GetString(GLOBAL_SPAWN_KEY));
        }
        else
        {
            nextGlobalSpawnTime = DateTime.UtcNow.AddSeconds(spawnInterval);
            PlayerPrefs.SetString(GLOBAL_SPAWN_KEY, nextGlobalSpawnTime.ToString());
        }
    }

    private void UpdateSpawnTimer()
    {
        TimeSpan remaining = nextGlobalSpawnTime - DateTime.UtcNow;
        currentTime = (int)remaining.TotalSeconds;

        if (enemyList.Count == 0 && !spawningInProgress)
        {
            if (remaining.TotalSeconds <= 0)
            {
                StartSpawning();

                nextGlobalSpawnTime = DateTime.UtcNow.AddSeconds(spawnInterval);
                PlayerPrefs.SetString(GLOBAL_SPAWN_KEY, nextGlobalSpawnTime.ToString());

            }
        }
    }

    #endregion

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
