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

        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        headerImage.sprite = itemData.inventoryHeaderImage;
        headerImage.enabled = itemData.inventoryHeaderImage != null;

        itemType.text = itemData.itemType.ToString();
        nameText.text = itemData.itemName;

        // 🔹 Use clean formatter methods
        string slot1Text = FormatAbilityBlock("Slot 1", itemData.slot1Ability, itemData.rarity);
        string slot2Text = FormatAbilityBlock("Slot 2", itemData.slot2Ability, itemData.rarity);
        string descriptionBlock = $"<size=36>{itemData.description}</size>";

        agimatCondensedBodyText.text = slot1Text + "\n" + slot2Text + "\n\n" + descriptionBlock;
    }

    private string FormatAbilityBlock(string slotLabel, R_AgimatAbility ability, R_ItemRarity rarity)
    {
        if (ability == null)
            return $"<size=36>{slotLabel}: None</size>";

        string typeLabel = ability.isPassive ? "Passive" : "Active";
        string description = ability.GetDescription(rarity);

        return $"<size=36>{slotLabel}: {ability.abilityName}</size>\n" +
            $"<size=30>• {typeLabel}: {description}</size>";
    }


    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
