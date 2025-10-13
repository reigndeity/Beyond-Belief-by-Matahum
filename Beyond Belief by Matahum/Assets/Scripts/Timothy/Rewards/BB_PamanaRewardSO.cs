using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Pamana Reward")]
public class BB_PamanaRewardSO : BB_RewardSO
{
    [Header("If nothing is set, random pamana will spawn")]
    public R_ItemData item;
    public int level = 1;

    public override Sprite RewardIcon()
    {
        if (item != null)
        {
            return item.itemIcon;
        }
        return base.RewardIcon();
    }

    public override string RewardName()
    {
        if (item != null)
        {
            return item.itemName;
        }
        return base.RewardName();
    }

    public override Sprite RewardBackground()
    {
        if (item != null)
        {
            return item.itemBackdropIcon;
        }
        return base.RewardBackground();
    }

    public override void GiveReward()
    {
        R_GeneralItemSpawner pamanaSpawner = FindFirstObjectByType<R_GeneralItemSpawner>();

        if(item != null)
        {
            pamanaSpawner.SpawnSinglePamana(new R_ItemData[] { item }, level);
        }
        else
        {
            pamanaSpawner.SetSimulatedLevel(level);
            pamanaSpawner.SpawnRandomPamanaFromTemplate();
        }
    }
}
