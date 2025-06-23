using UnityEngine;

public class EyeBlink : MonoBehaviour
{
    [Header("Setup")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public string blinkBlendShapeName = "Blink";

    [Header("Blink Settings")]
    public float blinkDuration = 0.1f;          // How long it takes to close or open eyes
    public Vector2 blinkIntervalRange = new Vector2(2f, 5f); // Time between blinks

    private int blinkIndex = -1;
    private float blinkTimer;
    private bool isBlinking = false;
    private float blinkProgress = 0f;
    private bool closing = true;

    void Start()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer not assigned.");
            enabled = false;
            return;
        }

        // Get the blend shape index for "Blink"
        Mesh mesh = skinnedMeshRenderer.sharedMesh;
        int count = mesh.blendShapeCount;

        for (int i = 0; i < count; i++)
        {
            if (mesh.GetBlendShapeName(i) == blinkBlendShapeName)
            {
                blinkIndex = i;
                break;
            }
        }

        if (blinkIndex == -1)
        {
            Debug.LogError($"Blend shape '{blinkBlendShapeName}' not found.");
            enabled = false;
            return;
        }

        ScheduleNextBlink();
    }

    void Update()
    {
        if (isBlinking)
        {
            float direction = closing ? 1f : -1f;
            blinkProgress += Time.deltaTime / blinkDuration * direction;
            float weight = Mathf.Clamp01(blinkProgress) * 100f;

            skinnedMeshRenderer.SetBlendShapeWeight(blinkIndex, weight);

            if (closing && blinkProgress >= 1f)
            {
                closing = false; // Start opening
            }
            else if (!closing && blinkProgress <= 0f)
            {
                isBlinking = false;
                ScheduleNextBlink();
            }
        }
        else
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                isBlinking = true;
                closing = true;
                blinkProgress = 0f;
            }
        }
    }

    void ScheduleNextBlink()
    {
        blinkTimer = Random.Range(blinkIntervalRange.x, blinkIntervalRange.y);
    }
}
