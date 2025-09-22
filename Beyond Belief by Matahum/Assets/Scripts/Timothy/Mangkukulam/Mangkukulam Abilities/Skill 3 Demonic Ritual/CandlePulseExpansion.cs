using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CandlePulseExpansion : MonoBehaviour
{
    [Header("Setup")]
    public GameObject colliderPrefab;   // Prefab with BoxCollider (trigger)
    public int colliderCount = 36;      // How many colliders in the ring
    public float radiusStart = 1f;      // Starting radius
    public float radiusEnd = 10f;       // Ending radius after expand
    public float expandDuration = 2f;   // Expansion time in seconds
    public float xScaleStart = 0.2f;    // Initial width of colliders
    public float xScaleEnd = 1.5f;      // Final width of colliders

    private List<Transform> colliders = new List<Transform>();

    [Header("Pulse Properties")]
    [HideInInspector] public EnemyStats enemyStats;
    public bool canDamage = true;

    void Start()
    {
        StartCoroutine(ExpandAndDestroy());
    }

    void SpawnColliders()
    {
        canDamage = true;
        for (int i = 0; i < colliderCount; i++)
        {
            float angle = i * Mathf.PI * 2f / colliderCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radiusStart;

            GameObject col = Instantiate(colliderPrefab, transform.position + pos, Quaternion.identity, transform);

            // Rotate so each collider faces outward
            col.transform.rotation = Quaternion.LookRotation(col.transform.position - transform.position);

            // Set initial scale
            col.transform.localScale = new Vector3(xScaleStart, 1f, 1f);

            colliders.Add(col.transform);

            CandlePulseDamage pulseDamage = col.GetComponent<CandlePulseDamage>();
            pulseDamage.damage = enemyStats.e_attack * 0.50f;
            pulseDamage.candlePulseExpansion = this;
        }
    }

    IEnumerator ExpandAndDestroy()
    {
        for (int i = 0; i < 5; i++) // 5 pulses
        {
            colliders.Clear();
            SpawnColliders();

            float elapsed = 0f;

            while (elapsed < expandDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / expandDuration);

                float currentRadius = Mathf.Lerp(radiusStart, radiusEnd, t);
                float currentXScale = Mathf.Lerp(xScaleStart, xScaleEnd, t);

                for (int j = 0; j < colliders.Count; j++)
                {
                    float angle = j * Mathf.PI * 2f / colliderCount;
                    Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * currentRadius;

                    colliders[j].position = transform.position + pos;
                    colliders[j].localScale = new Vector3(currentXScale, 1f, 1f);
                    colliders[j].rotation = Quaternion.LookRotation(colliders[j].position - transform.position);
                }

                yield return null;
            }

            // Destroy after each pulse
            foreach (var col in colliders)
            {
                if (col != null) Destroy(col.gameObject);
            }
        }

        Destroy(gameObject);
    }

}