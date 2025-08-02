using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Custom Reward")]
public class BB_GoldRewardSO : BB_RewardSO
{
    public Sprite rewardBackground;
    public Sprite rewardIcon;
    public string rewardName;
    public int rewardQuantity;

    public override void GiveReward()
    {
        //GoldManager manager = FindFirstObject<GoldManager>();
        //manager.GiveGold(RewardQuantity());
    }
    public override Sprite RewardBackground() => rewardBackground;
    public override Sprite RewardIcon() => rewardIcon;
    public override string RewardName() => rewardName;
    public override int RewardQuantity() => rewardQuantity;
}
