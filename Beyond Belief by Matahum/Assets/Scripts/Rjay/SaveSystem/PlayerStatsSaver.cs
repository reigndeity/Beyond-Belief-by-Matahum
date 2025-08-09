using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerStats))]
public class PlayerStatsSaver : MonoBehaviour, ISaveable
{
    PlayerStats stats;
    PlayerMovement movement;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        SaveManager.Instance.Register(this);
    }
    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Player.Stats";

    [System.Serializable]
    class DTO
    {
        public int level, xp;
        public float hpCurrent, hpMax;
        public int atk;
        public float def, critRate, critDmg;
        public float staminaCurrent, staminaMax;

        // NEW: weapon persistence
        public int weaponLevel;
        public int weaponXP;
    }

    public string CaptureJson()
    {
        var dto = new DTO
        {
            level = stats.currentLevel,
            xp    = stats.currentExp,

            hpCurrent = stats.p_currentHealth,
            hpMax     = stats.p_maxHealth,

            atk      = stats.p_attack,
            def      = stats.p_defense,
            critRate = stats.p_criticalRate,
            critDmg  = stats.p_criticalDamage,

            staminaCurrent = movement ? movement.CurrentStamina : 0f,
            staminaMax     = movement ? movement.MaxStamina     : 0f,

            // NEW
            weaponLevel = stats.weaponLevel,
            weaponXP    = stats.weaponXP
        };
        return JsonUtility.ToJson(dto, false);
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        var dto = JsonUtility.FromJson<DTO>(json);
        if (dto == null) return;

        stats.currentLevel = dto.level;
        stats.currentExp   = dto.xp;

        stats.p_maxHealth     = dto.hpMax;
        stats.p_currentHealth = Mathf.Clamp(dto.hpCurrent, 0f, dto.hpMax);

        stats.p_attack        = dto.atk;
        stats.p_defense       = dto.def;
        stats.p_criticalRate  = dto.critRate;
        stats.p_criticalDamage= dto.critDmg;

        if (movement)
        {
            movement.MaxStamina     = dto.staminaMax;
            movement.CurrentStamina = Mathf.Clamp(dto.staminaCurrent, 0f, dto.staminaMax);
        }

        // NEW: restore weapon fields
        stats.weaponLevel = dto.weaponLevel;
        stats.weaponXP    = dto.weaponXP;

        // Notify any UI/listeners you already wired
        stats.NotifyXPChanged();
        // If you have a stats/damage recalc method, call it here (optional):
        // stats.RecalculateStats();
    }
}
