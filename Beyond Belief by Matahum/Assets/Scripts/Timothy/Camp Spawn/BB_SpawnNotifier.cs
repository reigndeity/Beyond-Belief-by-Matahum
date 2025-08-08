using UnityEngine;

public class BB_SpawnNotifier : MonoBehaviour
{
    public BB_CampSpawner spawner;
    public EnemyStats enemyStats;
    private void OnDestroy()
    {
        spawner.RemoveEnemyInList(enemyStats);
    }
}
