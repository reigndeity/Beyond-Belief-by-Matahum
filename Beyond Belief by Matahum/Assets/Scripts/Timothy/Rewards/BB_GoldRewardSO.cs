using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Custom Reward")]
public class BB_GoldRewardSO : BB_RewardSO
{
    public override void GiveReward()
    {
        //GoldManager manager = FindFirstObject<GoldManager>();
        //manager.GiveGold(RewardQuantity());
    }
}
