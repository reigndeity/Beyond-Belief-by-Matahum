using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image backdrop;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private R_InventoryItem currentItem;
    private R_InventoryItemInfoPanel infoPanel;

    [SerializeField] private Button inventoryItemButton;
    public R_InventoryUI inventoryUI;

    private Vector3 defaultScale = Vector3.one;
    private Vector3 selectedScale = Vector3.one * 1.1f;

    [SerializeField] private GameObject equippedLabel;  // 🔹 Add this field
    private Player player;


    void Awake()
    {
        inventoryItemButton.GetComponent<Button>();
        player = FindFirstObjectByType<Player>();

    }

    void Start()
    {
        inventoryItemButton.onClick.AddListener(OnClickSlot);
    }

    public void Initialize(R_InventoryItemInfoPanel panel)
    {
        infoPanel = panel;
    }

    public void SetSlot(R_InventoryItem item)
    {
        currentItem = item;

        if (item != null && item.itemData != null)
        {
            // Set backdrop
            if (backdrop != null)
            {
                backdrop.sprite = item.itemData.itemBackdropIcon;
                backdrop.enabled = item.itemData.itemBackdropIcon != null;
            }

            icon.sprite = item.itemData.itemIcon;
            icon.enabled = true;

            // ✅ Final Display Logic
            if (item.itemData.itemType == R_ItemType.Pamana)
            {
                quantityText.text = $"+{item.itemData.pamanaData.currentLevel}";
            }
            else if (item.itemData.itemType == R_ItemType.Agimat)
            {
                quantityText.text = "1";
            }
            else if (item.itemData.isStackable)
            {
                quantityText.text = item.quantity.ToString();
            }
            else
            {
                quantityText.text = "1";
            }
        }
        else
        {
            if (backdrop != null)
            {
                backdrop.sprite = null;
                backdrop.enabled = false;
            }

            icon.sprite = null;
            icon.enabled = false;
            quantityText.text = "";
        }

        if (equippedLabel != null)
        {
            bool isEquipped = false;

            if (item != null && item.itemData != null && item.itemData.itemType == R_ItemType.Pamana && player != null)
            {
                isEquipped = player.IsPamanaEquipped(item);
            }
            if (item != null && item.itemData != null && item.itemData.itemType == R_ItemType.Agimat && player != null)
            {
                isEquipped = player.IsAgimatEquipped(item);
            }

            equippedLabel.SetActive(isEquipped);
        }


    }

    public void OnClickSlot()
    {
        if (currentItem != null && currentItem.itemData != null)
        {
            infoPanel.ShowItem(currentItem.itemData);
            if (inventoryUI != null)
                inventoryUI.SetSelectedItem(currentItem);
        }
    }

    public void SetSelected(bool isSelected)
    {
        transform.localScale = isSelected ? selectedScale : defaultScale;
    }

    public bool RepresentsItem(R_InventoryItem item)
    {
        return currentItem == item;
    }

}

