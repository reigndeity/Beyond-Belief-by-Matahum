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

    public GameObject flameVFXPrefab;
    public int flameMultiplier = 3;

    private List<Transform> colliders = new List<Transform>();
    private List<Transform> flames = new List<Transform>();

    [Header("Pulse Properties")]
    [HideInInspector] public EnemyStats enemyStats;
    public bool canDamage = true;

    private int fireCount;

    public void PrewarmPool()
    {
        fireCount = colliderCount * flameMultiplier;

        // Colliders
        for (int i = 0; i < colliderCount; i++)
        {
            GameObject col = Instantiate(colliderPrefab, transform);
            col.SetActive(false);
            colliders.Add(col.transform);

            CandlePulseDamage pulseDamage = col.GetComponent<CandlePulseDamage>();
            pulseDamage.damage = enemyStats.e_attack * 0.50f;
            pulseDamage.candlePulseExpansion = this;
        }

        // Flames
        for (int i = 0; i < fireCount; i++)
        {
            GameObject flame = Instantiate(flameVFXPrefab, transform);
            flame.SetActive(false);
            flames.Add(flame.transform);
        }
    }

    void ActivatePool()
    {
        canDamage = true;

        // Colliders
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].gameObject.SetActive(true);
            colliders[i].localScale = new Vector3(xScaleStart, 1f, 1f);
        }

        // Flames
        for (int i = 0; i < flames.Count; i++)
        {
            flames[i].gameObject.SetActive(true);
        }
    }

    void DeactivatePool()
    {
        foreach (var col in colliders)
            col.gameObject.SetActive(false);

        foreach (var fire in flames)
            fire.gameObject.SetActive(false);
    }

    public void StartExpanding()
    {
        StartCoroutine(ExpandAndRecycle());
    }
    IEnumerator ExpandAndRecycle()
    {
        for (int i = 0; i < 3; i++) // 3 pulses
        {
            ActivatePool();

            float elapsed = 0f;

            while (elapsed < expandDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / expandDuration);

                float currentRadius = Mathf.Lerp(radiusStart, radiusEnd, t);
                float currentXScale = Mathf.Lerp(xScaleStart, xScaleEnd, t);

                // Colliders
                for (int j = 0; j < colliders.Count; j++)
                {
                    float angle = j * Mathf.PI * 2f / colliderCount;
                    Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * currentRadius;

                    colliders[j].position = transform.position + pos;
                    colliders[j].localScale = new Vector3(currentXScale, 1f, 1f);
                    colliders[j].rotation = Quaternion.LookRotation(colliders[j].position - transform.position);
                }

                // Flames
                for (int k = 0; k < flames.Count; k++)
                {
                    float angle = k * Mathf.PI * 2f / fireCount;
                    Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * currentRadius;

                    flames[k].position = transform.position + pos;
                    flames[k].rotation = Quaternion.LookRotation(flames[k].position - transform.position);

                    // Shrink flames in last 0.5s
                    if (elapsed >= expandDuration - 0.5f)
                    {
                        float shrinkT = Mathf.InverseLerp(expandDuration - 0.5f, expandDuration, elapsed);
                        flames[k].localScale = Vector3.Lerp(Vector3.one, Vector3.zero, shrinkT);
                    }
                    else
                    {
                        flames[k].localScale = Vector3.one; // normal size before shrinking
                    }
                }

                yield return null;
            }

            DeactivatePool(); // instead of Destroy
            yield return new WaitForSeconds(1) ;
        }

        //Destroy(gameObject); // optional cleanup if this effect runs once
    }

}
