using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ItemSlot : MonoBehaviour
{
    [Header("Slot Settings")]
    public bool isAgimatSlot = false;                    // Only accepts Agimat if true
    public ItemType allowedType = ItemType.Any;         // Optional filtering if not agimat
    public bool isSlotLocked = false;

    [Header("References")]
    public InventoryItem currentItem;
    private Button thisItemButton;
    public TextMeshProUGUI quantityText;

    [Header("Is this Slot an Equipment Slot?")]
    public bool isEquipmentSlot = false;

    public event Action<ItemSlot, InventoryItem, InventoryItem> OnItemChanged;

    public bool IsSelected { get; private set; }

    private void Awake()
    {
        thisItemButton = GetComponent<Button>();
    }
    private void Start()
    {
        DeselectItem();
        thisItemButton.onClick.AddListener(HandleClick);

        currentItem = GetComponentInChildren<InventoryItem>();
        SetItem(currentItem);

        LoadLockedState();
    }

    private void HandleClick()
    {
        //This makes the button clickable again
        //EventSystem.current.SetSelectedGameObject(null);

        ItemActionController.Instance?.HandleSlotSelection(this);
        Debug.Log($"Selected {gameObject.name}");
    }

    public void SaveLockedState()
    {
        PlayerPrefs.SetInt("SlotLock_" + name, isSlotLocked ? 1 : 0);

        Image img = GetComponent<Image>();
        if (img != null)
        {
            if (isSlotLocked)
            {
                img.color = Color.yellow;
            }
            else
            {
                img.color = Color.black;
            }
        }
    }
    public void LoadLockedState()
    {
        isSlotLocked = PlayerPrefs.GetInt("SlotLock_" + name, 0) == 1;

        Image img = GetComponent<Image>();
        if (img != null)
        {
            if (isSlotLocked)
            {
                img.color = Color.yellow;
            }
            else
            {
                img.color = Color.black;
            }
        }
    }

    public void SetItem(InventoryItem newItem)
    {
        if (newItem == null)
        {
            currentItem = null;
            quantityText.text = "";
            return;
        }

        if (!IsValidForSlot(newItem))
        {
            Debug.Log($"{currentItem} is not valid for slot");
            return;
        }


        currentItem = newItem;
        currentItem.transform.SetParent(transform);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        currentItem.OnQuantityChanged += UpdateQuantityText;
        UpdateQuantityText(); // Update once immediately

        OnItemChanged?.Invoke(this, null, currentItem);
    }

    public void UpdateQuantityText()
    {
        if (currentItem == null)
        {
            quantityText.text = "";
            return;
        }

        if (currentItem.isStackable)
            quantityText.text = currentItem.quantity.ToString();
        else
            quantityText.text = "";

        quantityText.gameObject.transform.SetAsLastSibling();
    }


    public void ClearItem()
    {
        if (currentItem != null)
            currentItem.OnQuantityChanged -= UpdateQuantityText;

        OnItemChanged?.Invoke(this, currentItem, null);

        currentItem = null;
        quantityText.text = "";
        DeselectItem();

    }


    public bool HasItem()
    {
        return currentItem != null;
    }

    public bool IsValidForSlot(InventoryItem item)
    {
        if (item == null) return false;

        if (isAgimatSlot)
        {
            // Slot is specifically for Agimat
            return item.itemType == ItemType.Agimat;
        }
        else
        {
            if (allowedType == ItemType.Any)
            {
                // Accept any type EXCEPT Agimat
                return item.itemType != ItemType.Agimat;
            }
            else
            {
                // Only accept items that match the allowed type
                return item.itemType == allowedType;
            }
        }
    }


    public void SelectItem()
    {
        IsSelected = true;
    }

    public void DeselectItem()
    {
        IsSelected = false;
        EventSystem.current.SetSelectedGameObject(null);
    }
}
