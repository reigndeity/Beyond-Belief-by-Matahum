using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgLinta/MutyaNgLinta_S1")]
public class MutyaNgLinta_S1 : R_AgimatAbility
{
    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float selfDamage = itemData.slot1RollValue;
        float lifeStealValue = itemData.slot2RollValue;

        return $"Deal damage to self equal to {selfDamage:F1}% of health. " +
               $"For 5 seconds, heal {lifeStealValue:F1}% of damage dealt with attacks to enemies.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float selfDamage = itemData.slot1RollValue;
        float lifeStealValue = itemData.slot2RollValue;
        CoroutineRunner.Instance.RunCoroutine(Lifesteal(user, selfDamage, lifeStealValue));
    }

    public override void Deactivate(GameObject user)
    {
        MutyaNgLinta_Lifesteal lifesteal = user.GetComponent<MutyaNgLinta_Lifesteal>();
        if (lifesteal != null)
            Object.Destroy(lifesteal, 1);
    }

    IEnumerator Lifesteal(GameObject user, float selfDamage, float lifeStealValue)
    {
        Player player = user.GetComponent<Player>();
        PlayerStats stats = user.GetComponent<PlayerStats>();

        float damageReduction = stats.p_defense * 0.66f;
        float damageToSelf = damageReduction + (stats.p_currentHealth * (selfDamage / 100f));

        float totalHealth = stats.p_currentHealth - damageToSelf;
        if (totalHealth <= 10f)
            damageToSelf = stats.p_currentHealth - 10f;

        player.TakeDamage(damageToSelf, false);

        MutyaNgLinta_Lifesteal lifesteal = user.GetComponent<MutyaNgLinta_Lifesteal>();
        if (lifesteal == null)
            lifesteal = user.AddComponent<MutyaNgLinta_Lifesteal>();

        lifesteal.player = player;
        lifesteal.enabled = true;
        lifesteal.lifeStealPercent = lifeStealValue / 100f;

        yield return new WaitForSeconds(5f);

        lifesteal.enabled = false;
    }

    // Self damage roll
    public float GetRandomSelfDamagePercentage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(48f, 50f),
            R_ItemRarity.Rare => Random.Range(36f, 41.6f),
            R_ItemRarity.Epic => Random.Range(24f, 33.3f),
            R_ItemRarity.Legendary => Random.Range(12f, 25f),
            _ => 50f
        };
    }

    // Lifesteal roll
    public float GetRandomLifeStealPercentage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(25f, 50f),
            R_ItemRarity.Rare => Random.Range(33.3f, 66.6f),
            R_ItemRarity.Epic => Random.Range(41.6f, 83.3f),
            R_ItemRarity.Legendary => Random.Range(50f, 100f),
            _ => 25f
        };
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        // default to self-damage for compatibility
        return GetRandomSelfDamagePercentage(rarity);
    }
}
