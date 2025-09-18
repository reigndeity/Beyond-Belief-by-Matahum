using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSampalok/MutyaNgSampalok_S2")]
public class MutyaNgSampalok_S2 : R_AgimatAbility
{
    // Enemy subscription map
    private Dictionary<IDeathHandler, System.Action> subscribedEnemies = new Dictionary<IDeathHandler, System.Action>();

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float hpRoll = itemData.slot1RollValue;
        float staminaRoll = itemData.slot2RollValue;

        return $"Defeating an enemy restores {hpRoll:F1}% of health and {staminaRoll:F1} stamina.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float radius = 10f;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapSphere(user.transform.position, radius, enemyLayer);

        foreach (var hit in hits)
        {
            IDeathHandler deathHandler = hit.GetComponent<IDeathHandler>();
            if (deathHandler != null && !subscribedEnemies.ContainsKey(deathHandler))
            {
                System.Action handler = () => OnEnemyDeath(user, itemData);
                subscribedEnemies[deathHandler] = handler;
                deathHandler.OnDeath += handler;
            }
        }
    }

    public override void Deactivate(GameObject user)
    {
        foreach (var kvp in subscribedEnemies)
        {
            if (kvp.Key != null)
                kvp.Key.OnDeath -= kvp.Value;
        }
        subscribedEnemies.Clear();
    }

    private void OnEnemyDeath(GameObject user, R_ItemData itemData)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();
        PlayerMovement playerMovement = user.GetComponent<PlayerMovement>();
        Player player = user.GetComponent<Player>();
        if (stats == null || player == null) return;

        float hpRoll = itemData.slot1RollValue;
        float staminaRoll = itemData.slot2RollValue;

        float healAmount = stats.p_maxHealth * (hpRoll / 100f);
        player.Heal(healAmount);

        playerMovement.currentStamina += staminaRoll;

        Debug.Log($"🌿 Restored {hpRoll:F1}% HP and {staminaRoll:F1} stamina on enemy death.");
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        // Slot1 = HP restore %, Slot2 = stamina restore
        // We'll default to HP restore here
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(5f, 10f),
            R_ItemRarity.Rare => Random.Range(7.5f, 12.5f),
            R_ItemRarity.Epic => Random.Range(10f, 15f),
            R_ItemRarity.Legendary => Random.Range(12.5f, 20f),
            _ => 5f
        };
    }

    public float GetRandomStaminaRestore(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(7.5f, 20f),
            R_ItemRarity.Rare => Random.Range(15f, 30f),
            R_ItemRarity.Epic => Random.Range(22.5f, 40f),
            R_ItemRarity.Legendary => Random.Range(30f, 50f),
            _ => 7.5f
        };
    }
}
