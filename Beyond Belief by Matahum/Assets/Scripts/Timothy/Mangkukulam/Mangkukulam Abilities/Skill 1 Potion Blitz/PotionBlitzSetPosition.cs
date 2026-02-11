using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class PotionBlitzSetPosition : MonoBehaviour
{
    public int posCount = 4;
    public float radius = 25f;
    public GameObject posObj;
    public Transform[] posToFly;
    public void SpawnFlyPosition()
    {
        posToFly = new Transform[posCount];

        for (int i = 0; i < posCount; i++)
        {
            float angle = i * Mathf.PI * 2f / posCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            // Base position around the caster
            Vector3 basePos = transform.position + offset;

            // Try to find the nearest point on the NavMesh
            if (NavMesh.SamplePosition(basePos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                // Spawn at the navmesh location
                Vector3 navMeshOffset = new Vector3(0, 0.11f, 0);
                posToFly[i] = Instantiate(posObj, hit.position - navMeshOffset, Quaternion.identity, transform).transform;
            }
            else
            {
                Debug.LogWarning($"No NavMesh found near {basePos}, skipping position {i}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Optional: show needle spawn points as little spheres
        if (posCount > 0)
        {
            for (int i = 0; i < posCount; i++)
            {
                float angle = i * Mathf.PI * 2f / posCount;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                Gizmos.DrawSphere(transform.position + pos, 0.1f);
            }
        }
    }
}
