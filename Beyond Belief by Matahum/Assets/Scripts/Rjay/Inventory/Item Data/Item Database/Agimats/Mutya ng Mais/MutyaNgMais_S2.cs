using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais/MutyaNgMais_S2")]
public class MutyaNgMais_S2 : R_AgimatAbility
{
    [HideInInspector] public float cachedFlatHeal;

    [Header("Visual Effect")]
    [SerializeField] private GameObject regenVFXPrefab;
    [SerializeField] private float vfxYOffset = 0f;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (cachedFlatHeal == 0f)
            cachedFlatHeal = GetRandomFlatHeal(rarity);

        return $"Regenerates {cachedFlatHeal:F1} HP every {baseCooldown:F1} seconds.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        if (cachedFlatHeal == 0f)
            cachedFlatHeal = GetRandomFlatHeal(rarity);

        var stats = user.GetComponent<PlayerStats>();
        var player = user.GetComponent<Player>();

        // ✅ Only heal if not already at full HP
        if (stats.p_currentHealth < stats.p_maxHealth)
        {
            player.Heal(cachedFlatHeal);

            Debug.Log($"🌽 Passive heal: +{cachedFlatHeal:F1} HP every {baseCooldown:F1}s");
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

    private float GetRandomFlatHeal(R_ItemRarity rarity)
    {
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
