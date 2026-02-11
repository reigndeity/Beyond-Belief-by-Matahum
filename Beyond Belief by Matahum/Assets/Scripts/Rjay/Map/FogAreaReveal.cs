using UnityEngine;

[RequireComponent(typeof(PersistentGuid))]
public class FogAreaReveal : MonoBehaviour
{
    [Tooltip("IDs of map areas to reveal when triggered.")]
    [SerializeField] private string[] revealAreaIds;

    private PersistentGuid persistentGuid;
    private bool hasBeenRevealed = false;

    private void Awake()
    {
        persistentGuid = GetComponent<PersistentGuid>();
    }

    /// <summary>
    /// Immediately reveal this area's fog on the map.
    /// </summary>
    public void RevealNow()
    {
        if (MapManager.instance == null)
        {
            Debug.LogWarning("MapManager not found in scene.");
            return;
        }

        if (revealAreaIds != null && revealAreaIds.Length > 0)
        {
            MapManager.instance.RevealAreas(revealAreaIds);
            hasBeenRevealed = true;
            Debug.Log($"[FogAreaReveal] Revealed {revealAreaIds.Length} areas from {gameObject.name}");
        }
    }

    /// <summary>
    /// Check if this fog area has been revealed already.
    /// (Optional helper if you need to query state).
    /// </summary>
    public bool IsRevealed() => hasBeenRevealed;
}
