using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image backdrop; // 🔁 NEW
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private R_InventoryItem currentItem;
    private R_InventoryItemInfoPanel infoPanel; // This should be at the top of your script

    private Button inventoryItemButton;
    void Awake()
    {
        inventoryItemButton.GetComponent<Button>();
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
            // Set backdrop if available
            if (backdrop != null)
            {
                backdrop.sprite = item.itemData.itemBackdropIcon;
                backdrop.enabled = item.itemData.itemBackdropIcon != null;
            }

            icon.sprite = item.itemData.itemIcon;
            icon.enabled = true;

            quantityText.text = (item.itemData.isStackable && item.quantity > 1)
                ? item.quantity.ToString()
                : "";
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
    }
    public void OnClickSlot()
    {
        if (currentItem != null && currentItem.itemData != null)
        {
            infoPanel.ShowItem(currentItem.itemData);
        }
    }
}
