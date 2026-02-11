using UnityEngine;
using UnityEngine.Events;

public class QuestDestroyInteractable : Interactable
{
    [SerializeField] R_Inventory r_Inventory;
    [SerializeField] R_ItemData questItem;
    [SerializeField] string missionID;

    [Header("Events")]
    public UnityEvent onInteract;

    public override void OnInteract()
    {
        base.OnInteract();

        BB_QuestManager.Instance.UpdateMissionProgressOnce(missionID);
        r_Inventory.AddItem(questItem, 1);

        // Trigger event
        onInteract?.Invoke();

        Destroy(gameObject);
    }
}
