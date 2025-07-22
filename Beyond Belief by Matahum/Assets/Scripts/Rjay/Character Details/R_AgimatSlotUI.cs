using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_AgimatSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private GameObject equippedLabel;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI quantityText;

    private R_InventoryItem representedItem;
    private R_AgimatPanel parentPanel;
    private Player player;

    private Vector3 defaultScale = Vector3.one;
    private Vector3 selectedScale = Vector3.one * 1.1f;

    public void Setup(R_InventoryItem item, R_AgimatPanel panel)
    {
        representedItem = item;
        parentPanel = panel;
        player = FindFirstObjectByType<Player>();

        if (item != null && item.itemData != null)
        {
            // Icon
            iconImage.sprite = item.itemData.itemIcon;
            iconImage.enabled = true;

            // Quantity
            quantityText.text = "1";

            // Backdrop
            backdropImage.sprite = item.itemData.itemBackdropIcon;
            backdropImage.enabled = backdropImage.sprite != null;

            // Equipped label logic
            bool isEquipped = player != null && player.IsAgimatEquipped(item);
            equippedLabel.SetActive(isEquipped);
        }

        // Selection logic
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => parentPanel.OnAgimatSelected(representedItem));
    }

    public void SetSelected(bool isSelected)
    {
        transform.localScale = isSelected ? selectedScale : defaultScale;
    }

    public bool RepresentsItem(R_InventoryItem item)
    {
        return representedItem == item;
    }
}
