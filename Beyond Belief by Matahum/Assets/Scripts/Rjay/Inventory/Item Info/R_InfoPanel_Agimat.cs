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

    [Header("Condensed Body")]
    [SerializeField] private TextMeshProUGUI agimatCondensedBodyText;

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

        // Condensed body block
        string slot1Header = $"<size=36>Slot 1: {itemData.slot1Ability?.abilityName ?? "None"}</size>\n";
        string slot1Desc = itemData.slot1Ability != null
            ? $"<size=30> •{(itemData.slot1Ability.isPassive ? "Passive" : "Active")}: {itemData.slot1Ability.description}</size>\n"
            : "";

        string slot2Header = $"<size=36>Slot 2: {itemData.slot2Ability?.abilityName ?? "None"}</size>\n";
        string slot2Desc = itemData.slot2Ability != null
            ? $"<size=30> •{(itemData.slot2Ability.isPassive ? "Passive" : "Active")}: {itemData.slot2Ability.description}</size>\n"
            : "";

        string desc = $"<size=36>{itemData.description}</size>";

        agimatCondensedBodyText.text = slot1Header + slot1Desc + "\n" + slot2Header + slot2Desc + "\n\n" + desc;
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
