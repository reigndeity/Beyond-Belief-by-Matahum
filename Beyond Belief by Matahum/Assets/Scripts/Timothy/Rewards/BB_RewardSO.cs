using UnityEngine;

public abstract class BB_RewardSO : ScriptableObject, BB_IReward
{
    public int baseQuantity;
    [HideInInspector] public int multiplier = 1;

    public Sprite rewardBackground;
    public Sprite rewardIcon;
    public string rewardName;
    public void SetMultiplier(int modifiedMultiplier)
    {
        multiplier = modifiedMultiplier;
    }
    public abstract void GiveReward();
    public virtual Sprite RewardBackground() => rewardBackground;
    public virtual Sprite RewardIcon() => rewardIcon;
    public virtual string RewardName() => rewardName;
    public virtual int RewardQuantity()
    {
        return Mathf.RoundToInt(baseQuantity * multiplier);
    }
}
