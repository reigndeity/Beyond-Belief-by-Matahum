using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ConsumableEffects/Heal")]
public class HealConsumable : R_ConsumableEffect
{
    public int healAmount = 50;

    public override void Apply(GameObject user)
    {
        if (user == null) return;

        Player stats = user.GetComponent<Player>();
        if (stats != null)
        {
            stats.TakeDamage(healAmount);
            Debug.Log($"Healed {healAmount} HP.");
        }
        else
        {
            Debug.LogWarning("PlayerStats component not found on user.");
        }
    }
}
