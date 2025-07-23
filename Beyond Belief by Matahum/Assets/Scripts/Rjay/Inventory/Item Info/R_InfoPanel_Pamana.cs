using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using Febucci.UI;

public class R_InfoPanel_Pamana : R_ItemInfoDisplay
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image headerImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private TextMeshProUGUI nameText;
    //[SerializeField] private TextMeshProUGUI descriptionText; // Can be removed if unused elsewhere
    [SerializeField] private TextMeshProUGUI levelStatTxt;
    [SerializeField] private TextMeshProUGUI itemTypeTxt;
    [SerializeField] private TextMeshProUGUI mainStatTxt;
    [SerializeField] private TextMeshProUGUI mainStatValueTxt;

    [Header("Condensed Body Panel")]
    [SerializeField] private TextMeshProUGUI pamanaCondensedBodyText;

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        if (headerImage != null)
        {
            headerImage.sprite = itemData.inventoryHeaderImage;
            headerImage.enabled = itemData.inventoryHeaderImage != null;
        }

        nameText.text = itemData.itemName;
        itemTypeTxt.text = FormatEnumName(itemData.pamanaSlot.ToString());

        if (itemData.pamanaData != null)
        {
            var pamana = itemData.pamanaData;

            levelStatTxt.text = $"+{pamana.currentLevel}";
            mainStatTxt.text = FormatStatName(pamana.mainStatType);
            mainStatValueTxt.text = FormatStatValue(pamana.mainStatType, pamana.mainStatValue);

            // 🧠 Determine if set bonuses are active
            int equippedCount = 0;
            var player = FindFirstObjectByType<Player>();
            var equipped = player.GetEquippedPamanas();
            foreach (var kvp in equipped)
            {
                var equippedPamana = kvp.Value?.itemData?.pamanaData;
                if (equippedPamana != null && equippedPamana.set == pamana.set)
                    equippedCount++;
            }

            

            // ✨ Apply glow to bonuses if active
            string twoPieceBonus = itemData.twoPieceBonusDescription;
            string threePieceBonus = itemData.threePieceBonusDescription;
            string twoPieceLine = $" 2-Piece Set: {twoPieceBonus}";
            string threePieceLine = $" 3-Piece Set: {threePieceBonus}";

            if (equippedCount >= 2)
                twoPieceBonus = $"<wave><color=#FFA500>{twoPieceBonus}</color></wave>";
            if (equippedCount >= 3)
                threePieceBonus = $"<wave><color=#FFA500>{threePieceBonus}</color></wave>";

            if (equippedCount >= 2)
                twoPieceLine = $"<wave><color=#FFA500>{twoPieceLine}</color></wave>";
            if (equippedCount >= 3)
                threePieceLine = $"<wave><color=#FFA500>{threePieceLine}</color></wave>";


            // Build substat block
            string substatBlock = "";
            foreach (var sub in pamana.substats)
            {
                substatBlock += $" •{FormatStat(sub.statType, sub.value)}\n";
            }

            // Build final body text
            string bodyText =
                $"<size=36><color=#000000>{substatBlock}</color></size>\n" +
                $"<size=36><color=green>{FormatSetName(itemData.set)}</color></size>\n" +
                $"<size=30><color=#000000>{twoPieceLine}</color>\n" +
                $"<color=#000000>{threePieceLine}</color></size>\n\n" +
                $"<size=36><color=#000000>{itemData.description}</color></size>";

            pamanaCondensedBodyText.text = bodyText;
        }
        else
        {
            levelStatTxt.text = "+0";
            mainStatTxt.text = "";
            mainStatValueTxt.text = "";
            pamanaCondensedBodyText.text = "";
        }
    }


    private string FormatStat(R_StatType statType, float value)
    {
        string statName = FormatStatName(statType);
        string valueText = FormatStatValue(statType, value);
        return $"{statName} {valueText}";
    }

    private string FormatStatName(R_StatType statType)
    {
        return statType switch
        {
            R_StatType.CRITRate => "CRIT Rate",
            R_StatType.CRITDamage => "CRIT DMG",
            R_StatType.FlatATK => "ATK",
            R_StatType.FlatDEF => "DEF",
            R_StatType.FlatHP => "HP",
            R_StatType.PercentATK => "ATK",
            R_StatType.PercentDEF => "DEF",
            R_StatType.PercentHP => "HP",
            R_StatType.CooldownReduction => "Cooldown Reduction",
            _ => statType.ToString()
        };
    }

    private string FormatStatValue(R_StatType statType, float value)
    {
        bool isPercent = statType.ToString().Contains("Percent") ||
                         statType == R_StatType.CRITRate ||
                         statType == R_StatType.CRITDamage;

        return isPercent ? $"+{value:F1}%" : $"+{value}";
    }

    private string FormatSetName(R_PamanaSet set)
    {
        return Regex.Replace(set.ToString(), "(?<=.)([A-Z])", " $1");
    }

    private string FormatEnumName(string rawName)
    {
        return Regex.Replace(rawName, "(?<=.)([A-Z])", " $1");
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
