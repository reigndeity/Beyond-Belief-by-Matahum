using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Custom Reward")]
public class BB_CustomRewardSO : BB_RewardSO
{
    public Sprite rewardIcon;
    public string rewardName;
    public int rewardQuantity;
    public BB_CustomUnityEvent customRewardEvent;

    public override void GiveReward()
    {
        customRewardEvent.Invoke();
    }

    public override Sprite RewardIcon() => rewardIcon;
    public override string RewardName() => rewardName;
    public override int RewardQuantity() => rewardQuantity;
}
