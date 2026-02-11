using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSampalok/MutyaNgSampalok_S1")]
public class MutyaNgSampalok_S1 : R_AgimatAbility
{
    [Header("Vine Properties")]
    public GameObject vinePrefab;
    public float sweepAngle = 180f;    // total sweep arc
    public float sweepSpeed = 360f;    // degrees per second

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot1RollValue[0];
        return $"Lashes out a vine in front, dealing {roll:F1}% of ATK to enemies hit in a cone. Enemies hit get poisoned.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot1RollValue[0];

        Vector3 spawnPoint = user.transform.position + user.transform.forward * 1f + Vector3.up * 0.5f;

        if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            Vector3 navMeshOffset = new Vector3(0, 0.11f, 0);
            GameObject vineObj = Instantiate(vinePrefab, hit.position - navMeshOffset, user.transform.rotation);

            MutyaNgSampalok_VineDamage vineScript = vineObj.GetComponentInChildren<MutyaNgSampalok_VineDamage>();
            float atk = user.GetComponent<PlayerStats>().p_attack;
            vineScript.damage = atk * (roll / 100f);
        }
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(45f, 60f),
            R_ItemRarity.Rare => Random.Range(55f, 80f),
            R_ItemRarity.Epic => Random.Range(65f, 100f),
            R_ItemRarity.Legendary => Random.Range(75f, 125f),
            _ => 45f
        };
    }
}
