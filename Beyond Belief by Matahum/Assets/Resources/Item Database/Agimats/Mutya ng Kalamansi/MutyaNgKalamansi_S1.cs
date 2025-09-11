using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgKalamansi/MutyaNgKalamansi_S1")]
public class MutyaNgKalamansi_S1 : R_AgimatAbility
{
    [HideInInspector] public float damagePercentage;
    [HideInInspector] public float damageReduction;
    public GameObject kalamansi_BulletPrefab;
    public float bulletRange = 10f;

    [Header("Throw Settings")]
    public float throwForce = 10f;        // Base throw strength
    public float upwardForce = 5f;        // Extra arc height
    public override string GetDescription(R_ItemRarity rarity)
    {
        if (damagePercentage == 0)
            damagePercentage = GetRandomDamagePercent(rarity);

        if(damageReduction == 0)
            damageReduction = GetRandomDamageReductionPercent(rarity);

        return $"Throws a kalamansi that deals {damagePercentage:F1} and reduces enemy damage by {damageReduction:F1}% for 5 seconds. \n\n Deals 150% more damage against Aswang";
    }
    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        Vector3 throwPoint = user.transform.position + user.transform.forward * 1f + Vector3.up * 0.5f;
        GameObject projectile = Instantiate(kalamansi_BulletPrefab, throwPoint, Quaternion.identity);

        // Setup bullet stats
        Kalamansi_Bullet bullet = projectile.GetComponent<Kalamansi_Bullet>();
        float atk = user.GetComponent<PlayerStats>().p_attack;
        bullet.debuffType = Kalamansi_DebuffType.DamageDebuff;
        bullet.bulletDamage = atk * (damagePercentage / 100f);
        bullet.debuffValue = damageReduction;

        // Find enemies in range
        Collider[] hits = Physics.OverlapSphere(user.transform.position, bulletRange);
        Transform target = null;

        if (hits.Length > 0)
        {
            // Pick the closest enemy
            float minDist = Mathf.Infinity;
            foreach (var hit in hits)
            {
                if (hit.gameObject.GetComponent<EnemyStats>())
                {
                    float dist = Vector3.Distance(user.transform.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = hit.transform;
                    }
                }
            }
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDir;

            if (target != null)
            {
                // Aim at the enemy's last known position
                Vector3 dirToEnemy = (target.position - throwPoint).normalized;
                forceDir = dirToEnemy * throwForce + Vector3.up * upwardForce;
            }
            else
            {
                // No enemy → just throw forward
                forceDir = user.transform.forward * throwForce + Vector3.up * upwardForce;
            }

            rb.AddForce(forceDir, ForceMode.Impulse);
        }
    }


    #region RANDOM VALUE GENERATOR
    private float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(30, 47.5f),
            R_ItemRarity.Rare => Random.Range(40, 60),
            R_ItemRarity.Epic => Random.Range(50, 72.5f),
            R_ItemRarity.Legendary => Random.Range(60, 85),
            _ => 30f
        };
    }

    private float GetRandomDamageReductionPercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(25, 37.5f),
            R_ItemRarity.Rare => Random.Range(33.33f, 50),
            R_ItemRarity.Epic => Random.Range(41.66f, 62.5f),
            R_ItemRarity.Legendary => Random.Range(50, 75),
            _ => 30f
        };
    }
    #endregion
}
