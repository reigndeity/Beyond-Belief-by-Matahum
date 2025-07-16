using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class R_InfoPanel_Consumable : R_ItemInfoDisplay
{
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private Image backdropImage;          // Background frame behind icon
    [SerializeField] private Image iconImage;
    [SerializeField] private Image headerImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button useButton;

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        // Set icon
        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        // Set backdrop
        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        // 🔹 Set header image
        if (headerImage != null)
        {
            headerImage.sprite = itemData.inventoryHeaderImage;
            headerImage.enabled = itemData.inventoryHeaderImage != null;
        }

        // Set text
        itemType.text = itemData.itemType.ToString();
        nameText.text = itemData.itemName;
        descriptionText.text = itemData.description;

        // Use button visible only for Consumables
        useButton.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetUseButtonCallback(R_InventoryItem item, R_ItemActionPrompt prompt, Action refreshCallback)
    {
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() => {
            prompt.Open(item, R_ActionType.Use, amount => {
                // TODO: Add healing or effect logic
                item.quantity -= amount;
                if (item.quantity <= 0)
                {
                    // Optional: mark for removal here, or RefreshUI will clean up
                }
                refreshCallback.Invoke();
            });
        });
    }
}
