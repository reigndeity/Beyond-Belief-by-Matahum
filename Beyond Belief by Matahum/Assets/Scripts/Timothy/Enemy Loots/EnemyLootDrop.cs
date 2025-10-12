using UnityEngine;
using System.Collections.Generic;

public class EnemyLootDrop : MonoBehaviour
{
    Enemy enemy;
    EnemyStats stats;
    public List<LootContent> lootContent = new List<LootContent>();

    [Header("Loot Physics Settings")]
    float explosionForce = 5f;         // how strong the items shoot out
    Vector3 spawnOffset = Vector3.up * 0.5f; // offset above chest

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        stats = GetComponent<EnemyStats>();

        enemy.OnDeath += OnDeath;

        InitializeLootLevel();
    }

    void InitializeLootLevel()
    {
        foreach (var loot in lootContent)
        {
            loot.level = stats.e_level;
        }
    }

    public void OnDeath()
    {
        Invoke("ShootOutLoot", 2f);
    }

    void ShootOutLoot()
    {
        Vector3 chestPosition = transform.position + spawnOffset;

        foreach (var chestDrop in lootContent)
        {
            // Spawn loot object
            LootDrops lootObj = Instantiate(chestDrop.lootPrefab, chestPosition, Quaternion.identity);
            lootObj.lootContent = chestDrop;
            lootObj.Initialize();

            Rigidbody rb = lootObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate random direction within a sphere
                Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
                randomDir.y = Mathf.Abs(randomDir.y) + 0.5f; // ensure it pops upward a bit

                // Apply explosion-like force
                rb.AddForce(randomDir.normalized * explosionForce, ForceMode.Impulse);
            }
        }
    }
}
