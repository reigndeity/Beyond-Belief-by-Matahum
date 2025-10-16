using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Agimat Reward")]
public class BB_AgimatRewardSO : BB_RewardSO
{
    [Header("If nothing is set, random agimat will spawn")]
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

    public override void GiveReward()
    {
        R_GeneralItemSpawner agimataSpawner = FindFirstObjectByType<R_GeneralItemSpawner>();

        if (item != null)
        {

            agimataSpawner.SpawnSingleAgimat(new R_ItemData[] { item }, level);
        }
        else
        {
            agimataSpawner.SetSimulatedLevel(level);
            agimataSpawner.SpawnRandomAgimatFromTemplate();
        }
    }
}
