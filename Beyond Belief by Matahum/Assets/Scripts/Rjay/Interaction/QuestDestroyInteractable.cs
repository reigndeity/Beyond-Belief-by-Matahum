using UnityEngine;

public class QuestDestroyInteractable : Interactable
{
    [SerializeField] R_Inventory r_Inventory;
    [SerializeField] R_ItemData garlicQuestItem;
    public override void OnInteract()
    {
        BB_QuestManager.Instance.UpdateMissionProgressOnce("A1_Q1_Garlic");
        r_Inventory.AddItem(garlicQuestItem, 1);
        Destroy(gameObject);
    }
}
