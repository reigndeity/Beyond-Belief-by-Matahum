using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSaging/MutyaNgSaging_S2")]
public class MutyaNgSaging_S2 : R_AgimatAbility
{
    public GameObject bulletPrefab;
    [HideInInspector] public int bulletCount;
    [HideInInspector] public float bulletDamage;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (bulletCount == 0f)
            bulletCount = GetBulletCount(rarity);

        if (bulletDamage == 0f)
            bulletDamage = GetRandomDamage(rarity);

        return $"Shoots {bulletCount} banana bullets in quick succession.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        CoroutineRunner.Instance.StartCoroutine(ShootBullets(user, rarity));

        Debug.Log($"Shot {bulletCount} bullets.");
    }

    IEnumerator ShootBullets(GameObject user, R_ItemRarity rarity)
    {
        for(int i = 0; i < bulletCount; i++)
        {
            Vector3 spawnPoint = user.transform.position + user.transform.forward * 0.5f + Vector3.up * 1f;
            GameObject bulletObj = Instantiate(bulletPrefab, spawnPoint, user.transform.rotation);
            bulletObj.transform.localScale = Vector3.one;

            MutyaNgSaging_Bullet bullet = bulletObj.GetComponent<MutyaNgSaging_Bullet>();
            bullet.bulletDamage = user.GetComponent<PlayerStats>().p_attack * .75f; //75% of player attack
            bullet.SetDirection(user.transform.forward);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private float GetRandomDamage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(39, 65),
            R_ItemRarity.Rare => Random.Range(46.5f, 71.5f),
            R_ItemRarity.Epic => Random.Range(54, 79),
            R_ItemRarity.Legendary => Random.Range(61.5f, 86.5f),
            _ => 10f
        };
    }

    private int GetBulletCount(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => 1,
            R_ItemRarity.Rare => 2,
            R_ItemRarity.Epic => 3,
            R_ItemRarity.Legendary => 4,
            _ => 3
        };
    }
}
