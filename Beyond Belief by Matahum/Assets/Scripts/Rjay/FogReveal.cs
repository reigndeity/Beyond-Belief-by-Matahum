using MTAssets.EasyMinimapSystem;
using UnityEngine;

public class FogReveal : MonoBehaviour
{
    public MinimapFog minimapFog; // Drag your MinimapFogRoot here
    public float revealRadius = 150f; // Adjust for how much of the map it reveals
    private bool hasBeenActivated = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenActivated) return;
        if (other.CompareTag("Player"))
        {
            minimapFog.RevealThisWorldPosition(transform.position, (int)revealRadius);
            hasBeenActivated = true;
        }
    }
}
