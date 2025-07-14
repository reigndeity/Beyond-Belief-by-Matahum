using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
//using static UnityEditor.Progress;
using Unity.VisualScripting;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using static UnityEditor.Progress;

public class ItemActionController : MonoBehaviour
{
    public static ItemActionController Instance;

    [Header("Item Button Choices")]
    public GameObject itemActionControllerGameObject;
    public Button equipButton; //Equip the item
    public Button useButton; //Use a consumable item
    //public Button moveButton; //Move any item. Moving it to another item will swap them
    public Button removeItemButton; //Delete any item. Doesnt work with Agimat
    public Button lockAndUnlockButton; //Lock or Unlock a slot. Cannot interact with

    [Header("Remove Item Confirmation")]
    public GameObject removeItemConfirmationPanel;
    public Button increaseAmountButton;
    public Button decreaseAmountButton;
    public int amountToRemove = 1;
    public TextMeshProUGUI amountToRemoveText;
    public TextMeshProUGUI itemName;
    public ItemSlot toDelete;

    [Header("For Moving")]
    public bool isMoving;
    public ItemSlot currentSelectedSlot;
    public ItemSlot movingFromSlot;

    [Header("For Agimat")]
    public Button equipFirstAbilityButton;
    public Button equipSecondAbilityButton;
    public ItemSlot agimatFirstSlot;
    public ItemSlot agimatSecondSlot;

    [Header("Is Character Details Active")]
    public GameObject charDetailsHolder;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        equipButton.onClick.AddListener(EquipOrUnequipSelectedItem);
        //moveButton.onClick.AddListener(TryMoveOrSwap);
        removeItemButton.onClick.AddListener(DestroySelectedItem);
        useButton.onClick.AddListener(UseSelectedItem);
        lockAndUnlockButton.onClick.AddListener(LockOrUnlock);

        increaseAmountButton.onClick.AddListener(IncreaseAmountToDelete);
        decreaseAmountButton.onClick.AddListener(DecreaseAmountToDelete);

        equipFirstAbilityButton.onClick.AddListener(EquipToFirstSlot);
        equipSecondAbilityButton.onClick.AddListener(EquipToSecondSlot);

        DisableAllButtons();
        //ShowChoices(false);
    }

    // Called when an ItemSlot is clicked
    public void HandleSlotSelection(ItemSlot selectedSlot)
    {
        if (selectedSlot == currentSelectedSlot)
        {
            DisableAllButtons();
            //ShowChoices(false);
            selectedSlot.DeselectItem();
            currentSelectedSlot = null;
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
        DisableAllButtons();
        //ShowChoices(true);
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
            //moveButton.gameObject.SetActive(true);

            if (item.isEquippable && charDetailsHolder.gameObject.activeInHierarchy)
            {
                TextMeshProUGUI equipFirstText = equipFirstAbilityButton.GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI equipSecondText = equipSecondAbilityButton.GetComponentInChildren<TextMeshProUGUI>();
                equipFirstText.text = "Equip to First Slot";
                equipSecondText.text = "Equip to Second Slot";

                equipFirstAbilityButton.gameObject.SetActive(true);
                equipSecondAbilityButton.gameObject.SetActive(true);
                

                if (selectedSlot == agimatFirstSlot)
                {
                    //moveButton.gameObject.SetActive(false);
                    equipFirstText.text = "Unequip";
                }

                if (selectedSlot == agimatSecondSlot)
                {
                    //moveButton.gameObject.SetActive(false);
                    equipSecondText.text = "Unequip";
                }
            }
            //removeItemButton.interactable = true;
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

        DisableAllButtons();
        //ShowChoices(false);
    }

    public void EquipToSecondSlot()
    {
        InventoryItem item = currentSelectedSlot.GetComponentInChildren<InventoryItem>();
        IEquippable equippable = item.GetComponent<IEquippable>();

        if (!equippable.isEquipped || currentSelectedSlot != agimatSecondSlot)
            equippable.Equip(agimatSecondSlot);
        else
            equippable.Unequip();

        DisableAllButtons();
        //ShowChoices(false);
    }

    private void ItemAny(ItemSlot selectedSlot)
    {
        IEquippable itemEquipped = selectedSlot.GetComponentInChildren<IEquippable>();
        if (itemEquipped != null && itemEquipped.isEquipped && charDetailsHolder.gameObject.activeInHierarchy)
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
            if (item.isEquippable && charDetailsHolder.gameObject.activeInHierarchy)
            {
                TextMeshProUGUI equipText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                equipText.text = "Equip";

                equipButton.gameObject.SetActive(true);
            }
            //moveButton.gameObject.SetActive(true);
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
                //ShowChoices(false);

                if (movingFromSlot.isSlotLocked || currentSelectedSlot.isSlotLocked)//if the slot is locked
                {
                    movingFromSlot.GetComponent<Image>().color = Color.yellow;
                }
                return;

            }

            Inventory inventory = WhichInventory(movingFromSlot.GetComponentInChildren<InventoryItem>());

            SwapItemsAnimated(movingFromSlot, currentSelectedSlot);

            //Temporary
            movingFromSlot.GetComponent<Image>().color = Color.black;

            isMoving = false;
            movingFromSlot = null;

            DisableAllButtons();
            //ShowChoices(false);
        }
    }

    // Called by "Remove" button
    public void DestroySelectedItem()
    {       
        toDelete = currentSelectedSlot;
        amountToRemove = 1;
        amountToRemoveText.text = amountToRemove.ToString();
        itemName.text = toDelete.GetComponentInChildren<InventoryItem>().itemName;
        removeItemConfirmationPanel.SetActive(true);

        DisableAllButtons();
        //ShowChoices(false);
    }

    public void IncreaseAmountToDelete()
    {
        int maxQuantity = toDelete.GetComponentInChildren<InventoryItem>().quantity;

        if (amountToRemove < maxQuantity)
        {
            amountToRemove++;
            amountToRemoveText.text = amountToRemove.ToString();
        }
        else return;
    }

    public void DecreaseAmountToDelete()
    {
        int minQuantity = 1;

        if (amountToRemove > minQuantity)
        {
            amountToRemove--;
            amountToRemoveText.text = amountToRemove.ToString();
        }
        else return;
    }

    public void RemoveItem()
    {
        InventoryItem item = toDelete.GetComponentInChildren<InventoryItem>();

        item.quantity -= amountToRemove;
        item.SetQuantity(item.quantity);

        Inventory inventory = WhichInventory(item);

        if (inventory == null) return;

        if (item.quantity <= 0)
        {
            if (item.itemType == ItemType.Agimat)
            {
                if (!InventoryManager.Instance.agimatItems.Contains(toDelete.currentItem))
                {
                    Debug.Log($"{item.name} does not exists in manager");
                }
                InventoryManager.Instance.agimatItems.Remove(toDelete.currentItem);
                InventoryManager.Instance.agimatInventorySlots.Remove(toDelete);
            }
            else
            {
                if (!InventoryManager.Instance.mainInventoryItems.Contains(toDelete.currentItem))
                {
                    Debug.Log($"{item.name} does not exists in manager");
                }
                InventoryManager.Instance.mainInventoryItems.Remove(toDelete.currentItem);
                InventoryManager.Instance.mainInventorySlots.Remove(toDelete);
            }

            if (inventory.slots.Contains(toDelete) && inventory.items.Contains(item))
            {
                inventory.slots.Remove(toDelete);
                inventory.items.Remove(item);
                Destroy(toDelete.gameObject);
            }
        }
        
        amountToRemove = 1;
        removeItemConfirmationPanel.SetActive(false);
    }

    public void RemoveSpecificItem(string itemID , int amount)
    {
        Inventory inventory = GameObject.Find("Main Inventory").GetComponent<Inventory>();
        if(inventory == null) return;

        foreach (ItemSlot itemSlot in inventory.slots)
        {
            InventoryItem item = itemSlot.GetComponentInChildren<InventoryItem>();
            if (item != null)
            {
                if (item.itemID == itemID)
                {
                    toDelete = itemSlot;
                    amountToRemove = amount;
                    RemoveItem();
                }
            }
        }      
    }

    // Called by "Use" button
    public void UseSelectedItem()
    {
        InventoryItem item = currentSelectedSlot.GetComponentInChildren<InventoryItem>();
        IUsable usable = item.GetComponent<IUsable>();

        usable.Use();

        DisableAllButtons();
        //ShowChoices(false);
    }

    // Equip/Unequip could later be expanded for your equipment system
    public void EquipOrUnequipSelectedItem()
    {
        InventoryItem item = currentSelectedSlot.GetComponentInChildren<InventoryItem>();
        IEquippable equippable = item.GetComponent<IEquippable>();

        if (equippable.isEquipped == false)
            equippable.Equip();
        else
            equippable.Unequip();

        DisableAllButtons();
        //ShowChoices(false);
    }

    public void LockOrUnlock()
    {
        currentSelectedSlot.isSlotLocked = !currentSelectedSlot.isSlotLocked;
        Debug.Log($"Is this slot locked?: {currentSelectedSlot.isSlotLocked}");

        TextMeshProUGUI buttonText = lockAndUnlockButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = currentSelectedSlot.isSlotLocked ? "Unlock" : "Lock";

        currentSelectedSlot.SaveLockedState();

        DisableAllButtons();
        //ShowChoices(false);
        movingFromSlot = null;
        currentSelectedSlot = null;
    }

    public void DisableAllButtons()
    {
        equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip/Unequip";
        equipFirstAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip to First Slot";
        equipSecondAbilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip to Second Slot";
        lockAndUnlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Lock/Unlock";

        equipButton.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);
        removeItemButton.gameObject.SetActive(false);

        equipFirstAbilityButton.gameObject.SetActive(false);
        equipSecondAbilityButton.gameObject.SetActive(false);
        //ShowChoices(false);
    }

    /*public void ShowChoices(bool isShow)
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
            itemActionControllerGameObject.SetActive(true);

            GameObject slotObj = currentSelectedSlot.gameObject;

            // Get screen position of the slot
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, slotObj.transform.position);

            // Adjust based on screen height
            float offsetY = 50f;
            float screenHeight = Screen.height;

            RectTransform rectTransform = itemActionControllerGameObject.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 0);

            if (screenPos.y > screenHeight * 0.5f)
            {
                offsetY = -50f; // move upward if slot is in lower part of screen
                rectTransform.pivot = new Vector2(0.5f, 1);
            }

            // Final position
            Vector2 adjustedScreenPos = new Vector2(screenPos.x, screenPos.y + offsetY);

            // Convert screen space to local position in the canvas
            RectTransform canvasRect = itemActionControllerGameObject.transform.parent.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, adjustedScreenPos, null, out Vector2 localPoint);

            itemActionControllerGameObject.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
        else
        {
            removeItemButton.gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);


            itemActionControllerGameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            currentSelectedSlot = null;
        }
    }*/
    public List<ItemSlot> WhichSlotList(InventoryItem item)
    {
        if (item.itemType != ItemType.Agimat)
            return InventoryManager.Instance.mainInventorySlots;
        else
            return InventoryManager.Instance.agimatInventorySlots;
    }

    public Inventory WhichInventory(InventoryItem item)
    {
        if (item.itemType != ItemType.Agimat)
            return GameObject.Find("Main Inventory").GetComponent<Inventory>();
        else
            return GameObject.Find("Agimat Inventory").GetComponent<Inventory>();
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