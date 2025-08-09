using UnityEngine;

public abstract class BB_RewardSO : ScriptableObject, BB_IReward
{
    [Header("Reward Info")]
    public int baseQuantity = 1; // Starting quantity before scaling

    private int currentQuantity; // Mutable quantity
    public void SetQuantity(int quantity)
    {
        currentQuantity = quantity;
    }
    public abstract void GiveReward();
    public abstract Sprite RewardBackground();
    public abstract Sprite RewardIcon();
    public abstract string RewardName();
    public virtual int RewardQuantity()
    {
        return currentQuantity;
    }
}
