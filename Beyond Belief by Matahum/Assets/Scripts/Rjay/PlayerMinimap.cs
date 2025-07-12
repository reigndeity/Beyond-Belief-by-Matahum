using System.Collections;
using MTAssets.EasyMinimapSystem;
using UnityEngine;

public class PlayerMinimap : MonoBehaviour
{
    private PlayerInput m_playerInput;
    [Header("Projected View Rotation Settings")]
    [SerializeField] Transform cameraTransform;
    public MinimapItem projectedViewIcon;
    public bool debug = false;

    [Header("Full Screen Map Settings")]
    [SerializeField] GameObject fullscreenMap;
    [SerializeField] MinimapCamera minimapCamera;
    public float zoomSpeed = 100f;
    public float minFOV = 50f;
    public float maxFOV = 175f;
    private bool isMapOpen = false;

    [Header("Panning and Centering Player Map Settings")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Vector2 xClampRange = new Vector2(-500, 500);
    [SerializeField] Vector2 zClampRange = new Vector2(-500, 500);
    [SerializeField] float smoothPanDuration = 0.5f;
    [SerializeField] float targetZoomFOV = 80f;
    private Coroutine recenterRoutine;

    void Start()
    {
        m_playerInput = GetComponentInParent<PlayerInput>();
    }
    public void ProjectionRotation()
    {
        if (cameraTransform == null) return;

        // Flatten the camera forward vector to XZ plane
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        if (debug) Debug.DrawRay(transform.position, camForward * 2f, Color.yellow);

        // Rotate this indicator to face the flattened direction
        if (camForward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(camForward);
    }

    public void HandleMapToggle()
    {
        if (Input.GetKeyDown(m_playerInput.mapKey))
        {
            isMapOpen = !isMapOpen;
            fullscreenMap.SetActive(isMapOpen);

            // Toggle cursor
            Cursor.lockState = isMapOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMapOpen;

            // ✅ Auto-center with zoom when opening the map
            if (isMapOpen)
            {
                CenterMapOnPlayerWithZoom();
            }
        }
    }

    public void ZoomControl()
    {
        if (!isMapOpen) return; // Only allow zoom when map is open

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newFOV = minimapCamera.fieldOfView - scroll * zoomSpeed;
            newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);
            minimapCamera.fieldOfView = newFOV;
        }
    }

    public bool IsMapOpen()
    {
        return isMapOpen;
    }

    public void OnDragInMap(Vector3 startWorldPos, Vector3 currentWorldPos)
    {
        Vector3 delta = (currentWorldPos - startWorldPos) * -1f;
        Vector3 newPos = minimapCamera.transform.position + delta * 10f * Time.deltaTime;

        // Clamp X and Z
        newPos.x = Mathf.Clamp(newPos.x, xClampRange.x, xClampRange.y);
        newPos.z = Mathf.Clamp(newPos.z, zClampRange.x, zClampRange.y);

        minimapCamera.transform.position = newPos;
    }

    public void CenterMapOnPlayer()
    {
        if (playerTransform != null)
            minimapCamera.transform.position = new Vector3(playerTransform.position.x, minimapCamera.transform.position.y, playerTransform.position.z);
    }
    public void CenterMapOnPlayerWithZoom()
    {
        if (recenterRoutine != null)
            StopCoroutine(recenterRoutine);

        recenterRoutine = StartCoroutine(SmoothCenterOnPlayer());
    }

    private IEnumerator SmoothCenterOnPlayer()
    {
        Vector3 startPos = minimapCamera.transform.position;
        float startFOV = minimapCamera.fieldOfView;

        Vector3 targetPos = new Vector3(
            playerTransform.position.x,
            startPos.y, // maintain Y (height)
            playerTransform.position.z
        );

        float targetFOV = Mathf.Min(startFOV, targetZoomFOV); // only zoom in if zoomed out

        float elapsed = 0f;
        while (elapsed < smoothPanDuration)
        {
            float t = elapsed / smoothPanDuration;
            minimapCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            minimapCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final snap to make sure it's exact
        minimapCamera.transform.position = targetPos;
        minimapCamera.fieldOfView = targetFOV;
        recenterRoutine = null;
    }

}
