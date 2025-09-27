using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/NgipinNgKidlat/NgipinNgKidlat_S2")]
public class NgipinNgKidlat_S2 : R_AgimatAbility
{
    [Header("Strike Settings")]
    [SerializeField] private float searchRadius = 12f;
    [SerializeField] private float strikeRadius = 4f;

    [Header("Visual Effect")]
    [SerializeField] private GameObject lightningVFXPrefab;
    [SerializeField] private float vfxYOffset = 0f;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot2RollValue[0];
        return $"Calls down a thunder strike on the nearest target, dealing {roll:F1}% of ATK. Nearby targets within {strikeRadius}m are also damaged.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float atk = user.GetComponent<PlayerStats>().p_attack;
        float roll = itemData.slot2RollValue[0];
        float damage = atk + (atk * (roll / 100f));

        // 1️⃣ Find the closest valid target
        IDamageable mainTarget = FindNearestTarget(user, user.transform.position, searchRadius);

        if (mainTarget != null && !mainTarget.IsDead())
        {
            Vector3 strikePoint = (mainTarget as MonoBehaviour).transform.position;

            mainTarget.TakeDamage(damage);
            Debug.Log($"⚡ Ngipin Ng Kidlat S2 struck {strikePoint} for {damage:F1} dmg (ATK + {roll:F1}% of ATK).");

            if (lightningVFXPrefab != null)
                SpawnAndDestroyVFX(strikePoint);

            // 2️⃣ AoE splash
            Collider[] hits = Physics.OverlapSphere(strikePoint, strikeRadius);
            foreach (Collider col in hits)
            {
                IDamageable dmg = col.GetComponent<IDamageable>();
                if (dmg != null && dmg != mainTarget && !dmg.IsDead() && col.gameObject != user)
                {
                    dmg.TakeDamage(damage);
                    Debug.Log($"⚡ AoE splash hit {col.name} for {damage:F1} dmg.");
                }
            }
        }
        else
        {
            Debug.Log("⚡ Ngipin Ng Kidlat S2 activated but no valid target was found in range.");
        }
    }

    private IDamageable FindNearestTarget(GameObject caster, Vector3 origin, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius);
        float closestDist = Mathf.Infinity;
        IDamageable closest = null;

        foreach (Collider col in hits)
        {
            IDamageable dmg = col.GetComponent<IDamageable>();
            if (dmg != null && !dmg.IsDead() && col.gameObject != caster)
            {
                float dist = Vector3.Distance(origin, (dmg as MonoBehaviour).transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = dmg;
                }
            }
        }

        return closest;
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

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
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
