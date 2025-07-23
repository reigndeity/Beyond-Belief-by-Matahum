using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class R_WeaponPanel : MonoBehaviour
{
    private Player m_player;
    private PlayerStats m_playerStats;
    [SerializeField] private TextMeshProUGUI weaponAtkTxt;
    [SerializeField] private TextMeshProUGUI weaponLevel;
    [SerializeField] private Image weaponFill; // xp

    void OnEnable()
    {
        UpdateWeaponPanel();
    }
    void Start()
    {
        m_player = FindFirstObjectByType<Player>();
        m_playerStats = FindFirstObjectByType<PlayerStats>();
        
    }
    public void UpdateWeaponPanel()
    {
        int level = m_playerStats.weaponLevel;
        int xp = m_playerStats.weaponXP;
        int maxXP = m_player.GetWeaponXPRequired(level);

        weaponLevel.text = $"Lv. {level}";
        weaponAtkTxt.text = $"{Mathf.RoundToInt(m_player.GetWeaponATK())}";

        float fill = (float)xp / maxXP;
        weaponFill.fillAmount = Mathf.Clamp01(fill);
    }
}
