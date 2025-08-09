using UnityEngine;

public class BB_DomainEnemyDeathHandler : MonoBehaviour
{
    public System.Action onDeath;

    // Example — you'd replace this with your actual health/damage system
    public void OnDestroy()
    {
        onDeath?.Invoke();
    }
}
