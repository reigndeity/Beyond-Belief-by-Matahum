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

    public override void Show(R_ItemData itemData)
    {
        gameObject.SetActive(true);

        iconImage.sprite = itemData.itemIcon;
        iconImage.enabled = itemData.itemIcon != null;

        backdropImage.sprite = itemData.inventoryBackdropImage;
        backdropImage.enabled = itemData.inventoryBackdropImage != null;

        nameText.text = itemData.itemName;
        descriptionText.text = itemData.description;

        // 🔸 Pamana-specific
        if (itemData.pamanaData != null)
        {
            var pamana = itemData.pamanaData;

            // Level
            levelStatTxt.text = $"Lv. {pamana.currentLevel}";

            // Main stat
            string formattedMain = FormatStat(pamana.mainStatType, pamana.mainStatValue);
            mainStatTxt.text = $"Main Stat: {formattedMain}";

            // Substats
            if (pamana.substats != null && pamana.substats.Count > 0)
            {
                string subStatText = "";
                foreach (var sub in pamana.substats)
                {
                    subStatText += $"+{FormatStat(sub.statType, sub.value)}\n";
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
