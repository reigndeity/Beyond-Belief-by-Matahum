using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSaging/MutyaNgSaging_S2")]
public class MutyaNgSaging_S2 : R_AgimatAbility
{
    public GameObject bulletPrefab;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        int bulletCount = Mathf.RoundToInt(itemData.slot2RollValue[0]);
        float bulletDamage = itemData.slot2RollValue[1];

        return $"Shoots {bulletCount} banana bullets in quick succession, each dealing {bulletDamage:F1}% of ATK.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        int bulletCount = Mathf.RoundToInt(itemData.slot2RollValue[0]);
        float bulletDamage = itemData.slot2RollValue[1];

        CoroutineRunner.Instance.StartCoroutine(ShootBullets(user, bulletCount, bulletDamage));
        Debug.Log($"🍌 Shot {bulletCount} bullets at {bulletDamage:F1}% ATK each.");
    }

    private IEnumerator ShootBullets(GameObject user, int bulletCount, float bulletDamage)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 spawnPoint = user.transform.position + user.transform.forward * 0.5f + Vector3.up * 1f;
            GameObject bulletObj = Instantiate(bulletPrefab, spawnPoint, user.transform.rotation);
            bulletObj.transform.localScale = Vector3.one;

            MutyaNgSaging_Bullet bullet = bulletObj.GetComponent<MutyaNgSaging_Bullet>();
            float atk = user.GetComponent<PlayerStats>().p_attack;
            bullet.bulletDamage = atk * (bulletDamage / 100f); // scale by ATK %
            bullet.SetDirection(user.transform.forward);

            yield return new WaitForSeconds(0.5f);
        }
    }
    public int GetBulletCount(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => 2,
            R_ItemRarity.Rare => 3,
            R_ItemRarity.Epic => 4,
            R_ItemRarity.Legendary => 5,
            _ => 2
        };
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(85, 105),
            R_ItemRarity.Rare => Random.Range(105, 115),
            R_ItemRarity.Epic => Random.Range(115, 140),
            R_ItemRarity.Legendary => Random.Range(140, 165),
            _ => 85f
        };
    }
}
