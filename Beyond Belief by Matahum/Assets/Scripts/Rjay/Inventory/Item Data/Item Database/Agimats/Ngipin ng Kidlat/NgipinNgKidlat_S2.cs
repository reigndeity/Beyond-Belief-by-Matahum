using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/NgipinNgKidlat_S2")]
public class NgipinNgKidlat_S2 : R_AgimatAbility
{
    [Header("Strike Settings")]
    [SerializeField] private float searchRadius = 12f;   // how far we look for the nearest enemy
    [SerializeField] private float strikeRadius = 4f;    // AoE radius around the struck enemy

    [Header("Visual Effect")]
    [SerializeField] private GameObject lightningVFXPrefab;
    [SerializeField] private float vfxYOffset = 0f; // 🔹 new offset value

    [HideInInspector] public float cachedDamagePercent;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (cachedDamagePercent == 0f)
            cachedDamagePercent = GetRandomDamagePercent(rarity);

        return $"Calls down a thunder strike on the nearest enemy, dealing {cachedDamagePercent:F1}% of ATK. Nearby enemies within {strikeRadius}m are also damaged.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        if (cachedDamagePercent == 0f)
            cachedDamagePercent = GetRandomDamagePercent(rarity);

        float atk = user.GetComponent<PlayerStats>().p_attack;
        
        // ✅ Damage = ATK + (ATK * %)
        float damage = atk + (atk * (cachedDamagePercent / 100f));

        // 1️⃣ Find the closest enemy in searchRadius
        Enemy mainTarget = FindNearestEnemy(user.transform.position, searchRadius);

        if (mainTarget != null)
        {
            Vector3 strikePoint = mainTarget.transform.position;

            // 2️⃣ Damage the main target
            mainTarget.TakeDamage(damage);
            Debug.Log($"⚡ Ngipin Ng Kidlat S2 struck {mainTarget.name} for {damage:F1} damage (ATK + {cachedDamagePercent:F1}% of ATK).");

            // ✅ Spawn thunder VFX only once at the main target
            if (lightningVFXPrefab != null)
                SpawnAndDestroyVFX(strikePoint);

            // 3️⃣ Damage other enemies nearby (splash damage, no extra prefab)
            Collider[] hits = Physics.OverlapSphere(strikePoint, strikeRadius);
            foreach (Collider col in hits)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null && enemy != mainTarget)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log($"⚡ AoE splash hit {enemy.name} for {damage:F1} damage.");
                }
            }
        }
        else
        {
            Debug.Log("⚡ Ngipin Ng Kidlat S2 activated but no enemy was found in range.");
        }
    }


    private Enemy FindNearestEnemy(Vector3 origin, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius);
        float closestDist = Mathf.Infinity;
        Enemy closestEnemy = null;

        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                float dist = Vector3.Distance(origin, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void SpawnAndDestroyVFX(Vector3 position)
    {
        // apply Y offset
        Vector3 spawnPos = new Vector3(position.x, position.y + vfxYOffset, position.z);

        GameObject vfx = GameObject.Instantiate(lightningVFXPrefab, spawnPos, lightningVFXPrefab.transform.rotation);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            GameObject.Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            GameObject.Destroy(vfx, 2f);
        }
    }


    private float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(10f, 15f),
            R_ItemRarity.Rare => Random.Range(16f, 20f),
            R_ItemRarity.Epic => Random.Range(21f, 25f),
            R_ItemRarity.Legendary => Random.Range(26f, 30f),
            _ => 5f
        };
    }
}
