using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/NgipinNgKalabaw/NgipinNgKalabaw_S2")]
public class NgipinNgKalabawS2 : R_AgimatAbility
{
    public float duration;
    public GameObject buffVFX;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot2RollValue;
        return $"Gains additional {roll:F1}% of defense for {duration} seconds then push back surrounding enemies, damaging them.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot2RollValue;
        float percent = roll / 100f;
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

        if (buffVFX != null)
            CoroutineRunner.Instance.RunCoroutine(ShowBuffVFX(user));

        yield return new WaitForSeconds(duration);

        stats.p_defense -= (int)amountToIncrease;
        Debug.Log($"[Buff Ended] Defense reverted by {amountToIncrease}. Current Defense: {stats.p_defense}");
    }

    IEnumerator ShowBuffVFX(GameObject user)
    {
        Vector3 offset = new Vector3(0, 1, 0);
        GameObject vfxInstance = Instantiate(buffVFX, user.transform.position + offset, Quaternion.identity, user.transform);
        vfxInstance.transform.localScale = Vector3.zero;

        NgipinNgKalabaw_Knockback knockback = vfxInstance.GetComponent<NgipinNgKalabaw_Knockback>();
        knockback.canDamage = true;
        knockback.damage = user.GetComponent<PlayerStats>().p_attack / 2;

        Vector3 buffSize = new Vector3(2, 2, 2);
        float elapsed = 0f;
        float scaleDuration = 0.25f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            vfxInstance.transform.localScale = Vector3.Lerp(Vector3.zero, buffSize, t);
            yield return null;
        }

        knockback.canDamage = false;
        yield return new WaitForSeconds(duration - scaleDuration * 2);

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

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
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
