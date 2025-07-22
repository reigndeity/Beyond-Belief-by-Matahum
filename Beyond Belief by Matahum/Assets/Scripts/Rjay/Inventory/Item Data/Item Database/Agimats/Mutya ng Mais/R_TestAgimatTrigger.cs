using UnityEngine;

public class R_TestAgimatTrigger : MonoBehaviour
{
    public R_ItemData agimatItem; // Drag `Item_NgipinNgKidlat` here in Inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryUseAgimatAbility(agimatItem, 1, gameObject); // Simulate Slot 1
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryUseAgimatAbility(agimatItem, 2, gameObject); // Simulate Slot 2
        }
    }

    private void TryUseAgimatAbility(R_ItemData item, int slot, GameObject user)
    {
        if (item == null || item.itemType != R_ItemType.Agimat)
        {
            Debug.LogWarning("No valid agimat equipped!");
            return;
        }

        var ability = slot == 1 ? item.slot1Ability : item.slot2Ability;

        if (ability != null && !ability.isPassive)
        {
            //ability.Activate(user);
        }
        else
        {
            Debug.Log($"Slot {slot} ability is either null or passive.");
        }
    }
}
