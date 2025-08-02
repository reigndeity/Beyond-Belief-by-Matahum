using UnityEngine;

public class BB_ArchiveTracker : MonoBehaviour
{
    public string archiveID;
    private bool hasBeenDiscovered = false;
    public Camera detectionCamera; // assign this to your player camera
    public float checkInterval = 0.3f;

    private void Start()
    {
        if (detectionCamera == null)
            detectionCamera = Camera.main;

        InvokeRepeating(nameof(CheckIfVisibleToPlayer), 0f, checkInterval);
    }

    void CheckIfVisibleToPlayer()
    {
        if (hasBeenDiscovered || detectionCamera == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(detectionCamera);
        Collider col = GetComponent<Collider>();

        if (col == null) return;

        if (GeometryUtility.TestPlanesAABB(planes, col.bounds))
        {
            BB_ArchiveManager.instance.UpdateArchive(archiveID);
            hasBeenDiscovered = true;
            Debug.Log(archiveID + " has been discovered");
        }
    }
}
