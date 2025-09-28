using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ConsumableEffects/PercentHeal")]
public class PercentHealConsumable : R_ConsumableEffect
{
    public float percentHealAmount = 25;

    public override void Apply(GameObject user)
    {
        if (user == null) return;

        Player player = user.GetComponent<Player>();
        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            float finalHeal = 50;
            if (stats != null)
            {
                finalHeal = stats.p_maxHealth * (percentHealAmount / 100);
            }
            
            player.Heal(finalHeal);
            Debug.Log($"Healed {finalHeal} HP.");
        }
        else
        {
            Debug.LogWarning("PlayerStats component not found on user.");
        }
    }
}
