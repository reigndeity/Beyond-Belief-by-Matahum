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

    private Player m_player;

    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        m_player = FindFirstObjectByType<Player>();
    }

    private void OnEnable()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats not assigned to R_AttributePanel.");
            return;
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

        // --- Baseline stats from growth table ---
        float baseHP  = PlayerStatGrowthTable.GetHP(playerStats.currentLevel);
        float baseATK = PlayerStatGrowthTable.GetATK(playerStats.currentLevel);
        float baseDEF = PlayerStatGrowthTable.GetDEF(playerStats.currentLevel);

        // --- Weapon ATK ---
        float weaponATK = m_player != null ? m_player.GetWeaponATK() : 0f;
        float atkWithoutWeapon = playerStats.p_attack - weaponATK;

        // --- Flat baselines from PlayerStats ---
        float baseCritRate = playerStats.baseCriticalRate;
        float baseCritDmg  = playerStats.baseCriticalDamage;
        float baseCDR      = playerStats.baseCooldownReduction;
        float baseStamina  = playerStats.baseStamina;

        // 🔹 HP
        float bonusHP = playerStats.p_maxHealth - baseHP;
        maxHPTxt.text = bonusHP > 0
            ? $"HP: {playerStats.p_maxHealth:F0} <color=#00FF00>(+{bonusHP:F0})</color>"
            : $"HP: {playerStats.p_maxHealth:F0}";

        // --- ATK (exclude weapon ATK from bonus)
        float bonusATK = atkWithoutWeapon - baseATK;
        int roundedBonusATK = Mathf.RoundToInt(bonusATK);

        if (roundedBonusATK > 0)
        {
            atkTxt.text = $"ATK: {playerStats.p_attack} <color=#00FF00>(+{roundedBonusATK})</color>";
        }
        else if (roundedBonusATK < 0)
        {
            atkTxt.text = $"ATK: {playerStats.p_attack} <color=#FF0000>({roundedBonusATK})</color>";
        }
        else
        {
            atkTxt.text = $"ATK: {playerStats.p_attack}";
}

        // 🔹 DEF
        float bonusDEF = playerStats.p_defense - baseDEF;
        defTxt.text = bonusDEF > 0
            ? $"DEF: {playerStats.p_defense:F0} <color=#00FF00>(+{bonusDEF:F0})</color>"
            : $"DEF: {playerStats.p_defense:F0}";

        // 🔹 CRIT Rate
        float bonusCritRate = playerStats.p_criticalRate - baseCritRate;
        critRateTxt.text = bonusCritRate > 0
            ? $"CRIT Rate: {playerStats.p_criticalRate:F1}% <color=#00FF00>(+{bonusCritRate:F1}%)</color>"
            : $"CRIT Rate: {playerStats.p_criticalRate:F1}%";

        // 🔹 CRIT DMG
        float bonusCritDmg = playerStats.p_criticalDamage - baseCritDmg;
        critDmgTxt.text = bonusCritDmg > 0
            ? $"CRIT DMG: {playerStats.p_criticalDamage:F1}% <color=#00FF00>(+{bonusCritDmg:F1}%)</color>"
            : $"CRIT DMG: {playerStats.p_criticalDamage:F1}%";

        // 🔹 Cooldown Reduction
        float bonusCDR = playerStats.p_cooldownReduction - baseCDR;
        cooldownTxt.text = bonusCDR > 0
            ? $"CDR: {playerStats.p_cooldownReduction:F1}% <color=#00FF00>(+{bonusCDR:F1}%)</color>"
            : $"CDR: {playerStats.p_cooldownReduction:F1}%";

        // 🔹 Stamina
        float bonusStamina = playerStats.p_stamina - baseStamina;
        staminaTxt.text = bonusStamina > 0
            ? $"Stamina: {playerStats.p_stamina:F0} <color=#00FF00>(+{bonusStamina:F0})</color>"
            : $"Stamina: {playerStats.p_stamina:F0}";
    }
}
