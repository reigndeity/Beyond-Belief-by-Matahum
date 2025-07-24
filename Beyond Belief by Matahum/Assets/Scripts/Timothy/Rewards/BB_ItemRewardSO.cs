using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Item Reward")]
public class BB_ItemRewardSO : BB_RewardSO
{
    public R_ItemData item;
    public int rewardQuantity;

    public override void GiveReward()
    {
        R_Inventory inventory = FindFirstObjectByType<R_Inventory>();
        R_InventoryUI inventoryUI = FindFirstObjectByType<R_InventoryUI>();

        inventory.AddItem(item, RewardQuantity());
        inventoryUI?.RefreshUI();
    }

    public override Sprite RewardIcon() => item.itemIcon;
    public override string RewardName() => item.itemName;
    public override int RewardQuantity() => rewardQuantity;
}
