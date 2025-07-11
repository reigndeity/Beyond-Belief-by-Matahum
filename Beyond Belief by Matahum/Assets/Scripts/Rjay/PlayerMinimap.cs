using UnityEngine;

public class PlayerMinimap : MonoBehaviour
{
    [Header("Projected View Rotation Settings")]
    public Transform cameraTransform;
    public bool debug = false;

    private void Update()
    {
        ProjectionRotation();
    }
    private void ProjectionRotation()
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
}
