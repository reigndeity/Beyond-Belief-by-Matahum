using UnityEngine;

[RequireComponent(typeof(PersistentGuid))]
public class FogAreaReveal : MonoBehaviour, ISaveable
{
    [Tooltip("IDs of map areas to reveal when triggered.")]
    [SerializeField] private string[] revealAreaIds;

    private PersistentGuid persistentGuid;
    private bool hasBeenRevealed = false;

    private void Awake()
    {
        persistentGuid = GetComponent<PersistentGuid>();
        SaveManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        SaveManager.Instance?.Unregister(this);
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

    // ---------------- SAVE SYSTEM HOOKS ----------------

    public string SaveId => $"FogArea.{persistentGuid.Guid}";

    [System.Serializable]
    private class DTO
    {
        public bool revealed;
    }

    public string CaptureJson()
    {
        return JsonUtility.ToJson(new DTO { revealed = hasBeenRevealed });
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        var dto = JsonUtility.FromJson<DTO>(json);
        if (dto != null && dto.revealed)
        {
            RevealNow(); // auto-reveal on load if it was already revealed
        }
    }
}
