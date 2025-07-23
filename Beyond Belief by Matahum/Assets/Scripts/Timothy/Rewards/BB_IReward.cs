using UnityEngine;

public interface BB_IReward
{
    void GiveReward();
    Sprite RewardIcon();
    string RewardName();
    int RewardQuantity();
}