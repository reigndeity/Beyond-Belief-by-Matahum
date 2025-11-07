using UnityEngine;
using UnityEngine.Events;

public class CollectibleInteractable : Interactable
{
    [SerializeField] R_Inventory r_Inventory;
    [SerializeField] R_ItemData questItem;

    [Header("Events")]
    public UnityEvent onInteract;

    public override void OnInteract()
    {
        base.OnInteract();
        r_Inventory.AddItem(questItem, 1);
        // Trigger event
        onInteract?.Invoke();
        this.gameObject.SetActive(false);
    }
}
