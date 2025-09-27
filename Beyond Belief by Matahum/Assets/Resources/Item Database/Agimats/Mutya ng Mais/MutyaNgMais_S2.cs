using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais/MutyaNgMais_S2")]
public class MutyaNgMais_S2 : R_AgimatAbility
{
    [Header("Visual Effect")]
    [SerializeField] private GameObject regenVFXPrefab;
    [SerializeField] private float vfxYOffset = 0f;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot2RollValue[0];
        return $"Regenerates {roll:F1} HP every {baseCooldown:F1} seconds.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot2RollValue[0];

        var stats = user.GetComponent<PlayerStats>();
        var player = user.GetComponent<Player>();

        if (stats.p_currentHealth < stats.p_maxHealth)
        {
            player.Heal(roll);

            Debug.Log($"🌽 Passive heal: +{roll:F1} HP every {baseCooldown:F1}s");
            Debug.Log($"🌽 [PASSIVE] MutyaNgMais_S2 activated at {Time.time:F2}s");

            if (regenVFXPrefab != null)
                SpawnAndDestroyVFX(user.transform.position);
        }
        else
        {
            Debug.Log("🌽 Passive heal skipped — player is already at full HP.");
        }
    }

    private void SpawnAndDestroyVFX(Vector3 position)
    {
        Vector3 spawnPos = new Vector3(position.x, position.y + vfxYOffset, position.z);
        GameObject vfx = GameObject.Instantiate(regenVFXPrefab, spawnPos, regenVFXPrefab.transform.rotation);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
            GameObject.Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            GameObject.Destroy(vfx, 2f);
    }

    public float GetRandomHealPercent(R_ItemRarity rarity)
    {
        // For S2 we’re treating “damage percent” as flat heal roll
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(2f, 3f),
            R_ItemRarity.Rare => Random.Range(4f, 6f),
            R_ItemRarity.Epic => Random.Range(7f, 9f),
            R_ItemRarity.Legendary => Random.Range(10f, 12f),
            _ => 1f
        };
    }
}
