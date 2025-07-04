using Unity.VisualScripting;
using UnityEngine;

public class AgimatInventory : MonoBehaviour
{
    private UseAgimat useAgimat;
    public ItemSlot firstSlot;
    public ItemSlot secondSlot;
    public Agimat firstSkill;
    public Agimat secondSkill;

    private void Start()
    {
        useAgimat = FindFirstObjectByType<UseAgimat>();
        firstSlot.OnItemChanged += HandleItemChanged;
        secondSlot.OnItemChanged += HandleItemChanged;
    }

    private void HandleItemChanged(ItemSlot slot, InventoryItem oldItem, InventoryItem newItem)
    {
        if (slot == firstSlot)
        {
            if (newItem != null)
            {
                SetFirstAbility();
                Debug.Log("Set First Ability");
            }
            else
            {
                firstSkill = null;
                useAgimat.firstAgimat = null;
                Debug.Log("Remove First Ability");
            }
        }

        if (slot == secondSlot)
        {
            if (newItem != null)
            {
                SetSecondAbility();
                Debug.Log("Set Second Ability");
            }
            else
            {
                secondSkill = null;
                useAgimat.secondAgimat = null;
                Debug.Log("Remove Second Ability");
            }
        }
    }


    void SetFirstAbility()
    {
        firstSkill = firstSlot.GetComponentInChildren<Agimat>();
        useAgimat.firstAgimat = firstSkill;
    }
    void SetSecondAbility()
    {
        secondSkill = secondSlot.GetComponentInChildren<Agimat>();
        useAgimat.secondAgimat = secondSkill;
    }
}
