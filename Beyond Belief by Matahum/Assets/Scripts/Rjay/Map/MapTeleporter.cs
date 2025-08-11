using UnityEngine;
using MTAssets.EasyMinimapSystem;

public class MapTeleporter : MonoBehaviour
{
    [Header("Teleporter Info")]
    public string location;
    public string description;

    [Header("Teleport Offset")]
    public Transform teleportTarget;
}
