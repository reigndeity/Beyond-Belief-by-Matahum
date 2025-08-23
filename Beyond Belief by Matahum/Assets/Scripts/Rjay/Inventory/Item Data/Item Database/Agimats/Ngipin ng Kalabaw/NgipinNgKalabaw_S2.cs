using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/NgipinNgKalabaw_S2")]
public class NgipinNgKalabawS2 : R_AgimatAbility
{
    public float increaseStats;
    public float duration;
    public GameObject buffVFX;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (increaseStats == 0f)
            increaseStats = GetRandomFlatDamage(rarity);

        return $"Gains {increaseStats:F1} for {duration} seconds then push back surrounding enemies.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        float percent = increaseStats / 100f;
        float defPrcnt = user.GetComponent<PlayerStats>().p_defense;
        float amountToIncrease = defPrcnt * percent;

        CoroutineRunner.Instance.RunCoroutine(DamageIncreaseDuration(user, amountToIncrease));
        Debug.Log($"Increase Defense for {percent * 100}% of Defense.");
    }

    IEnumerator DamageIncreaseDuration(GameObject user, float amountToIncrease)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();

        // Apply buff
        stats.p_defense += (int)amountToIncrease;
        Debug.Log($"[Buff Applied] Defense increased by {amountToIncrease}. Current Defense: {stats.p_defense}");

        // Start VFX
        if (buffVFX != null)
            CoroutineRunner.Instance.RunCoroutine(ShowBuffVFX(user));

        // Wait for buff duration
        yield return new WaitForSeconds(duration);

        // Remove buff
        stats.p_defense -= (int)amountToIncrease;
        Debug.Log($"[Buff Ended] Defense reverted by {amountToIncrease}. Current Defense: {stats.p_defense}");
    }

    IEnumerator ShowBuffVFX(GameObject user)
    {
        Vector3 offset = new Vector3(0, 1, 0);
        // Spawn VFX on player
        GameObject vfxInstance = Instantiate(buffVFX, user.transform.position + offset, Quaternion.identity, user.transform);
        vfxInstance.transform.localScale = Vector3.zero;

        Vector3 buffSize = new Vector3(2, 2, 2);

        float elapsed = 0f;
        float scaleDuration = 0.5f;

        // Scale in
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            vfxInstance.transform.localScale = Vector3.Lerp(Vector3.zero, buffSize, t);
            yield return null;
        }

        // Keep full size until buff duration ends (minus fade out time)
        yield return new WaitForSeconds(duration - scaleDuration * 2);

        // Scale out
        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            vfxInstance.transform.localScale = Vector3.Lerp(buffSize, Vector3.zero, t);
            yield return null;
        }

        Destroy(vfxInstance);
    }

    private float GetRandomFlatDamage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(9f, 12.5f),
            R_ItemRarity.Rare => Random.Range(18f, 25f),
            R_ItemRarity.Epic => Random.Range(28f, 37.5f),
            R_ItemRarity.Legendary => Random.Range(42f, 50f),
            _ => 10f
        };
    }
}
