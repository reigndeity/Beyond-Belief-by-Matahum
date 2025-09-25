using UnityEngine;

public class MutyaNgLinta_Lifesteal : MonoBehaviour
{
    public float lifeStealPercent;
    public Player player;

    public void OnDamageDealt(float damage)
    {
        float healAmount = (damage * 0.66f) * lifeStealPercent;
        player.Heal(healAmount);
    }
}
