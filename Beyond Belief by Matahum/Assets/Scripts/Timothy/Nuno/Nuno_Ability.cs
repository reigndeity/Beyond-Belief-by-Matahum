using UnityEngine;

public abstract class Nuno_Ability : ScriptableObject
{
    public virtual bool CanBeUsed()
    {
        return true;
    }
    public abstract void Activate();
    public abstract void Deactivate();
}
