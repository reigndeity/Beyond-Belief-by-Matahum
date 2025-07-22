using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais_S1")]
public class MutyaNgMais_S1 : R_AgimatAbility
{
    public override string GetDescription(R_ItemRarity rarity)
    {
        float healAmount = GetRandomHealPercent(rarity);
        return $"Heals {healAmount:F1}% of the player's HP.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        float percent = GetRandomHealPercent(rarity) / 100f;
        // float amount = user.GetComponent<PlayerStats>().maxHP * percent;
        // user.GetComponent<PlayerStats>().Heal(amount);

        // Debug.Log($"🌽 Healing for {amount} HP ({percent * 100}%)");
    }

    private float GetRandomHealPercent(R_ItemRarity rarity)
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
