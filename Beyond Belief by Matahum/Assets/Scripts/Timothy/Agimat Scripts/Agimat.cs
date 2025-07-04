using UnityEngine;
using UnityEngine.UI;

public abstract class Agimat : MonoBehaviour
{
    public abstract void FirstAbility();
    public abstract void SecondAbility();

    public abstract bool FirstPassive();
    public abstract bool SecondPassive();

    public abstract Sprite FirstIcon();
    public abstract Sprite SecondIcon();

    public abstract float FirstCooldown();
    public abstract float SecondCooldown();
}
