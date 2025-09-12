using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSampalok/MutyaNgSampalok_S2")]
public class MutyaNgSampalok_S2 : R_AgimatAbility
{
    [HideInInspector] public float healthRestore = 0f;
    [HideInInspector] public float staminaRestore = 0f;

    // Keep track of enemies + their handlers
    private Dictionary<IDeathHandler, System.Action> subscribedEnemies = new Dictionary<IDeathHandler, System.Action>();

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (healthRestore == 0)
            healthRestore = GetRandomHealthRestorePercentage(rarity);

        if (staminaRestore == 0)
            staminaRestore = GetRandomStaminaRestore(rarity);

        return $"Defeating an enemy restores {healthRestore:F1}% of health and {staminaRestore:F1} stamina.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        float radius = 10f; // detection range
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider[] hits = Physics.OverlapSphere(user.transform.position, radius, enemyLayer);

        foreach (var hit in hits)
        {
            IDeathHandler deathHandler = hit.GetComponent<IDeathHandler>();
            if (deathHandler != null && !subscribedEnemies.ContainsKey(deathHandler))
            {
                System.Action handler = () => OnEnemyDeath(user);
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

    private void OnEnemyDeath(GameObject user)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();
        PlayerMovement playerMovement = user.GetComponent<PlayerMovement>();
        Player player = user.GetComponent<Player>();
        if (stats == null && player == null) return;

        // Restore values
        float healthToRestore = stats.p_maxHealth * (healthRestore / 100f);
        player.Heal(healthRestore);
        //stats.p_currentHealth = Mathf.Min(stats.p_currentHealth + healthToRestore, stats.p_maxHealth);

        playerMovement.currentStamina += staminaRestore;

        Debug.Log($"Restored {healthRestore:F1}% HP and {staminaRestore:F1} stamina on enemy death.");
    }

    #region RANDOM VALUE GENERATOR
    private float GetRandomHealthRestorePercentage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(5f, 10f),
            R_ItemRarity.Rare => Random.Range(7.5f, 12.5f),
            R_ItemRarity.Epic => Random.Range(10f, 15f),
            R_ItemRarity.Legendary => Random.Range(12.5f, 20),
            _ => 5f
        };
    }

    private float GetRandomStaminaRestore(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(7.5f, 20f),
            R_ItemRarity.Rare => Random.Range(15, 30),
            R_ItemRarity.Epic => Random.Range(22.5f, 40f),
            R_ItemRarity.Legendary => Random.Range(30, 50),
            _ => 7.5f
        };
    }
    #endregion
}
