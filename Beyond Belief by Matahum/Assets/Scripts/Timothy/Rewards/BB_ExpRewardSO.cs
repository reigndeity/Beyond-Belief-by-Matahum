using UnityEngine;
[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Experience Reward")]
public class BB_ExpRewardSO : BB_RewardSO
{
    public Sprite rewardBackground;
    public Sprite rewardIcon;
    public string rewardName;
    public int rewardQuantity;
    private Player player;

    public override void GiveReward()
    {
        player = FindFirstObjectByType<Player>();
        player.GainXP(RewardQuantity());
    }
    public override Sprite RewardBackground() => rewardBackground;
    public override Sprite RewardIcon() => rewardIcon;
    public override string RewardName() => rewardName;
    public override int RewardQuantity() => rewardQuantity;
}
