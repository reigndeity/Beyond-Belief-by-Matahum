using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
//using static UnityEditor.Progress;
using Unity.VisualScripting;
using Unity.PlasticSCM.Editor.WebApi;

public class ItemActionController : MonoBehaviour
{
    public static ItemActionController Instance;


    [Header("Item Button Choices")]
    public GameObject itemActionControllerGameObject;
    public Button equipButton; //Equip the item
    public Button useButton; //Use a consumable item
    public Button moveButton; //Move any item. Moving it to another item will swap them
    public Button removeItemButton; //Delete any item. Doesnt work with Agimat
    public Button lockAndUnlockButton; //Lock or Unlock a slot. Cannot interact with
    //public Button sortMainInventoryButton; //Sort the main inventory based on their item type

    [Header("For Moving")]
    public bool isMoving;
    public ItemSlot currentSelectedSlot;
    public ItemSlot movingFromSlot;

    [Header("For Agimat")]
    public Button equipFirstAbilityButton;
    public Button equipSecondAbilityButton;
    public ItemSlot agimatFirstSlot;
    public ItemSlot agimatSecondSlot;
    //public Button sortAgimatInventoryButton; //Sort    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        equipButton.onClick.AddListener(EquipOrUnequipSelectedItem);
        moveButton.onClick.AddListener(TryMoveOrSwap);
        removeItemButton.onClick.AddListener(DestroySelectedItem);
        useButton.onClick.AddListener(UseSelectedItem);
        lockAndUnlockButton.onClick.AddListener(LockOrUnlock);

        equipFirstAbilityButton.onClick.AddListener(EquipToFirstSlot);
        equipSecondAbilityButton.onClick.AddListener(EquipToSecondSlot);

        ShowChoices(false);
    }

    // Called when an ItemSlot is clicked
    public void HandleSlotSelection(ItemSlot selectedSlot)
    {
        if (selectedSlot == currentSelectedSlot)
        {
            //DisableAllButtons();
            ShowChoices(false);
            selectedSlot.DeselectItem();
            currentSelectedSlot = null;
            Debug.Log("Same Slot");
            return;
        }

        currentSelectedSlot = selectedSlot;

        if (isMoving)
        {
            TryMoveOrSwap();
            isMoving = false;
            return;
        }

        CheckForOptions(selectedSlot);
    }

    //Check options if the item is interactable
    public void CheckForOptions(ItemSlot selectedSlot)
    {
        // Start by disabling everything
        //DisableAllButtons();
        ShowChoices(true);
        lockAndUnlockButton.gameObject.SetActive(true);

        TextMeshProUGUI lockSlotText = lockAndUnlockButton.GetComponentInChildren<TextMeshProUGUI>();
        lockSlotText.text = selectedSlot.isSlotLocked ? "Unlock" : "Lock";

        if (selectedSlot.isSlotLocked) return;

        CheckIfAgimat(selectedSlot);
    }

    private void CheckIfAgimat(ItemSlot selectedSlot)
    {
        if (selectedSlot.allowedType == ItemType.Agimat)
        {
            ItemAgimat(selectedSlot);
        }
        else
        {
            ItemAny(selectedSlot);
        }
    }

    private void ItemAgimat(ItemSlot selectedSlot)
    {
        InventoryItem item = selectedSlot.currentItem;
        if (item != null)
        {
            if (item.isEquippable)
            {
                TextMeshProUGUI equipFirstText = equipFirstAbilityButton.GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI equipSecondText = equipSecondAbilityButton.GetComponentInChildren<TextMeshProUGUI>();
                equipFirstText.text = "Equip to First Slot";
                equipSecondText.text = "Equip to Second Slot";

                equipFirstAbilityButton.gameObject.SetActive(true);
                equipSecondAbilityButton.gameObject.SetActive(true);
                moveButton.gameObject.SetActive(true);

                if (selectedSlot == agimatFirstSlot)
                {
                    //moveButton.interactable = false;
                    moveButton.gameObject.SetActive(false);
                    equipFirstText.text = "Unequip";
                }

                if (selectedSlot == agimatSecondSlot)
                {
                    //moveButton.interactable = false;
                    moveButton.gameObject.SetActive(false);
                    equipSecondText.text = "Unequip";
                }
            }
            removeItemButton.interactable = true;
        }
    }

    public void EquipToFirstSlot()
    {
        InventoryItem item = currentSelectedSlot.GetComponentInChildren<InventoryItem>();
        IEquippable equippable = item.GetComponent<IEquippable>();

        if (!equippable.isEquipped || currentSelectedSlot != agimatFirstSlot)
            equippable.Equip(agimatFirstSlot);
        else
            equippable.Unequip();

        //DisableAllButtons();
        ShowChoices(false);
    }

    public void EquipToSecondSlot()
    {
        IEquippable equippable = currentSelectedSlot.GetComponentInChildren<IEquippable>();

        if (!equippable.isEquipped || currentSelectedSlot != agimatSecondSlot)
            equippable.Equip(agimatSecondSlot);
        else
            equippable.Unequip();

        //DisableAllButtons();
        ShowChoices(false);
    }

    private void ItemAny(ItemSlot selectedSlot)
    {
        IEquippable itemEquipped = selectedSlot.GetComponentInChildren<IEquippable>();
        if (itemEquipped != null && itemEquipped.isEquipped)
        {
            TextMeshProUGUI equipText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
            equipText.text = "Unequip";
            //equipButton.interactable = true;
            equipButton.gameObject.SetActive(true);
            return;
        }


        InventoryItem item = selectedSlot.currentItem;
        if (item != null)
        {
            if (item.isUsable) useButton.gameObject.SetActive(true);
            if (item.isEquippable)
            {
                TextMeshProUGUI equipText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                equipText.text = "Equip";

                equipButton.gameObject.SetActive(true);
            }
            moveButton.gameObject.SetActive(true);
            removeItemButton.gameObject.SetActive(true);
        }
    }


    // Called by a "Move" or "Swap" button in UI
    public void TryMoveOrSwap()
    {
        if (!isMoving)
        {
            isMoving = true;
            movingFromSlot = currentSelectedSlot;

            //Temporary
            movingFromSlot.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            if (movingFromSlot == currentSelectedSlot //If selected the same slot
            || movingFromSlot.isEquipmentSlot || currentSelectedSlot.isEquipmentSlot
            || (currentSelectedSlot.allowedType == ItemType.Agimat) != (movingFromSlot.allowedType == ItemType.Agimat))
            {      
                
                isMoving = false;
                movingFromSlot.GetComponent<Image>().color = Color.black;
                ShowChoices(false);

                if (movingFromSlot.isSlotLocked || currentSelectedSlot.isSlotLocked)//if the slot is locked
                {
                    movingFromSlot.GetComponent<Image>().color = Color.yellow;
                }
                return;

            }

            SwapItemsAnimated(movingFromSlot, currentSelectedSlot);

            //Temporary
            movingFromSlot.GetComponent<Image>().color = Color.black;

            isMoving = false;
            movingFromSlot = null;
            ShowChoices(false);
        }
    }




    // Called by "Destroy" button
    public void DestroySelectedItem()
    {
        InventoryItem item = currentSelectedSlot.GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            Destroy(item.gameObject);
        }

        ShowChoices(false);
    }

    // Called by "Use" button
    public void UseSelectedItem()
    {
        InventoryItem item = currentSelectedSlot.GetComponentInChildren<InventoryItem>();
        IUsable usable = item.GetComponent<IUsable>();

        usable.Use();
    }

    // Equip/Unequip could later be expanded for your equipment system
    public void EquipOrUnequipSelectedItem()
    {
        IEquippable equippable = currentSelectedSlot.GetComponentInChildren<IEquippable>();

        if (equippable.isEquipped == false)
            equippable.Equip();
        else
            equippable.Unequip();

        ShowChoices(false);
    }

    public void LockOrUnlock()
    {
        currentSelectedSlot.isSlotLocked = !currentSelectedSlot.isSlotLocked;
        Debug.Log($"Is this slot locked?: {currentSelectedSlot.isSlotLocked}");

        TextMeshProUGUI buttonText = lockAndUnlockButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = currentSelectedSlot.isSlotLocked ? "Unlock" : "Lock";

        currentSelectedSlot.SaveLockedState();

        ShowChoices(false);
        movingFromSlot = null;
        currentSelectedSlot = null;
    }

    public void DisableAllButtons()
    {
        equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip/Unequip";
        equipFirstAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip to First Slot";
        equipSecondAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip to Second Slot";
        lockAndUnlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Lock/Unlock";

        ShowChoices(false);
    }

    public void ShowChoices(bool isShow)
    {
        int childCount = itemActionControllerGameObject.transform.childCount;
        GameObject[] children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = itemActionControllerGameObject.transform.GetChild(i).gameObject;
        }

        foreach (GameObject child in children)
        {
            child.SetActive(false);
        }

        if (isShow)
        {
            itemActionControllerGameObject.SetActive(isShow);
            GameObject slotObj = currentSelectedSlot.gameObject;
            itemActionControllerGameObject.transform.position = new Vector2(slotObj.transform.position.x, slotObj.transform.position.y + 50);
        }
        else
        {
            itemActionControllerGameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            ItemActionController.Instance.currentSelectedSlot = null;
        }
    }

    public void SwapItemsAnimated(ItemSlot fromSlot, ItemSlot toSlot)
    {
        StartCoroutine(SwapCoroutine(fromSlot, toSlot));
    }
    private IEnumerator SwapCoroutine(ItemSlot fromSlot, ItemSlot toSlot)
    {
        if (fromSlot.isSlotLocked || toSlot.isSlotLocked) yield break;

        var fromItem = fromSlot.currentItem;
        var toItem = toSlot.currentItem;

        var fromRect = fromItem?.GetComponent<RectTransform>();
        var toRect = toItem?.GetComponent<RectTransform>();

        Vector3 fromPos = fromRect?.position ?? Vector3.zero;

        // ✅ Correct destination for empty slot
        Vector3 toPos = toRect != null ? toRect.position : toSlot.transform.position;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            if (fromRect != null)
                fromRect.position = Vector3.Lerp(fromPos, toPos, t);

            if (toRect != null)
                toRect.position = Vector3.Lerp(toPos, fromPos, t);

            yield return null;
        }

        // ✅ MOVE logic
        if (toItem == null)
        {
            if (fromRect != null)
            {
                fromRect.SetParent(toSlot.transform);
                fromRect.localPosition = Vector3.zero;
            }

            toSlot.SetItem(fromItem);
            fromSlot.ClearItem();
            yield break;
        }

        // SWAP logic
        if (fromRect != null)
        {
            fromRect.SetParent(toSlot.transform);
            fromRect.localPosition = Vector3.zero;
        }

        if (toRect != null)
        {
            toRect.SetParent(fromSlot.transform);
            toRect.localPosition = Vector3.zero;
        }

        fromSlot.SetItem(toItem);
        toSlot.SetItem(fromItem);
    }
}