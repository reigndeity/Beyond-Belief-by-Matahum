using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

public class PlayerWeapon : MonoBehaviour
{
    private PlayerStats m_playerStats;
    public Collider weaponCollider;
    public float m_scalingAmount;
    [Header("Hit Cooldown")]
    private Dictionary<Collider, float> hitTimestamps = new Dictionary<Collider, float>();
    public float hitCooldown = 0.3f; // Time between valid hits to the same enemy
    [Header("Sword Trail")]
    public ParticleSystem swordTrail;
    public VisualEffect lastAttackEffect;
    [Header("Hit Impact")]
    public GameObject hitImpactPrefab;

    [Header("Weapon Visibility")]
    public bool isWeaponShowing;

    [Header("Particle To Play when Dissolving Weapon")]
    public ParticleSystem dissolveParticleSystem;
    [Header("Ultimate Blade Renderer")]
    [SerializeField] SkinnedMeshRenderer bladeRenderer;
    [SerializeField] Material defaultBladeMaterial;
    [SerializeField] Material ultimateBladeMaterial;
    private Material runtimeUltimateBladeMat;
    private Coroutine ultimateBladeRoutine;

    [Header("Materials to Dissolve (shared shader)")]
    public Renderer[] renderers;

    private List<Material> materials = new List<Material>();
    private Coroutine currentRoutine;

    private const string DISSOLVE_PROPERTY = "_Dissolve";

    void Awake()
    {
        InitializeMaterials();
    }

    void Start()
    {
        m_playerStats = GetComponentInParent<PlayerStats>();
        weaponCollider = GetComponent<Collider>();
        DissolveWeapon(0f);
        lastAttackEffect.Stop();
    }
    private void Update()
    {
        ClearHitStamps();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        // Check hit cooldown
        if (hitTimestamps.TryGetValue(other, out float lastHitTime))
        {
            if (Time.time - lastHitTime < hitCooldown)
            {
                return; // Recently hit, ignore
            }
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            float damage = m_playerStats.p_attack * m_scalingAmount;
            damageable.TakeDamage(damage);

            MutyaNgLinta_Lifesteal lifesteal = m_playerStats.GetComponent<MutyaNgLinta_Lifesteal>();
            if (lifesteal != null && lifesteal.enabled)
            {
                lifesteal.OnDamageDealt(damage);
            }

            hitTimestamps[other] = Time.time; // Record this hit
            SpawnHitImpact(other.ClosestPoint(transform.position));
        }
    }

    void ClearHitStamps()
    {
        var keysToRemove = new List<Collider>();
        foreach (var kvp in hitTimestamps)
        {
            if (Time.time - kvp.Value > hitCooldown * 2f)
                keysToRemove.Add(kvp.Key);
        }
        foreach (var key in keysToRemove)
            hitTimestamps.Remove(key);
    }

    void InitializeMaterials()
    {
        foreach (Renderer r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty(DISSOLVE_PROPERTY))
                {
                    materials.Add(mat);
                }
            }
        }
    }

    public void DissolveWeapon(float duration)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDissolve(GetCurrentDissolveValue(), 1f, duration));
        isWeaponShowing = false;
    }

    public void UndissolveWeapon(float duration)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDissolve(GetCurrentDissolveValue(), 0f, duration));
        isWeaponShowing = true;
    }

    private float GetCurrentDissolveValue()
    {
        if (materials.Count > 0)
            return materials[0].GetFloat(DISSOLVE_PROPERTY);
        return 0f;
    }

    private IEnumerator AnimateDissolve(float from, float to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float value = Mathf.Lerp(from, to, t);
            foreach (var mat in materials)
                mat.SetFloat(DISSOLVE_PROPERTY, value);

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in materials)
            mat.SetFloat(DISSOLVE_PROPERTY, to);
    }

    public void ShowDissolveWeaponParticles()
    {
        dissolveParticleSystem.Play();
    }
    public void HideDissolveWeaponParticles()
    {
        dissolveParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void SpawnHitImpact(Vector3 position)
    {
        if (hitImpactPrefab == null) return;

        GameObject vfx = Instantiate(hitImpactPrefab, position, Quaternion.identity);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(vfx, 1.5f); // fallback if no particle system
        }
    }
    public void ShowSwordTrail()
    {
        swordTrail.Play();
    }
    public void HideSwordTrail()
    {
        swordTrail.Stop();
    }

    public void LastAttack()
    {
        lastAttackEffect.Play();
    }

    public void SetToDefaultBlade()
    {
        float currentDissolve = GetCurrentDissolveValue();
        Material newMat = new Material(defaultBladeMaterial); // clone
        newMat.SetFloat(DISSOLVE_PROPERTY, currentDissolve);
        bladeRenderer.materials = new Material[] { newMat };
        UpdateMaterialReferences(); // re-link to dissolve system
    }

    public void SetToUltimateBlade()
    {
        runtimeUltimateBladeMat = new Material(ultimateBladeMaterial);
        runtimeUltimateBladeMat.SetFloat("_GradientNoisePower", 400f); // <-- SET DEFAULT BASELINE
        bladeRenderer.materials = new Material[] { runtimeUltimateBladeMat };
        materials.Clear(); // clear dissolve list
    }
    public void UltimateBlade(float from, float to, float duration)
    {
        if (ultimateBladeRoutine != null)
            StopCoroutine(ultimateBladeRoutine);
        ultimateBladeRoutine = StartCoroutine(AnimateUltimateBlade(from, to, duration));
    }
    private IEnumerator AnimateUltimateBlade(float from, float to, float duration)
    {
        if (runtimeUltimateBladeMat == null || !runtimeUltimateBladeMat.HasProperty("_GradientNoisePower"))
            yield break;

        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            float value = Mathf.Lerp(from, to, t);
            runtimeUltimateBladeMat.SetFloat("_GradientNoisePower", value);

            timer += Time.deltaTime;
            yield return null;
        }

        runtimeUltimateBladeMat.SetFloat("_GradientNoisePower", to); // ensure final value
    }

    void UpdateMaterialReferences()
    {
        materials.Clear();
        foreach (Renderer r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty(DISSOLVE_PROPERTY))
                {
                    materials.Add(mat);
                }
            }
        }
    }
}
