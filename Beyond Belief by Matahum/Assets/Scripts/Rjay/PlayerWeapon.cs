using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    private PlayerStats m_playerStats;
    public Collider weaponCollider;
    public float m_scalingAmount;
    [Header("Particle To Play when Dissolving Weapon")]
    public ParticleSystem swordParticleSystem;

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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            float damage = m_playerStats.p_attack * m_scalingAmount;
            Debug.Log(m_scalingAmount);
            damageable.TakeDamage(damage);
        }
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
    }

    public void UndissolveWeapon(float duration)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDissolve(GetCurrentDissolveValue(), 0f, duration));
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
}
