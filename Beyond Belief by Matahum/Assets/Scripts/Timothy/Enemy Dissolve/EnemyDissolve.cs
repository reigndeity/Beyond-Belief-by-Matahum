using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDissolve : MonoBehaviour
{
    public Renderer[] targetRenderer;
    public GameObject[] turnOffGameObject;
    [SerializeField] private string dissolveProperty = "_Dissolve";
    [SerializeField] private float defaultDuration = 1f;

    private ParticleSystem enemyParticle;
    private List<Material> materials = new List<Material>();
    private Coroutine dissolveRoutine;

    void Awake()
    {
        foreach (var renderer in targetRenderer)
        {
            foreach (var mat in renderer.materials)
                materials.Add(mat);
        }

        enemyParticle = GetComponentInChildren<ParticleSystem>();
    }

    public void Dissolve(float duration)
    {
        if (dissolveRoutine != null) StopCoroutine(dissolveRoutine);

        dissolveRoutine = StartCoroutine(SetDissolve(1f, duration));
        
        if (turnOffGameObject != null)
        {
            foreach (var gameObj in turnOffGameObject)
            {
                gameObj.SetActive(false);
            }
        }

        if(enemyParticle != null) enemyParticle.Play();
    }

    public void Undissolve(float duration)
    {
        if (dissolveRoutine != null) StopCoroutine(dissolveRoutine);
        dissolveRoutine = StartCoroutine(SetDissolve(0f, duration));
    }

    [ContextMenu("Dissolve (Default Duration)")]
    private void DissolveInspector()
    {
        Dissolve(defaultDuration);
    }

    [ContextMenu("Undissolve (Default Duration)")]
    private void UndissolveInspector()
    {
        Undissolve(defaultDuration);
    }

    private IEnumerator SetDissolve(float targetValue, float duration)
    {
        float startValue = materials[0].GetFloat(dissolveProperty);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newValue = Mathf.Lerp(startValue, targetValue, t);

            foreach (var mat in materials)
                mat.SetFloat(dissolveProperty, newValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in materials)
            mat.SetFloat(dissolveProperty, targetValue);
    }
}