using Unity.VisualScripting;
using UnityEngine;

public abstract class Mangkukulam_Ability : ScriptableObject
{
    public abstract Coroutine AbilityActivate();
    public abstract void Activate();
    public abstract float Cooldown();
}
