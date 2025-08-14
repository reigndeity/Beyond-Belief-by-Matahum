using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BlacksmithUpgradeUI : MonoBehaviour
{
    [Header("References")]
    public BlacksmithUpgradeManager upgradeManager;

    [Header("Tabs")]
    public Button pamanaTabButton;
    public Button weaponTabButton;

    [Header("Pamana List")]
    public Transform pamanaListParent;
    public GameObject pamanaButtonPrefab;

    [Header("Pamana Details")]
    public Image pamanaIcon;
    public TextMeshProUGUI pamanaName;
    public TextMeshProUGUI pamanaLevel;
    public TextMeshProUGUI pamanaMainStat;
    public Image pamanaExpFill; // <-- now using Image instead of Slider
    public TextMeshProUGUI pamanaExpText;

    [Header("Weapon Details")]
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponLevel;
    public Image weaponExpFill; // <-- now using Image instead of Slider
    public TextMeshProUGUI weaponExpText;

    [Header("Materials List")]
    public Transform materialListParent;
    public GameObject materialButtonPrefab;

    [Header("Quantity Selector")]
    public TextMeshProUGUI quantityText;
    private int currentQuantity = 1;

    [Header("Upgrade Button")]
    public Button upgradeButton;

    // State
    private bool upgradingPamana = true;
    private R_ItemData selectedPamana;
    private R_InventoryItem selectedMaterial;

    private void OnEnable()
    {
        // Setup tab buttons
        pamanaTabButton.onClick.AddListener(() => SwitchToPamana());
        weaponTabButton.onClick.AddListener(() => SwitchToWeapon());

        SwitchToPamana();
    }

    private void SwitchToPamana()
    {
        upgradingPamana = true;
        ClearPamanaDetails();
        PopulatePamanaList();
        PopulateMaterialList();
    }

    private void SwitchToWeapon()
    {
        upgradingPamana = false;
        ClearWeaponDetails();
        PopulateWeaponDetails();
        PopulateMaterialList();
    }

    // ===== PAMANA MODE =====
    private void PopulatePamanaList()
    {
        foreach (Transform child in pamanaListParent)
            Destroy(child.gameObject);

        var pamanas = upgradeManager.GetEquippedPamanas();

        foreach (var pamana in pamanas)
        {
            var btn = Instantiate(pamanaButtonPrefab, pamanaListParent).GetComponent<Button>();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = pamana.itemName;

            var capturedPamana = pamana;
            btn.onClick.AddListener(() => SelectPamana(capturedPamana));
        }
    }

    private void SelectPamana(R_ItemData pamana)
    {
        selectedPamana = pamana;

        pamanaIcon.sprite = pamana.itemIcon;
        pamanaName.text = pamana.itemName;
        pamanaLevel.text = $"Level {pamana.pamanaData.currentLevel}";
        pamanaMainStat.text = $"{pamana.pamanaData.mainStatType}: {pamana.pamanaData.mainStatValue}";

        float fill = (float)pamana.pamanaData.currentExp / pamana.pamanaData.maxExp;
        pamanaExpFill.fillAmount = Mathf.Clamp01(fill);
        pamanaExpText.text = $"{pamana.pamanaData.currentExp} / {pamana.pamanaData.maxExp}";
    }

    private void ClearPamanaDetails()
    {
        pamanaIcon.sprite = null;
        pamanaName.text = "";
        pamanaLevel.text = "";
        pamanaMainStat.text = "";
        pamanaExpFill.fillAmount = 0;
        pamanaExpText.text = "";
    }

    // ===== WEAPON MODE =====
    private void PopulateWeaponDetails()
    {
        var stats = upgradeManager.player.GetComponent<PlayerStats>();
        weaponName.text = "Current Weapon";
        weaponLevel.text = $"Level {stats.weaponLevel}";

        int maxExp = upgradeManager.player.GetWeaponXPRequired(stats.weaponLevel);
        float fill = (float)stats.weaponXP / maxExp;
        weaponExpFill.fillAmount = Mathf.Clamp01(fill);
        weaponExpText.text = $"{stats.weaponXP} / {maxExp}";
    }

    private void ClearWeaponDetails()
    {
        weaponName.text = "";
        weaponLevel.text = "";
        weaponExpFill.fillAmount = 0;
        weaponExpText.text = "";
    }

    // ===== MATERIALS =====
    private void PopulateMaterialList()
    {
        foreach (Transform child in materialListParent)
            Destroy(child.gameObject);

        List<R_InventoryItem> materials = upgradingPamana
            ? upgradeManager.GetPamanaUpgradeMaterials()
            : upgradeManager.GetWeaponUpgradeMaterials();

        foreach (var mat in materials)
        {
            var btn = Instantiate(materialButtonPrefab, materialListParent).GetComponent<Button>();
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"{mat.itemData.itemName} x{mat.quantity}";

            var capturedMat = mat;
            btn.onClick.AddListener(() => SelectMaterial(capturedMat));
        }
    }

    private void SelectMaterial(R_InventoryItem material)
    {
        selectedMaterial = material;
        currentQuantity = 1;
        UpdateQuantityText();
        upgradeButton.interactable = true;
    }

    // ===== QUANTITY & UPGRADE =====
    public void ChangeQuantity(int delta)
    {
        if (selectedMaterial == null) return;

        currentQuantity = Mathf.Clamp(currentQuantity + delta, 1, selectedMaterial.quantity);
        UpdateQuantityText();
    }

    private void UpdateQuantityText()
    {
        quantityText.text = currentQuantity.ToString();
    }

    public void ConfirmUpgrade()
    {
        if (selectedMaterial == null) return;

        if (upgradingPamana && selectedPamana != null)
        {
            upgradeManager.UpgradePamana(selectedMaterial, selectedPamana, currentQuantity);
            SelectPamana(selectedPamana);
        }
        else if (!upgradingPamana)
        {
            upgradeManager.UpgradeWeapon(selectedMaterial, currentQuantity);
            PopulateWeaponDetails();
        }

        PopulateMaterialList();
    }
}
