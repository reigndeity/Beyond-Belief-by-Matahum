using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureObject : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject fracturedObject;
    public GameObject explosionVFX;
    public float explosionMinForce = 5;
    public float explosionMaxForce = 100;
    public float explosionForceRadius = 10;

    [Header("Debris Shrink Settings")]
    public float shrinkDelay = 2f;       // wait before starting shrink
    public float shrinkDuration = 1.5f;  // how long it takes to shrink to 0

    private GameObject fractObj;

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }*/

    public void Explode()
    {
        if (originalObject != null)
        {
            originalObject.SetActive(false);
        }

        if (fracturedObject != null)
        {
            fractObj = Instantiate(fracturedObject, originalObject.transform.position, Quaternion.identity, originalObject.transform.parent);

            foreach (Transform t in fractObj.transform)
            {
                var rb = t.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(
                        Random.Range(explosionMinForce, explosionMaxForce),
                        originalObject.transform.position,
                        explosionForceRadius
                    );

                // start shrink coroutine
                StartCoroutine(Shrink(t, shrinkDelay, shrinkDuration));
            }

            Destroy(fractObj, 7);

            if (explosionVFX != null)
            {
                GameObject exploVFX = Instantiate(explosionVFX, originalObject.transform.position, Quaternion.identity, originalObject.transform.parent);
                Destroy(exploVFX, 7);
            }
        }
    }

    public void Reset()
    {
        if (fractObj != null)
            Destroy(fractObj);

        originalObject.SetActive(true);
    }

    IEnumerator Shrink(Transform t, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (t == null) yield break;

        Vector3 startScale = t.localScale;
        float elapsed = 0f;

        while (elapsed < duration && t != null)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            t.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);

            yield return null; // wait until next frame
        }

        if (t != null)
            Destroy(t.gameObject);
    }
}
