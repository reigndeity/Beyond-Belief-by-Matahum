using UnityEngine;
[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Experience Reward")]
public class BB_ExpRewardSO : BB_RewardSO
{
    private Player player;

    public override void GiveReward()
    {
        player = FindFirstObjectByType<Player>();
        player.GainXP(RewardQuantity());
    }
}
