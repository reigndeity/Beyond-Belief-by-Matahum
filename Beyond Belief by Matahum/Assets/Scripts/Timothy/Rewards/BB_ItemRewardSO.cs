using UnityEngine;

[CreateAssetMenu(menuName = "Beyond Belief/Rewards/Item Reward")]
public class BB_ItemRewardSO : BB_RewardSO
{
    public R_ItemData item;

    public override void GiveReward()
    {
        R_Inventory inventory = FindFirstObjectByType<R_Inventory>();
        R_InventoryUI inventoryUI = FindFirstObjectByType<R_InventoryUI>();

        inventory.AddItem(item, RewardQuantity());
        inventoryUI?.RefreshUI();
    }
}
