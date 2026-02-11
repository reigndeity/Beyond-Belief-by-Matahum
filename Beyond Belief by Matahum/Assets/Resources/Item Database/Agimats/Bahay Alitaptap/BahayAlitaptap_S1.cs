using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/BahayAlitaptap/BahayAlitaptap_S1")]
public class BahayAlitaptap_S1 : R_AgimatAbility
{
    [Header("Skill Prefabs")]
    public GameObject orbPrefab;
    public GameObject pulsePrefab;

    [Header("Pulse Properties")]
    public float pulseInterval = 2f;
    public float pulseExpansionDuration = 0.25f;
    public float pulseDuration = 1;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float increasedDamage = itemData.slot1RollValue[0];
        return $"Place an orb on the ground that pulses 3 times. First pulse deals {increasedDamage:F1}% of ATK, increasing by 50% per pulse.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float increasedDamage = itemData.slot1RollValue[0];
        float atk = user.GetComponent<PlayerStats>().p_attack;
        float calculatedDamage = (increasedDamage / 100f) * atk;

        Vector3 spawnPoint = user.transform.position + user.transform.forward * 2f + Vector3.up * 0.5f;
        GameObject orbObj = Instantiate(orbPrefab, spawnPoint, Quaternion.identity);

        CoroutineRunner.Instance.RunCoroutine(PulseInterval(orbObj, calculatedDamage));
    }

    private IEnumerator PulseInterval(GameObject pulseSpawnPoint, float calculatedDamage)
    {
        yield return new WaitForSeconds(1f);

        float currentDamage = calculatedDamage;

        for (int i = 0; i < 3; i++)
        {
            GameObject pulseObj = Instantiate(pulsePrefab, pulseSpawnPoint.transform.position, pulsePrefab.transform.rotation);

            CoroutineRunner.Instance.RunCoroutine(ShowPulseVFX(pulseObj, i, currentDamage));
            currentDamage *= 1.5f;

            yield return new WaitForSeconds(pulseInterval);
        }

        yield return new WaitForSeconds(1f);

        float elapsed = 0f;
        float scaleDuration = 0.5f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            pulseSpawnPoint.transform.localScale = Vector3.Lerp(pulseSpawnPoint.transform.localScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(pulseSpawnPoint);
    }

    private IEnumerator ShowPulseVFX(GameObject pulseObj, int i, float currentDamage)
    {
        BahayAlitaptap_PulseDamage pulseDamage = pulseObj.GetComponent<BahayAlitaptap_PulseDamage>();
        pulseDamage.pulseDamage = currentDamage;

        Vector3 pulseSize = new Vector3(i + 2, i + 2, 0.5f);
        Vector3 originalSize = new Vector3(0, 0, 0.5f);
        float elapsed = 0f;
        float scaleDuration = 0.5f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            pulseObj.transform.localScale = Vector3.Lerp(originalSize, pulseSize, t);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            pulseObj.transform.localScale = Vector3.Lerp(pulseSize, originalSize, t);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Destroy(pulseObj);
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(30, 47.5f),
            R_ItemRarity.Rare => Random.Range(40, 60),
            R_ItemRarity.Epic => Random.Range(50, 72.5f),
            R_ItemRarity.Legendary => Random.Range(60, 85),
            _ => 30f
        };
    }
}
