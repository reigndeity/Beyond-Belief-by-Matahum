using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class R_InfoPanel_Pamana : R_ItemInfoDisplay
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backdropImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelStatTxt;
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
        descriptionText.text = itemData.description;
        setPieceName.text = itemData.set.ToString();
        twoPieceSet.text = $"2-Piece Set: {itemData.twoPieceBonusDescription}";
        threePieceSet.text = $"3-Piece Set: {itemData.threePieceBonusDescription}";

        // 🔸 Pamana-specific
        if (itemData.pamanaData != null)
        {
            var pamana = itemData.pamanaData;

            // Level
            levelStatTxt.text = $"+{pamana.currentLevel}";

            // Main stat
            //string formattedMain = FormatStat(pamana.mainStatType, pamana.mainStatValue);
            mainStatTxt.text = $"{pamana.mainStatType}";
            mainStatValueTxt.text = $"{pamana.mainStatValue}";

            // Substats
            if (pamana.substats != null && pamana.substats.Count > 0)
            {
                string subStatText = "";
                foreach (var sub in pamana.substats)
                {
                    subStatText += $"• {FormatStat(sub.statType, sub.value)}\n\n";
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
            levelStatTxt.text = "0+";
            mainStatTxt.text = "";
            subStatTxt.text = "";
        }
    }

    private string FormatStat(R_StatType statType, float value)
    {
        // Use "%" for percent-based stats
        bool isPercent = statType.ToString().Contains("Percent") || statType == R_StatType.CRITRate || statType == R_StatType.CRITDamage;
        string valueString = isPercent ? $"{value}%" : $"{value}";
        return $"{statType}: +{valueString}";
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
