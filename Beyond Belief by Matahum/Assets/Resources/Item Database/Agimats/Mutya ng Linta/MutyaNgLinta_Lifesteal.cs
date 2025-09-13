using UnityEngine;

public class MutyaNgLinta_Lifesteal : MonoBehaviour
{
    public float lifeStealPercent;
    public Player player;

    public void OnDamageDealt(float damage)
    {
        float healAmount = damage * lifeStealPercent;
        player.Heal(healAmount);
    }
}
