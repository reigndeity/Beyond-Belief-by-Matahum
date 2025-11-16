using UnityEngine;

public class GroundFootstepChanger : MonoBehaviour
{
    public int typeOfGround;

    // Static since all zones modify the same global groundType.
    private static int grassZoneCount = 0;
    private static int currentGroundType = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Entering this ground type
        if (typeOfGround != 0)
        {
            grassZoneCount++;
            AudioManager.instance.groundType = typeOfGround;
            currentGroundType = typeOfGround;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Only decrease count if this was a non-default zone
        if (typeOfGround != 0)
        {
            grassZoneCount--;
            grassZoneCount = Mathf.Max(grassZoneCount, 0);
        }

        // If no more grass zones are active, return to default
        if (grassZoneCount == 0)
        {
            AudioManager.instance.groundType = 0;
            currentGroundType = 0;
        }
        else
        {
            // Still inside other zones → keep the last known ground type
            AudioManager.instance.groundType = currentGroundType;
        }
    }
}
