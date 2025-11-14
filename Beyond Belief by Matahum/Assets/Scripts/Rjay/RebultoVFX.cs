using UnityEngine;
using System.Collections;

public class RebultoVFX : MonoBehaviour
{
    [Header("Target Renderers")]
    [SerializeField] private Renderer[] targetRenderers;

    [Header("Emission Settings")]
    [SerializeField] private float startIntensity = -10f;
    [SerializeField] private float overshootIntensity = 5f;
    [SerializeField] private float overshootHoldDuration = 0.25f;
    [SerializeField] private float endIntensity = 2f;
    [SerializeField] private float duration = 1.5f;

    [Header("Particle Control Settings")]
    [SerializeField] private ParticleSystem rebultoParticle;
    [SerializeField] private ParticleSystem unlockedRebultoParticle;

    private Material[] mats;
    private Color[] baseColors;
    private Coroutine vfxRoutine;

    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    

    void Awake()
    {
        mats = new Material[targetRenderers.Length];
        baseColors = new Color[targetRenderers.Length];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            mats[i] = targetRenderers[i].material;

            // Force emission to stay enabled (URP)
            mats[i].EnableKeyword("_EMISSION");
            mats[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            Color stored = mats[i].GetColor(EmissionID);
            float intensity = stored.maxColorComponent;
            if (intensity <= 0f) intensity = 1f;

            baseColors[i] = stored / intensity;
        }
    }

    public void InitialUnlockVFX()
    {
        if (vfxRoutine != null)
            StopCoroutine(vfxRoutine);

        vfxRoutine = StartCoroutine(AnimateEmission());
        rebultoParticle.Play();
    }
    public void AlreadyUnlockedVFX()
    {
        if (vfxRoutine != null)
            StopCoroutine(vfxRoutine);

        vfxRoutine = StartCoroutine(AnimateEmission());
        unlockedRebultoParticle.Play();
    }

    private IEnumerator AnimateEmission()
    {
        float t = 0f;

        // Phase 1: start → overshoot
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            float intensity = Mathf.Lerp(startIntensity, overshootIntensity, lerp);
            SetEmission(intensity);

            yield return null;
        }

        // Phase 2: hold
        SetEmission(overshootIntensity);
        yield return new WaitForSeconds(overshootHoldDuration);

        // Phase 3: overshoot → end
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            float intensity = Mathf.Lerp(overshootIntensity, endIntensity, lerp);
            SetEmission(intensity);

            yield return null;
        }

        SetEmission(endIntensity);
    }

    private void SetEmission(float intensity)
    {
        float gamma = Mathf.LinearToGammaSpace(intensity);

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor(EmissionID, baseColors[i] * gamma);
        }
    }
}
