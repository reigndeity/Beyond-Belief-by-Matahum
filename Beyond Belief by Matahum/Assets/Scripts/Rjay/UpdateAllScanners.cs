using UnityEngine;
using MTAssets.EasyMinimapSystem;

public class UpdateAllScanners : MonoBehaviour
{
    [ContextMenu("Update All Scanners")]
    public void UpdateAll()
    {
        MinimapScanner[] scanners = FindObjectsOfType<MinimapScanner>();
        foreach (var scanner in scanners)
        {
            scanner.DoScanInThisAreaOfComponentAndShowOnMinimap();
        }

        Debug.Log($"Updated {scanners.Length} minimap scanners!");
    }
}
