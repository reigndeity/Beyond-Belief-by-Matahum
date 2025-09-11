using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/NgipinNgKidlat/NgipinNgKidlat_S1")]
public class NgipinNgKidlat_S1 : R_AgimatAbility
{
    [Header("AOE Settings")]
    [SerializeField] private float strikeRadius = 6f;

    [Header("Visual Effect")]
    [SerializeField] private GameObject lightningVFXPrefab;
    [SerializeField] private float vfxYOffset = 0f;

    [HideInInspector] public float cachedDamagePercent;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (cachedDamagePercent == 0f)
            cachedDamagePercent = GetRandomDamagePercent(rarity);

        return $"Summons a lightning ball, instantly dealing {cachedDamagePercent:F1}% of ATK to all targets within {strikeRadius}m.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        if (cachedDamagePercent == 0f)
            cachedDamagePercent = GetRandomDamagePercent(rarity);

        float atk = user.GetComponent<PlayerStats>().p_attack;
        float damage = atk + (atk * (cachedDamagePercent / 100f));

        Vector3 origin = user.transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, strikeRadius);

        int targetsHit = 0;
        foreach (Collider col in hits)
        {
            IDamageable dmg = col.GetComponent<IDamageable>();
            if (dmg != null && !dmg.IsDead() && col.gameObject != user) // ✅ skip self
            {
                dmg.TakeDamage(damage);
                targetsHit++;
                Debug.Log($"⚡ Ngipin Ng Kidlat struck {col.name} for {damage:F1} damage (ATK + {cachedDamagePercent:F1}% of ATK).");

                if (lightningVFXPrefab != null)
                    SpawnAndDestroyVFX(col.transform.position);
            }
        }

        if (targetsHit == 0)
            Debug.Log("⚡ Ngipin Ng Kidlat activated but no targets were in range.");
    }

    private void SpawnAndDestroyVFX(Vector3 position)
    {
        Vector3 spawnPos = new Vector3(position.x, position.y + vfxYOffset, position.z);
        GameObject vfx = GameObject.Instantiate(lightningVFXPrefab, spawnPos, lightningVFXPrefab.transform.rotation);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
            GameObject.Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            GameObject.Destroy(vfx, 2f);
    }

    private float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(2f, 5f),
            R_ItemRarity.Rare => Random.Range(6f, 10f),
            R_ItemRarity.Epic => Random.Range(11f, 15f),
            R_ItemRarity.Legendary => Random.Range(15f, 20f),
            _ => 5f
        };
    }
}
