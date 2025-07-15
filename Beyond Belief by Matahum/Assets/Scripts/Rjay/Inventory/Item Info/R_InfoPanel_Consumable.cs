using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_InfoPanel_Consumable : R_ItemInfoDisplay
{
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private Image backdropImage;          // Background frame behind icon
    [SerializeField] private Image iconImage;
    [SerializeField] private Image headerImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject useButton;

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
        useButton.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
