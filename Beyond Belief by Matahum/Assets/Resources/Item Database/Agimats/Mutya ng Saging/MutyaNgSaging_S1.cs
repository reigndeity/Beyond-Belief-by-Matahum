using UnityEngine;

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

        Vector3 spawnPoint = new Vector3(user.transform.position.x, user.transform.position.y + 1, user.transform.position.z + 1);
        GameObject turretObj = Instantiate(bananaTurret, spawnPoint, Quaternion.identity);

        MutyaNgSaging_Turret turret = turretObj.GetComponent<MutyaNgSaging_Turret>();
        turret.bulletDamage = user.GetComponent<PlayerStats>().p_attack * (bulletDamage / 100f);
        turret.bulletCount = bulletCount;
        turret.bulletsLeft = bulletCount;
        turret.turretDuration = turretDuration;

        Debug.Log($"🍌 Placed a turret that will attack {bulletCount} times, {bulletDamage:F1} dmg each.");
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
        // Slot2 = bullet damage
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(30, 48),
            R_ItemRarity.Rare => Random.Range(35, 56),
            R_ItemRarity.Epic => Random.Range(40, 64),
            R_ItemRarity.Legendary => Random.Range(45, 72),
            _ => 30f
        };
    }
}
