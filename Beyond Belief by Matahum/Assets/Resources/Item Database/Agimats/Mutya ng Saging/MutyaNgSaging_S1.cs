using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;


[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSaging/MutyaNgSaging_S1")]
public class MutyaNgSaging_S1 : R_AgimatAbility
{
    public GameObject bananaTurret;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        int bulletCount = Mathf.RoundToInt(itemData.slot1RollValue[0]);
        float bulletDamage = itemData.slot1RollValue[1];
        float turretDuration = bulletCount * 2f;

        return $"Plants a banana turret that attacks for {bulletCount} times, dealing {bulletDamage} each. " +
               $"Turret dies after attacking {bulletCount} times or after {turretDuration} seconds.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        int bulletCount = Mathf.RoundToInt(itemData.slot1RollValue[0]);
        float bulletDamage = itemData.slot1RollValue[1];
        float turretDuration = bulletCount * 2f;

        Vector3 spawnPoint = user.transform.position + user.transform.forward * 1f + Vector3.up * 0.5f;

        if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            Vector3 navMeshOffset = new Vector3(0, 0.11f, 0);
            GameObject turretObj = Instantiate(
                bananaTurret,
                hit.position - navMeshOffset,
                Quaternion.identity
            );

            MutyaNgSaging_Turret turret = turretObj.GetComponent<MutyaNgSaging_Turret>();
            turret.bulletDamage = user.GetComponent<PlayerStats>().p_attack * (bulletDamage / 100f);
            turret.bulletCount = bulletCount;
            turret.bulletsLeft = bulletCount;
            turret.turretDuration = turretDuration;

            Debug.Log($"🍌 Placed a turret that will attack {bulletCount} times, {bulletDamage:F1} dmg each.");
        }
    }
    public int GetBulletCount(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => 3,
            R_ItemRarity.Rare => 4,
            R_ItemRarity.Epic => 5,
            R_ItemRarity.Legendary => 6,
            _ => 3
        };
    }
    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(70, 90),
            R_ItemRarity.Rare => Random.Range(90, 100),
            R_ItemRarity.Epic => Random.Range(100, 125),
            R_ItemRarity.Legendary => Random.Range(125, 150),
            _ => 70f
        };
    }
}
