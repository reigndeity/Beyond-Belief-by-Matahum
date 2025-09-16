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

    // Persisted list of revealed area IDs
    [SerializeField] private List<string> revealedAreaIds = new List<string>();

    [Header("Transitions")]
    [SerializeField] UI_TransitionController transition;  // assign in Inspector (Fade + Loading)

    [Header("Teleport Settings")]
    [SerializeField] private float minTeleportDistance = 10f; // distance threshold to block teleport

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
                    foreach (var p in area.points)
                        if (p != null) p.minimapFog = minimapFog;
                }
            }
        }
    }

    void Start()
    {
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

        bool canTeleport = true;
        string buttonText = "Teleport";

        var interactable = teleporter.GetComponent<TeleportInteractable>();
        if (interactable != null && !interactable.IsUnlocked())
        {
            canTeleport = false;
            buttonText = "Locked";
        }
        else
        {
            // Check if player is already near teleporter
            Vector3 playerPos = playerTransform.position;
            Vector3 tpPos = (teleporter.teleportTarget != null ?
                             teleporter.teleportTarget.position :
                             teleporter.transform.position);

            if (Vector3.Distance(playerPos, tpPos) < minTeleportDistance)
            {
                canTeleport = false;
                buttonText = "You are already nearby.";
            }
        }

        if (teleportButton != null)
        {
            teleportButton.interactable = canTeleport;
            if (teleportButtonLabel != null)
                teleportButtonLabel.text = buttonText;
        }

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
        if (currentTeleporter == null || playerTransform == null) return;

        var interactable = currentTeleporter.GetComponent<TeleportInteractable>();
        if (interactable != null && !interactable.IsUnlocked())
        {
            Debug.Log("Teleport is locked — cannot teleport.");
            return;
        }

        // Destination is the teleporter's target (full XYZ, no Y override)
        Vector3 tpPos = (currentTeleporter.teleportTarget != null ?
                         currentTeleporter.teleportTarget.position :
                         currentTeleporter.transform.position);

        if (Vector3.Distance(playerTransform.position, tpPos) < minTeleportDistance)
        {
            Debug.Log("Player is already at this teleporter — teleport cancelled.");
            return;
        }

        Vector3 targetPos = tpPos;

        if (transition != null)
        {
            StartCoroutine(transition.TeleportTransition(() =>
            {
                playerTransform.position = targetPos;
            }));
        }
        else
        {
            playerTransform.position = targetPos;
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

        if (!revealedAreaIds.Contains(id))
            revealedAreaIds.Add(id);

        foreach (var p in area.points)
        {
            if (p != null)
            {
                p.minimapFog = minimapFog;
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

    public void SyncRevealVisuals()
    {
        if (revealedAreaIds == null || revealedAreaIds.Count == 0) return;
        foreach (var id in revealedAreaIds)
            RevealArea(id);
    }

    public void SetRevealedAreas(List<string> ids)
    {
        revealedAreaIds = ids ?? new List<string>();
    }

    public List<string> GetRevealedAreaIds()
    {
        return new List<string>(revealedAreaIds);
    }

    public void CloseTeleporterUI()
    {
        if (teleporterPanel) teleporterPanel.SetActive(false);
        HideSelection();
        currentTeleporter = null;

        if (teleportButton) teleportButton.interactable = false;
        if (teleportButtonLabel) teleportButtonLabel.text = "Teleport";
    }
}
