using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Item Reward")]
public class BB_ItemRewardSO : BB_RewardSO
{
    public R_ItemData item;
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
        R_Inventory inventory = FindFirstObjectByType<R_Inventory>();
        R_InventoryUI inventoryUI = FindFirstObjectByType<R_InventoryUI>();

        inventory.AddItem(item, RewardQuantity());
        inventoryUI?.RefreshUI();
    }
}
