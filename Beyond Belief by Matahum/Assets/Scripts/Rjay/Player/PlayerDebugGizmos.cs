using UnityEngine;

[ExecuteAlways]
public class PlayerDebugGizmos : MonoBehaviour
{
    private CharacterController m_characterController;

    void OnDrawGizmos()
    {
        if (m_characterController == null)
            m_characterController = GetComponent<CharacterController>();

        if (m_characterController == null)
            return;

        Vector3 centerWorldPos = transform.position + m_characterController.center;
        Vector3 bottom = centerWorldPos;
        bottom.y -= m_characterController.height / 2f;
        bottom.y += m_characterController.radius;

        // Draw a red sphere at the grounded position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bottom, 0.05f);

        // Optional: draw a wire capsule of the controller
        Gizmos.color = Color.cyan;
        DrawWireCapsule(centerWorldPos, m_characterController.height, m_characterController.radius);
    }

    void DrawWireCapsule(Vector3 center, float height, float radius)
    {
        float cylinderHeight = height - 2 * radius;
        Vector3 up = Vector3.up * cylinderHeight / 2f;

        // Draw center cylinder
        Gizmos.DrawWireSphere(center + up, radius);
        Gizmos.DrawWireSphere(center - up, radius);

        // Approximate side lines
        Gizmos.DrawLine(center + up + Vector3.forward * radius, center - up + Vector3.forward * radius);
        Gizmos.DrawLine(center + up - Vector3.forward * radius, center - up - Vector3.forward * radius);
        Gizmos.DrawLine(center + up + Vector3.right * radius, center - up + Vector3.right * radius);
        Gizmos.DrawLine(center + up - Vector3.right * radius, center - up - Vector3.right * radius);
    }
}
