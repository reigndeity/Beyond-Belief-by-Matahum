using UnityEngine;
using System.Collections.Generic;

public class R_InventoryItemInfoPanel : MonoBehaviour
{
    [SerializeField] private R_ItemInfoDisplay consumablePanel;
    [SerializeField] private R_ItemInfoDisplay questItemPanel;
    [SerializeField] private R_ItemInfoDisplay pamanaPanel;
    [SerializeField] private R_ItemInfoDisplay agimatPanel;
    [SerializeField] private R_ItemInfoDisplay upgradeMaterialPanel;
    [SerializeField] private R_ItemInfoDisplay ingredientPanel;

    private List<R_ItemInfoDisplay> allPanels;

    private void Awake()
    {
        allPanels = new List<R_ItemInfoDisplay>
        {
            consumablePanel, questItemPanel, pamanaPanel,
            agimatPanel, upgradeMaterialPanel, ingredientPanel
        };

        HideAll();
    }

    public void ShowItem(R_ItemData itemData)
    {
        HideAll();

        if (itemData == null) return;

        switch (itemData.itemType)
        {
            case R_ItemType.Consumable:
                consumablePanel.Show(itemData);
                break;
            case R_ItemType.QuestItem:
                questItemPanel.Show(itemData);
                break;
            case R_ItemType.Pamana:
                pamanaPanel.Show(itemData);
                break;
            case R_ItemType.Agimat:
                agimatPanel.Show(itemData);
                break;
            case R_ItemType.UpgradeMaterial:
                upgradeMaterialPanel.Show(itemData);
                break;
            case R_ItemType.Ingredient:
                ingredientPanel.Show(itemData);
                break;
        }
    }

    public void HideAll()
    {
        foreach (var panel in allPanels)
            panel.Hide();
    }

    public void ClearPanel() => HideAll();
}
