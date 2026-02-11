using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ConsumableEffects/FlatHeal")]
public class FlatHealConsumable : R_ConsumableEffect
{
    public int flatHealAmount = 50;

    public override void Apply(GameObject user)
    {
        if (user == null) return;

        Player stats = user.GetComponent<Player>();
        if (stats != null)
        {
            stats.Heal(flatHealAmount);
            Debug.Log($"Healed {flatHealAmount} HP.");
        }
        else
        {
            Debug.LogWarning("PlayerStats component not found on user.");
        }
    }
}
