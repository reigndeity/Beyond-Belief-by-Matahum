using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgKalamansi/MutyaNgKalamansi_S2")]
public class MutyaNgKalamansi_S2 : R_AgimatAbility
{
    public GameObject kalamansi_BulletPrefab;
    public float bulletRange = 10f;

    [Header("Throw Settings")]
    public float throwForce = 10f;
    public float upwardForce = 5f;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float damagePercentage = itemData.slot2RollValue[0];
        float armorReduction = itemData.slot2RollValue[1];

        return $"Throws a kalamansi that deals {damagePercentage:F1}% ATK and reduces enemy defenses by {armorReduction:F1}% for 5s.\n\nDeals 50% more damage against Aswang.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float damagePercentage = itemData.slot2RollValue[0];
        float armorReduction = itemData.slot2RollValue[1];

        Vector3 throwPoint = user.transform.position + user.transform.forward * 1f + Vector3.up;
        GameObject projectile = Instantiate(kalamansi_BulletPrefab, throwPoint, Quaternion.identity);

        Kalamansi_Bullet bullet = projectile.GetComponent<Kalamansi_Bullet>();
        float atk = user.GetComponent<PlayerStats>().p_attack;
        bullet.debuffType = Kalamansi_DebuffType.ArmorDebuff;
        bullet.bulletDamage = atk * (damagePercentage / 100f);
        bullet.debuffValue = armorReduction;

        // Aim assist
        Collider[] hits = Physics.OverlapSphere(user.transform.position, bulletRange);
        Transform target = null;
        float minDist = Mathf.Infinity;
        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                float dist = Vector3.Distance(user.transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = hit.transform;
                }
            }
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDir;
            if (target != null)
            {
                Vector3 dirToEnemy = (target.position - throwPoint).normalized;
                forceDir = dirToEnemy * throwForce + Vector3.up * upwardForce;
            }
            else
            {
                forceDir = user.transform.forward * throwForce + Vector3.up * upwardForce;
            }
            rb.AddForce(forceDir, ForceMode.Impulse);
        }
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(60, 95f),
            R_ItemRarity.Rare => Random.Range(80, 120),
            R_ItemRarity.Epic => Random.Range(100, 145f),
            R_ItemRarity.Legendary => Random.Range(120, 170),
            _ => 60f
        };
    }

    public float GetRandomDefenseReductionPercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(25, 37.5f),
            R_ItemRarity.Rare => Random.Range(33.33f, 50),
            R_ItemRarity.Epic => Random.Range(41.66f, 62.5f),
            R_ItemRarity.Legendary => Random.Range(50, 75),
            _ => 25f
        };
    }
}
