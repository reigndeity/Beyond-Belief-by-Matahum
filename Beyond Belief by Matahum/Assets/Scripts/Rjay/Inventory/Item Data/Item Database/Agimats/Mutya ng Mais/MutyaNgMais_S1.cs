using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais_S1")]
public class MutyaNgMais_S1 : R_AgimatAbility
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Activate(GameObject user)
    {
        Debug.Log($"Mutya ng Mais Slot 1 Ability");
    }
}
