using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgMais/MutyaNgMais_S1")]
public class MutyaNgMais_S1 : R_AgimatAbility
{
    [Header("Visual Effect")]
    [SerializeField] private GameObject healVFXPrefab;
    [SerializeField] private float vfxYOffset = 0f;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot1RollValue[0];
        return $"Heals {roll:F1}% of the player's HP.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot1RollValue[0];
        float percent = roll / 100f;
        float maxHP = user.GetComponent<PlayerStats>().p_maxHealth;
        float amountToHeal = maxHP * percent;

        user.GetComponent<Player>().Heal(amountToHeal);
        Debug.Log($"🌽 Healing for {roll:F1}% of HP → {amountToHeal:F1} HP.");

        if (healVFXPrefab != null)
            SpawnAndDestroyVFX(user.transform.position);
    }

    private void SpawnAndDestroyVFX(Vector3 position)
    {
        Vector3 spawnPos = new Vector3(position.x, position.y + vfxYOffset, position.z);
        GameObject vfx = GameObject.Instantiate(healVFXPrefab, spawnPos, healVFXPrefab.transform.rotation);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
            GameObject.Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            GameObject.Destroy(vfx, 2f);
    }

    public float GetRandomHealPercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(9f, 12.5f),
            R_ItemRarity.Rare => Random.Range(18f, 25f),
            R_ItemRarity.Epic => Random.Range(28f, 37.5f),
            R_ItemRarity.Legendary => Random.Range(42f, 50f),
            _ => 10f
        };
    }
}
