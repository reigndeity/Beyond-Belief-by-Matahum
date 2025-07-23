using UnityEngine;

public abstract class BB_RewardSO : ScriptableObject, BB_IReward
{
    public abstract void GiveReward();
    public abstract Sprite RewardIcon();
    public abstract string RewardName();
    public abstract int RewardQuantity();
}
