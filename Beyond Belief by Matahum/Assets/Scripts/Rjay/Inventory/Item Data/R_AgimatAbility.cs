using UnityEngine;

public abstract class R_AgimatAbility : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    
    [Header("Passive Timing")]
    public float passiveTriggerDelay = 1f; // configurable buffer after reaching cooldown

    public bool isPassive;

    [SerializeField] protected float baseCooldown = 5f;

    // Generate the full description at runtime based on rarity
    public abstract string GetDescription(R_ItemRarity rarity, R_ItemData itemData);

    // Called when the player activates it
    public abstract void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData);

    // ✅ Default random roll (override in children)
    public virtual float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return 0f; // fallback for abilities that don’t use damage % rolls
    }

    // 🔹 Cooldown duration for UI and logic
    public virtual float GetCooldown(R_ItemRarity rarity, PlayerStats stats = null)
    {
        float cooldown = baseCooldown;

        if (stats != null)
        {
            float cdrMultiplier = 1f - (stats.p_cooldownReduction / 100f);
            cooldown *= Mathf.Max(0.1f, cdrMultiplier);
        }

        return cooldown;
    }

    public virtual void Deactivate(GameObject user) { }
}
