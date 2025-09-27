using UnityEngine;

public class QuestDestroyInteractable : Interactable
{
    [SerializeField] R_Inventory r_Inventory;
    [SerializeField] R_ItemData questItem;
    [SerializeField] string missionID;
    public override void OnInteract()
    {
        BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        r_Inventory.AddItem(questItem, 1);
        Destroy(gameObject);
    }
}
