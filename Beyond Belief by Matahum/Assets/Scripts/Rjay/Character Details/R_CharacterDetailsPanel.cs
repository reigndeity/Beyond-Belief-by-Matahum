using UnityEngine;

public enum CharacterDetailsTab
{
    Attribute,
    Weapon,
    Agimat,
    Pamana
}

public class R_CharacterDetailsPanel : MonoBehaviour
{
    [Header("Tab Panels")]
    [SerializeField] private GameObject attributePanel;
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private GameObject agimatPanel;
    [SerializeField] private GameObject pamanaPanel;

    private CharacterDetailsTab currentTab;

    private void Start()
    {
        // Optional: default to Attribute panel on open
        SwitchToTab(CharacterDetailsTab.Attribute);
    }

    public void SwitchToTab(CharacterDetailsTab tab)
    {
        currentTab = tab;

        attributePanel.SetActive(tab == CharacterDetailsTab.Attribute);
        weaponPanel.SetActive(tab == CharacterDetailsTab.Weapon);
        agimatPanel.SetActive(tab == CharacterDetailsTab.Agimat);
        pamanaPanel.SetActive(tab == CharacterDetailsTab.Pamana);
    }

    // These will be hooked to button OnClick events in Inspector
    public void OnClick_AttributeTab() => SwitchToTab(CharacterDetailsTab.Attribute);
    public void OnClick_WeaponTab() => SwitchToTab(CharacterDetailsTab.Weapon);
    public void OnClick_AgimatTab() => SwitchToTab(CharacterDetailsTab.Agimat);
    public void OnClick_PamanaTab() => SwitchToTab(CharacterDetailsTab.Pamana);
}
