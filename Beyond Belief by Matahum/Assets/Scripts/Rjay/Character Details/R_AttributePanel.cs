using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class R_AttributePanel : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Level Info")]
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private Image xpFillImage;

    [Header("Stat Text Fields")]
    [SerializeField] private TextMeshProUGUI maxHPTxt;
    [SerializeField] private TextMeshProUGUI atkTxt;
    [SerializeField] private TextMeshProUGUI defTxt;
    [SerializeField] private TextMeshProUGUI critRateTxt;
    [SerializeField] private TextMeshProUGUI critDmgTxt;
    [SerializeField] private TextMeshProUGUI cooldownTxt;
    [SerializeField] private TextMeshProUGUI staminaTxt;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }
    private void OnEnable()
    {
        RefreshStats();
    }

    public void RefreshStats()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats not assigned to R_AttributePanel.");
            return;
        }

        // Level display
        levelTxt.text = $"Level {playerStats.currentLevel} / 50";

        // XP bar fill calculation
        int currXP = playerStats.currentExp;
        int xpToNext = PlayerLevelTable.GetXPRequiredForLevel(playerStats.currentLevel);
        float fill = xpToNext > 0 ? (float)currXP / xpToNext : 0f;
        xpFillImage.fillAmount = fill;

        // Stat fields
        maxHPTxt.text = $"HP: {playerStats.p_maxHealth:F0}";
        atkTxt.text = $"ATK: {playerStats.p_attack}";
        defTxt.text = $"DEF: {playerStats.p_defense:F0}";
        critRateTxt.text = $"CRIT Rate: {playerStats.p_criticalRate:F1}%";
        critDmgTxt.text = $"CRIT DMG: {playerStats.p_criticalDamage:F1}%";
        cooldownTxt.text = $"CDR: {playerStats.p_cooldownReduction:F1}%";
        staminaTxt.text = $"Stamina: {playerStats.p_stamina:F0}";
    }
}
