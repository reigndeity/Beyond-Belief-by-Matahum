using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais_S2")]
public class MutyaNgMais_S2 : R_AgimatAbility
{
    [HideInInspector] public float cachedFlatHeal;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (cachedFlatHeal == 0f)
            cachedFlatHeal = GetRandomFlatHeal(rarity);

        return $"Regenerates {cachedFlatHeal:F1} HP every 5 seconds.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        if (cachedFlatHeal == 0f)
            cachedFlatHeal = GetRandomFlatHeal(rarity);

        user.GetComponent<Player>().Heal(cachedFlatHeal);

        Debug.Log($"🌽 Passive heal: +{cachedFlatHeal:F1} HP every {baseCooldown:F1}s");
        Debug.Log($"🌽 [PASSIVE] MutyaNgMais_S2 activated at {Time.time:F2}s");
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
