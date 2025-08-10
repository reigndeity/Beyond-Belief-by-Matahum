using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MTAssets.EasyMinimapSystem;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Player Reference")]
    [SerializeField] Transform playerTransform;

    [Header("UI")]
    public GameObject teleporterPanel;
    [SerializeField] TextMeshProUGUI teleporterLocationTxt;
    [SerializeField] TextMeshProUGUI teleporterDescriptionTxt;
    [SerializeField] Button teleportButton;
    [SerializeField] TextMeshProUGUI teleportButtonLabel;

    [Header("Selection Ring")]
    [SerializeField] MinimapItem circleSelect;
    [SerializeField] Vector3 startSize = new Vector3(0.01f, 0f, 0.01f);
    [SerializeField] Vector3 endSize = new Vector3(15f, 0f, 15f);
    [SerializeField] float tweenDuration = 0.15f;
    [SerializeField] float zOffset = -1.2f;

    private Coroutine circleTween;
    private MapTeleporter currentTeleporter;

    [Header("Map Fog")]
    [SerializeField] MinimapFog minimapFog;

    // NEW: Persisted list of revealed area IDs
    [SerializeField] private List<string> revealedAreaIds = new List<string>();

    [System.Serializable]
    public class MapArea
    {
        [Tooltip("Unique ID used by TeleportInteractable.revealAreaIds")]
        public string id;

        [Header("Auto Gather")]
        [Tooltip("Parent object containing this area's FogRevealPoints")]
        public Transform areaRoot;

        [Tooltip("If true, auto-gather points from areaRoot on Awake")]
        public bool autoGatherOnAwake = true;

        [Header("Collected Points (auto-populated)")]
        public FogRevealPoint[] points;

        /// <summary>
        /// Collect all FogRevealPoints under areaRoot (including inactive children)
        /// and ensure their minimapFog reference is set.
        /// </summary>
        public void GatherPoints(MinimapFog fog)
        {
            if (areaRoot == null)
            {
                Debug.LogWarning($"MapArea '{id}' has no areaRoot assigned.");
                points = null;
                return;
            }

            points = areaRoot.GetComponentsInChildren<FogRevealPoint>(true);
            if (points == null) return;

            foreach (var p in points)
            {
                if (p != null) p.minimapFog = fog;
            }
        }
    }

    [Tooltip("Register all revealable areas here. Use areaRoot to auto-gather points.")]
    [SerializeField] private MapArea[] areas;

    void Awake()
    {
        instance = this;
        if (circleSelect) circleSelect.sizeOnMinimap = startSize;

        // Auto-gather FogRevealPoints per area (no more manual dragging)
        if (areas != null)
        {
            foreach (var area in areas)
            {
                if (area == null) continue;

                if (area.autoGatherOnAwake)
                {
                    area.GatherPoints(minimapFog);
                }
                else if (area.points != null)
                {
                    // If not auto-gathering, still make sure fog reference is correct
                    foreach (var p in area.points)
                        if (p != null) p.minimapFog = minimapFog;
                }
            }
        }
    }

    void Start()
    {
        // In case revealedAreaIds got loaded by ES3AutoSave or set externally before Start,
        // sync the visual fog to match the list.
        SyncRevealVisuals();
    }

    [ContextMenu("Gather All Areas Now")]
    void GatherAllAreasNow()
    {
        if (areas == null) return;

        foreach (var area in areas)
            area?.GatherPoints(minimapFog);

        Debug.Log("MapManager: Gathered FogRevealPoints for all areas.");
    }

    // Called by your minimap click handler
    public void OnClickInMinimapRendererArea(Vector3 worldClick, MinimapItem clickedItem)
    {
        if (clickedItem != null && clickedItem.CompareTag("Teleporter"))
        {
            MapTeleporter teleporter = clickedItem.GetComponent<MapTeleporter>();
            if (teleporter != null)
            {
                ShowTeleporter(teleporter, clickedItem.transform.position);
            }
        }
    }

    public void ShowTeleporter(MapTeleporter teleporter, Vector3 worldPos)
    {
        currentTeleporter = teleporter;

        teleporterPanel.SetActive(true);
        teleporterLocationTxt.text = teleporter.location;
        teleporterDescriptionTxt.text = teleporter.description;

        // Disable button + change label if linked TeleportInteractable is locked
        bool canTeleport = true;
        var interactable = teleporter.GetComponent<TeleportInteractable>();
        if (interactable != null && !interactable.IsUnlocked())
            canTeleport = false;

        if (teleportButton != null)
        {
            teleportButton.interactable = canTeleport;
            if (teleportButtonLabel != null)
                teleportButtonLabel.text = canTeleport ? "Teleport" : "Locked";
        }

        // Selection ring
        if (circleSelect)
        {
            circleSelect.transform.position = worldPos + new Vector3(0f, 0f, zOffset);
            if (circleTween != null) StopCoroutine(circleTween);
            circleTween = StartCoroutine(TweenCircleSize(circleSelect.sizeOnMinimap, endSize));
        }
    }

    public void HideSelection()
    {
        if (circleTween != null) StopCoroutine(circleTween);
        if (circleSelect) circleTween = StartCoroutine(TweenCircleSize(circleSelect.sizeOnMinimap, startSize));
    }

    public void TeleportPlayerToSelected()
    {
        if (currentTeleporter != null && playerTransform != null)
        {
            // Safety — prevent teleport if still locked
            var interactable = currentTeleporter.GetComponent<TeleportInteractable>();
            if (interactable != null && !interactable.IsUnlocked())
            {
                Debug.Log("Teleport is locked — cannot teleport.");
                return;
            }

            Vector3 targetPos = (currentTeleporter.teleportTarget != null)
                ? currentTeleporter.teleportTarget.position
                : currentTeleporter.transform.position;

            // Maintain player Y height to avoid clipping into terrain
            targetPos.y = playerTransform.position.y;

            playerTransform.position = targetPos;

            HideSelection();
            teleporterPanel.SetActive(false);
        }
    }

    IEnumerator TweenCircleSize(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < tweenDuration)
        {
            float t = elapsed / tweenDuration;
            circleSelect.sizeOnMinimap = Vector3.Lerp(from, to, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        circleSelect.sizeOnMinimap = to;
    }

    // --------- FOG REVEAL API ---------

    public void RevealArea(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        var area = System.Array.Find(areas, a => a != null && a.id == id);
        if (area == null)
        {
            Debug.LogWarning($"MapManager: area '{id}' not found.");
            return;
        }

        if (area.points == null || area.points.Length == 0)
        {
            Debug.LogWarning($"MapManager: area '{id}' has no points. Did you set areaRoot or run Gather All Areas Now?");
            return;
        }

        // Track persistence
        if (!revealedAreaIds.Contains(id))
            revealedAreaIds.Add(id);

        foreach (var p in area.points)
        {
            if (p != null)
            {
                p.minimapFog = minimapFog; // ensure fog ref is correct
                p.Reveal();
            }
        }
    }

    public void RevealAreas(IEnumerable<string> ids)
    {
        if (ids == null) return;
        foreach (var id in ids)
            RevealArea(id);
    }

    // --------- NEW: persistence helpers ---------

    /// <summary>Reveals all areas already in the revealedAreaIds list.</summary>
    public void SyncRevealVisuals()
    {
        if (revealedAreaIds == null || revealedAreaIds.Count == 0) return;

        // Re-run reveal to ensure visuals match (Reveal() is effectively idempotent)
        foreach (var id in revealedAreaIds)
            RevealArea(id);
    }

    /// <summary>Replace the revealed list (called by load) and apply visuals.</summary>
    public void SetRevealedAreas(List<string> ids)
    {
        revealedAreaIds = ids ?? new List<string>();
    }

    /// <summary>Get a copy of revealed IDs for saving.</summary>
    public List<string> GetRevealedAreaIds()
    {
        return new List<string>(revealedAreaIds);
    }

    // --------- UI Close Helper ---------

    public void CloseTeleporterUI()
    {
        if (teleporterPanel) teleporterPanel.SetActive(false);
        HideSelection();
        currentTeleporter = null;

        if (teleportButton) teleportButton.interactable = false;
        if (teleportButtonLabel) teleportButtonLabel.text = "Teleport";
    }
}
