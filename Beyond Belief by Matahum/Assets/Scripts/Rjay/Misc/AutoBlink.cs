using UnityEngine;
using System.Collections;

public class AutoBlink : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // assign in inspector
    public string blinkShapeName = "Blink";
    public float blinkIntervalMin = 3f;
    public float blinkIntervalMax = 6f;
    public float blinkSpeed = 10f; // higher = faster blink
    public float closedTime = 0.1f;

    private int blinkShapeIndex;

    void Start()
    {
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        blinkShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blinkShapeName);

        if (blinkShapeIndex < 0)
        {
            Debug.LogError($"Blend shape '{blinkShapeName}' not found!");
            enabled = false;
            return;
        }

        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkIntervalMin, blinkIntervalMax));

            // close eyes
            float weight = 0f;
            while (weight < 100f)
            {
                weight += Time.deltaTime * blinkSpeed * 100f;
                skinnedMeshRenderer.SetBlendShapeWeight(blinkShapeIndex, Mathf.Clamp(weight, 0f, 100f));
                yield return null;
            }

            yield return new WaitForSeconds(closedTime);

            // open eyes
            while (weight > 0f)
            {
                weight -= Time.deltaTime * blinkSpeed * 100f;
                skinnedMeshRenderer.SetBlendShapeWeight(blinkShapeIndex, Mathf.Clamp(weight, 0f, 100f));
                yield return null;
            }
        }
    }
}
