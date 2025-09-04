using UnityEngine;

public class FogAreaReveal : MonoBehaviour
{
    [Tooltip("IDs of map areas to reveal when triggered.")]
    [SerializeField] private string[] revealAreaIds;

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
            Debug.Log($"[FogAreaReveal] Revealed {revealAreaIds.Length} areas from {gameObject.name}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            RevealNow();
        }
    }
}
