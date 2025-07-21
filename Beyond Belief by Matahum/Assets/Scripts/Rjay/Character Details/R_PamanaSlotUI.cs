using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_PamanaSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject equippedLabel;

    private R_InventoryItem representedItem;
    private R_PamanaPanel parentPanel;

    private Vector3 defaultScale = Vector3.one;
    private Vector3 selectedScale = Vector3.one * 1.1f;

    public void Setup(R_InventoryItem item, R_PamanaPanel panel)
    {
        representedItem = item;
        parentPanel = panel;

        if (item != null && item.itemData != null)
        {
            // Icon
            iconImage.sprite = item.itemData.itemIcon;
            iconImage.enabled = true;

            // Backdrop
            backdropImage.sprite = item.itemData.itemBackdropIcon;
            backdropImage.enabled = backdropImage.sprite != null;

            // Level Display
            int level = item.itemData.pamanaData?.currentLevel ?? 0;
            levelText.text = $"+{level}";

            // Equipped Label Logic
            bool isEquipped = panel.IsItemEquipped(item);
            equippedLabel.SetActive(isEquipped);
        }

        // Selection Button
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => parentPanel.OnPamanaSelected(representedItem));
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
