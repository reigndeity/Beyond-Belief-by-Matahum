using UnityEngine;

public enum ArchiveTrackerMode
{
    OnSight,
    OnTriggerEnter,
    OnInteract
}

public class BB_ArchiveTracker : MonoBehaviour
{
    public BB_ArchiveSO archiveData;
    private bool hasBeenDiscovered = false;
    private Camera detectionCamera; // assign this to your player camera
    public float checkInterval = 0.3f;
    public float detectionRange = 30f; // Maximum range in units
    public CanvasGroup canvasGroup;

    public ArchiveTrackerMode mode;

    private void Start()
    {
        if(mode == ArchiveTrackerMode.OnSight) 
        {
            if (detectionCamera == null)
                detectionCamera = Camera.main;

            InvokeRepeating(nameof(CheckIfVisibleToPlayer), 0f, checkInterval);
        }
    }

    void CheckIfVisibleToPlayer()
    {
        if (hasBeenDiscovered || detectionCamera == null) return;

        float distance = Vector3.Distance(detectionCamera.transform.position, transform.position);
        if (distance > detectionRange) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(detectionCamera);
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        if (!GeometryUtility.TestPlanesAABB(planes, col.bounds)) return;

        // Line of sight check
        Vector3 dirToObject = (col.bounds.center - detectionCamera.transform.position).normalized;
        Ray ray = new Ray(detectionCamera.transform.position, dirToObject);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
        {
            if (hit.collider == col)
            {
                DiscoveredArchive();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mode == ArchiveTrackerMode.OnTriggerEnter) return;
        if (hasBeenDiscovered) return;

        if (other.gameObject.CompareTag("Player"))
        {
            DiscoveredArchive();
        }
    }

    public void DiscoveredArchive()
    {
        BB_ArchiveManager.instance.UpdateArchive(archiveData);
        hasBeenDiscovered = true;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
