using Unity.VisualScripting;
using UnityEngine;

public abstract class Mangkukulam_Ability : ScriptableObject
{
    public abstract void Initialize();
    public abstract bool CanBeUsed();
    public virtual float Cooldown()
    {
        return Cooldown();
    }

    public abstract void Activate(GameObject user);
    
}
