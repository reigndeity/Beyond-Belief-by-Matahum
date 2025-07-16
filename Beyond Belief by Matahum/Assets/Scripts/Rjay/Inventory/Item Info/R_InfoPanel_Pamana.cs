using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class R_InfoPanel_Pamana : R_ItemInfoDisplay
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelStatTxt;
    [SerializeField] private TextMeshProUGUI itemTypeTxt;
    [SerializeField] private TextMeshProUGUI mainStatTxt;
    [SerializeField] private TextMeshProUGUI mainStatValueTxt;
    [SerializeField] private TextMeshProUGUI subStatTxt;
    [SerializeField] private TextMeshProUGUI setPieceName;
    [SerializeField] private TextMeshProUGUI twoPieceSet;
    [SerializeField] private TextMeshProUGUI threePieceSet;

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        
        nameText.text = itemData.itemName;
        itemTypeTxt.text = itemData.pamanaSlot.ToString();
        descriptionText.text = itemData.description;
        setPieceName.text = FormatSetName(itemData.set);
        twoPieceSet.text = $"2-Piece Set: {itemData.twoPieceBonusDescription}";
        threePieceSet.text = $"3-Piece Set: {itemData.threePieceBonusDescription}";

        if (itemData.pamanaData != null)
        {
            var pamana = itemData.pamanaData;

            levelStatTxt.text = $"+{pamana.currentLevel}";

            mainStatTxt.text = FormatStatName(pamana.mainStatType);
            mainStatValueTxt.text = FormatStatValue(pamana.mainStatType, pamana.mainStatValue);

            if (pamana.substats != null && pamana.substats.Count > 0)
            {
                string subStatText = "";
                foreach (var sub in pamana.substats)
                {
                    subStatText += $"• {FormatStat(sub.statType, sub.value)}\n";
                }
                subStatTxt.text = subStatText.TrimEnd();
            }
            else
            {
                subStatTxt.text = "";
            }
        }
        else
        {
            levelStatTxt.text = "+0";
            mainStatTxt.text = "";
            mainStatValueTxt.text = "";
            subStatTxt.text = "";
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
        // Insert space before uppercase letters (excluding first character)
        return Regex.Replace(set.ToString(), "(?<=.)([A-Z])", " $1");
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
