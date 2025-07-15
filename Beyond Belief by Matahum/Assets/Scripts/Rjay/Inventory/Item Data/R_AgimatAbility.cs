using UnityEngine;

public abstract class R_AgimatAbility : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    [TextArea] public string description;
    public bool isPassive;

    public abstract void Activate(GameObject user);
}
