using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene_ToolTips : MonoBehaviour
{
    //0 = Normal Tool Tip, 1 = Nuno Tool Tip, 2 = Mangkukulam Tool Tip
    public ToolTipGroups[] toolTipsGroup;
    private TextMeshProUGUI toolTipText;

    private void Start()
    {
        ShowToolTip();
    }
    public void ShowToolTip()
    {
        toolTipText = GetComponent<TextMeshProUGUI>();

        int tipGroup = Mathf.Clamp(PlayerPrefs.GetInt("ToolTipGroup", 0), 0, toolTipsGroup.Length);

        int randomizer = UnityEngine.Random.Range(0, toolTipsGroup[tipGroup].toolTips.Length);
        toolTipText.text = /*$"<color=yellow><b>Tool Tip:</b></color>*/$"{toolTipsGroup[tipGroup].toolTips[randomizer]}";
    }
}

[Serializable]
public class ToolTipGroups
{
    public string toolTipGroupName;
    [TextArea(1, 3)]
    public string[] toolTips;
}