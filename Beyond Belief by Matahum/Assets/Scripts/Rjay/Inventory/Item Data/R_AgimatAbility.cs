using UnityEngine;

public abstract class R_AgimatAbility : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    public bool isPassive;

    // Generate the full description at runtime based on rarity
    public abstract string GetDescription(R_ItemRarity rarity);

    // Called when the player activates it
    public abstract void Activate(GameObject user, R_ItemRarity rarity);
}
