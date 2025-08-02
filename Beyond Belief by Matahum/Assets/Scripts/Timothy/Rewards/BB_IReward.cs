using UnityEngine;

public interface BB_IReward
{
    void GiveReward();
    Sprite RewardBackground();
    Sprite RewardIcon();
    string RewardName();
    int RewardQuantity();
}