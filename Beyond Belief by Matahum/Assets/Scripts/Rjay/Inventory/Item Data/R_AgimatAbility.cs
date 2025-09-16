using UnityEngine;

public abstract class R_AgimatAbility : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    
    [Header("Passive Timing")]
    public float passiveTriggerDelay = 1f; // configurable buffer after reaching cooldown

    public bool isPassive;

    // 🔸 Base cooldown value, can be overridden by child class
    [SerializeField] protected float baseCooldown = 5f;

    // Generate the full description at runtime based on rarity
    public abstract string GetDescription(R_ItemRarity rarity);

    // Called when the player activates it
    public abstract void Activate(GameObject user, R_ItemRarity rarity);

    // 🔹 Cooldown duration for UI and logic
    public virtual float GetCooldown(R_ItemRarity rarity, PlayerStats stats = null)
    {
        float cooldown = baseCooldown;

        // Optional rarity scaling (if you want later)
        // cooldown *= GetRarityMultiplier(rarity);

        if (stats != null)
        {
            float cdrMultiplier = 1f - (stats.p_cooldownReduction / 100f);
            cooldown *= Mathf.Max(0.1f, cdrMultiplier); // don’t let it go below 10% of base
        }

        return cooldown;
    }


    public virtual void Deactivate(GameObject user)
    {

    }
}
