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

    private R_InventoryItem representedItem;
    private R_PamanaPanel parentPanel;

    public void Setup(R_InventoryItem item, R_PamanaPanel panel)
    {
        representedItem = item;
        parentPanel = panel;

        if (item != null && item.itemData != null)
        {
            iconImage.sprite = item.itemData.itemIcon;
            iconImage.enabled = true;

            backdropImage.sprite = item.itemData.inventoryBackdropImage;
            backdropImage.enabled = backdropImage.sprite != null;

            levelText.text = $"+{item.itemData.pamanaData?.currentLevel ?? 0}";
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => parentPanel.OnPamanaSelected(representedItem));
    }
}
