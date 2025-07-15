using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_InfoPanel_Agimat : R_ItemInfoDisplay
{
    [Header("General Info")]
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private Image headerImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Slot 1")]
    [SerializeField] private TextMeshProUGUI slot1Name;
    [SerializeField] private TextMeshProUGUI slot1Description;

    [Header("Slot 2")]
    [SerializeField] private TextMeshProUGUI slot2Name;
    [SerializeField] private TextMeshProUGUI slot2Description;

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        // General
        headerImage.sprite = itemData.inventoryHeaderImage;
        headerImage.enabled = itemData.inventoryHeaderImage != null;

        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        itemType.text = itemData.itemType.ToString();
        nameText.text = itemData.itemName;
        descriptionText.text = itemData.description;

        // Slot 1
        if (itemData.slot1Ability != null)
        {
            slot1Name.text = $"{itemData.slot1Ability.abilityName} {(itemData.slot1Ability.isPassive ? "(Passive)" : "(Active)")}";
            slot1Description.text = itemData.slot1Ability.description;
        }
        else
        {
            slot1Name.text = "No Slot 1 Ability";
            slot1Description.text = "";
        }

        // Slot 2
        if (itemData.slot2Ability != null)
        {
            slot2Name.text = $"{itemData.slot2Ability.abilityName} {(itemData.slot2Ability.isPassive ? "(Passive)" : "(Active)")}";
            slot2Description.text = itemData.slot2Ability.description;
        }
        else
        {
            slot2Name.text = "No Slot 2 Ability";
            slot2Description.text = "";
        }
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
