using UnityEngine;

public static class EnemySpawnUtil
{
    /// Spawns an enemy and sets its stats based on the player's level + your mapping.
    public static GameObject SpawnEnemyWithStats(
        GameObject enemyPrefab,
        EnemyDefinition definition,
        int playerLevel,
        Vector3 position,
        Quaternion rotation,
        int minEnemyLevel = 5,
        int maxEnemyLevel = 50,
        int levelOffset   = 5)
    {
        if (enemyPrefab == null) return null;

        GameObject go = Object.Instantiate(enemyPrefab, position, rotation);

        if (go.TryGetComponent<EnemyStats>(out var stats))
        {
            // Map player level -> enemy level (your rule)
            int enemyLevel =
                (playerLevel >= maxEnemyLevel - levelOffset) ? maxEnemyLevel :
                (playerLevel <  minEnemyLevel)               ? minEnemyLevel :
                Mathf.Clamp(playerLevel + levelOffset, minEnemyLevel, maxEnemyLevel);

            if (definition != null) stats.definition = definition;

            // If you added Initialize(def, level), you can call that instead of the two lines below.
            stats.e_level = enemyLevel;
            stats.RecalculateStats();
        }

        return go;
    }
}
