using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public readonly List<Enemy> AllEnemies = new List<Enemy>();
    public static event System.Action<Enemy> OnEnemySpawned;

    void Awake() => Instance = this;

    public void Register(Enemy e)
    {
        AllEnemies.Add(e);
        OnEnemySpawned?.Invoke(e);
    }

    public void Unregister(Enemy e) => AllEnemies.Remove(e);
}
