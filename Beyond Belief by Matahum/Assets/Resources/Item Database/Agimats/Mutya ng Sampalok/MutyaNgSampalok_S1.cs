using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSampalok/MutyaNgSampalok_S1")]
public class MutyaNgSampalok_S1 : R_AgimatAbility
{
    [Header("Vine Properties")]
    public GameObject vinePrefab;
    public float sweepAngle = 180f;    // total sweep arc
    public float sweepSpeed = 360f;    // degrees per second

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot1RollValue;
        return $"Lashes out a vine in front, dealing {roll:F1}% of ATK to enemies hit in a cone.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float roll = itemData.slot1RollValue;
        CoroutineRunner.Instance.RunCoroutine(StartSweeping(user, roll));
    }

    private IEnumerator StartSweeping(GameObject user, float roll)
    {
        Vector3 offset = new Vector3(0, 0.5f, 0);
        GameObject vineObj = Instantiate(vinePrefab, user.transform.position + offset, user.transform.rotation, user.transform);

        MutyaNgSampalok_VineDamage vineScript = vineObj.GetComponentInChildren<MutyaNgSampalok_VineDamage>();
        float atk = user.GetComponent<PlayerStats>().p_attack;
        vineScript.damage = atk * (roll / 100f);

        float startAngle = 90f;
        float endAngle = -90f;
        float currentAngle = startAngle;

        while (currentAngle > endAngle)
        {
            float step = sweepSpeed * Time.deltaTime;
            currentAngle -= step;
            vineObj.transform.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }

        Destroy(vineObj);
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
