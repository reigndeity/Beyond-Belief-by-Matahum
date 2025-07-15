using UnityEngine;
using TMPro;
using MTAssets.EasyMinimapSystem;
using System.Collections;

public class MapTeleportManager : MonoBehaviour
{
    public static MapTeleportManager instance;

    [Header("Player Reference")]
    [SerializeField] Transform playerTransform; // Assign your player here

    [Header("UI")]
    [SerializeField] GameObject teleporterPanel;
    [SerializeField] TextMeshProUGUI teleporterLocationTxt;
    [SerializeField] TextMeshProUGUI teleporterDescriptionTxt;

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
    public FogRevealPoint[] lewenriVillage;

    void Awake()
    {
        instance = this;
        circleSelect.sizeOnMinimap = startSize;

        foreach (var point in lewenriVillage)
        {
            point.minimapFog = minimapFog;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RevealArea();
        }
    }

    public void RevealArea()
    {
        foreach (var point in lewenriVillage)
        {
            point.Reveal();
        }
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

        circleSelect.transform.position = worldPos + new Vector3(0f, 0f, zOffset);
        if (circleTween != null) StopCoroutine(circleTween);
        circleTween = StartCoroutine(TweenCircleSize(circleSelect.sizeOnMinimap, endSize));
    }

    public void HideSelection()
    {
        if (circleTween != null) StopCoroutine(circleTween);
        circleTween = StartCoroutine(TweenCircleSize(circleSelect.sizeOnMinimap, startSize));
    }

    public void TeleportPlayerToSelected()
    {
        if (currentTeleporter != null && playerTransform != null)
        {
            Vector3 targetPos = (currentTeleporter.teleportTarget != null)
            ? currentTeleporter.teleportTarget.position
            : currentTeleporter.transform.position;

            // Optional: Maintain player Y height to avoid clipping into terrain
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
}
