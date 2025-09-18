using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgLinta/MutyaNgLinta_S2")]
public class MutyaNgLinta_S2 : R_AgimatAbility
{
    private Coroutine buffCoroutine;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float lifeStealValue = itemData.slot1RollValue;
        return $"Heal {lifeStealValue:F1}% of damage dealt with attacks to enemies.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float lifeStealValue = itemData.slot1RollValue;

        if (buffCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);

        buffCoroutine = CoroutineRunner.Instance.RunCoroutine(ApplyPassive(user, lifeStealValue));
    }

    public override void Deactivate(GameObject user)
    {
        if (buffCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);
            buffCoroutine = null;
        }

        var lifesteal = user.GetComponent<MutyaNgLinta_Lifesteal>();
        if (lifesteal != null)
            Object.Destroy(lifesteal);
    }

    private IEnumerator ApplyPassive(GameObject user, float lifeStealValue)
    {
        Player player = user.GetComponent<Player>();

        MutyaNgLinta_Lifesteal lifesteal = user.GetComponent<MutyaNgLinta_Lifesteal>();
        if (lifesteal == null)
            lifesteal = user.AddComponent<MutyaNgLinta_Lifesteal>();

        lifesteal.player = player;
        lifesteal.lifeStealPercent = lifeStealValue / 100f;
        lifesteal.enabled = true;

        while (true)
            yield return null;
    }

    public float GetRandomLifeStealPercentage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(5f, 10f),
            R_ItemRarity.Rare => Random.Range(7.5f, 13.3f),
            R_ItemRarity.Epic => Random.Range(10f, 16.6f),
            R_ItemRarity.Legendary => Random.Range(12.5f, 20f),
            _ => 5f
        };
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return GetRandomLifeStealPercentage(rarity);
    }
}
