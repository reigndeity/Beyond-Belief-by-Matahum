using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais_S2")]
public class MutyaNgMais_S2 : R_AgimatAbility
{
    public override string GetDescription(R_ItemRarity rarity)
    {
        float healAmount = GetRandomFlatHeal(rarity);
        return $"Regenerates {healAmount:F1} HP every 5 seconds.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        float heal = GetRandomFlatHeal(rarity);
        Debug.Log($"🌽 Passive heal: {heal} every 5s (apply this over time externally)");
        // You’d register this in a passive regen system or coroutine.
    }

    private float GetRandomFlatHeal(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(2f, 3f),
            R_ItemRarity.Rare => Random.Range(4f, 6f),
            R_ItemRarity.Epic => Random.Range(7f, 9f),
            R_ItemRarity.Legendary => Random.Range(10f, 12f),
            _ => 1f
        };
    }
}
