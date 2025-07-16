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
    [SerializeField] private TextMeshProUGUI condensedBodyText;
    [SerializeField] private Button useButton;
    public R_Inventory playerInventory;

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        // Set icon
        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        // Set backdrop
        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        // Header image
        if (headerImage != null)
        {
            headerImage.sprite = itemData.inventoryHeaderImage;
            headerImage.enabled = itemData.inventoryHeaderImage != null;
        }

        // Labels
        itemType.text = itemData.itemType.ToString();
        nameText.text = itemData.itemName;

        // 🔹 Build condensed body (effectText + description)
        string effectBlock = $"<size=36> •{itemData.effectText}</size>\n\n";
        string descBlock = $"<size=36>{itemData.description}</size>";

        condensedBodyText.text = effectBlock + descBlock;

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
                GameObject user = GameObject.FindWithTag("Player");

                if (item.itemData.consumableEffect != null)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        item.itemData.consumableEffect.Apply(user);
                    }
                }

                if (playerInventory != null)
                {
                    playerInventory.RemoveItem(item.itemData, amount);
                }

                refreshCallback.Invoke();
            });
        });
    }

}
