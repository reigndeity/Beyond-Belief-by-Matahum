using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_InfoPanel_QuestItem : R_ItemInfoDisplay
{
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private Image headerImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject useButton;

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        // 🔹 Set header image
        if (headerImage != null)
        {
            headerImage.sprite = itemData.inventoryHeaderImage;
            headerImage.enabled = itemData.inventoryHeaderImage != null;
        }

        nameText.text = itemData.itemName;
        descriptionText.text = itemData.description;
        itemType.text = "Quest Item";

        useButton.SetActive(itemData.isConsumable); // Show only if it's a consumable quest item
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
