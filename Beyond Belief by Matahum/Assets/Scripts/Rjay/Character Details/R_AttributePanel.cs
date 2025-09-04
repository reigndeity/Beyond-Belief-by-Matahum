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

    // 🔹 Store baseline values once
    private float storedBaseHP;
    private float storedBaseATK;
    private float storedBaseDEF;
    private float storedBaseCritRate;
    private float storedBaseCritDmg;
    private float storedBaseCDR;
    private float storedBaseStamina;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    private void OnEnable()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats not assigned to R_AttributePanel.");
            return;
        }

        // Store baseline stats only once
        if (storedBaseHP == 0 && storedBaseATK == 0 && storedBaseDEF == 0)
        {
            storedBaseHP = playerStats.p_maxHealth;
            storedBaseATK = playerStats.p_attack;
            storedBaseDEF = playerStats.p_defense;
            storedBaseCritRate = playerStats.p_criticalRate;
            storedBaseCritDmg = playerStats.p_criticalDamage;
            storedBaseCDR = playerStats.p_cooldownReduction;
            storedBaseStamina = playerStats.p_stamina;
        }

        RefreshStats();
        PlayerStats.OnExpChange += RefreshStats;
    }

    void OnDisable()
    {
        PlayerStats.OnExpChange -= RefreshStats;
    }

    public void RefreshStats()
    {
        if (playerStats == null) return;

        // Level display
        levelTxt.text = $"Level {playerStats.currentLevel} / 50";

        // XP bar
        int currXP = playerStats.currentExp;
        int xpToNext = PlayerLevelTable.GetXPRequiredForLevel(playerStats.currentLevel);
        float fill = xpToNext > 0 ? (float)currXP / xpToNext : 0f;
        xpFillImage.fillAmount = fill;

        // 🔹 HP
        float bonusHP = playerStats.p_maxHealth - storedBaseHP;
        maxHPTxt.text = bonusHP > 0
            ? $"HP: {playerStats.p_maxHealth:F0} <color=#00FF00>(+{bonusHP:F0})</color>"
            : $"HP: {playerStats.p_maxHealth:F0}";

        // 🔹 ATK
        float bonusATK = playerStats.p_attack - storedBaseATK;
        atkTxt.text = bonusATK > 0
            ? $"ATK: {playerStats.p_attack:F0} <color=#00FF00>(+{bonusATK:F0})</color>"
            : $"ATK: {playerStats.p_attack:F0}";

        // 🔹 DEF
        float bonusDEF = playerStats.p_defense - storedBaseDEF;
        defTxt.text = bonusDEF > 0
            ? $"DEF: {playerStats.p_defense:F0} <color=#00FF00>(+{bonusDEF:F0})</color>"
            : $"DEF: {playerStats.p_defense:F0}";

        // 🔹 CRIT Rate
        float bonusCritRate = playerStats.p_criticalRate - storedBaseCritRate;
        critRateTxt.text = bonusCritRate > 0
            ? $"CRIT Rate: {playerStats.p_criticalRate:F1}% <color=#00FF00>(+{bonusCritRate:F1}%)</color>"
            : $"CRIT Rate: {playerStats.p_criticalRate:F1}%";

        // 🔹 CRIT DMG
        float bonusCritDmg = playerStats.p_criticalDamage - storedBaseCritDmg;
        critDmgTxt.text = bonusCritDmg > 0
            ? $"CRIT DMG: {playerStats.p_criticalDamage:F1}% <color=#00FF00>(+{bonusCritDmg:F1}%)</color>"
            : $"CRIT DMG: {playerStats.p_criticalDamage:F1}%";

        // 🔹 Cooldown Reduction
        float bonusCDR = playerStats.p_cooldownReduction - storedBaseCDR;
        cooldownTxt.text = bonusCDR > 0
            ? $"CDR: {playerStats.p_cooldownReduction:F1}% <color=#00FF00>(+{bonusCDR:F1}%)</color>"
            : $"CDR: {playerStats.p_cooldownReduction:F1}%";

        // 🔹 Stamina
        float bonusStamina = playerStats.p_stamina - storedBaseStamina;
        staminaTxt.text = bonusStamina > 0
            ? $"Stamina: {playerStats.p_stamina:F0} <color=#00FF00>(+{bonusStamina:F0})</color>"
            : $"Stamina: {playerStats.p_stamina:F0}";

    }
}
