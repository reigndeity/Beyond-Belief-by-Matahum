using MTAssets.EasyMinimapSystem;
using UnityEngine;

public class FogRevealPoint : MonoBehaviour
{
    public float revealRadius = 5f;
    [HideInInspector] public MinimapFog minimapFog; // Optional, or pass externally

    public void Reveal()
    {
        if (minimapFog == null) return;
        minimapFog.RevealThisWorldPosition(transform.position, (int)revealRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.5f); // light blue
        Gizmos.DrawWireSphere(transform.position, revealRadius);
    }
}
