using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSampalok/MutyaNgSampalok_S1")]
public class MutyaNgSampalok_S1 : R_AgimatAbility
{
    [HideInInspector] public float damage = 0f;

    [Header("Vine Properties")]
    public GameObject vinePrefab;
    public float sweepAngle = 180f;    // total sweep arc
    public float sweepSpeed = 360f;    // degrees per second

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (damage == 0)
            damage = GetRandomDamagePercent(rarity);

        return $"Lashes out a vine in front, dealing {damage:F1} damage to enemies hit in a cone.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        CoroutineRunner.Instance.RunCoroutine(StartSweeping(user));
    }

    private IEnumerator StartSweeping(GameObject user)
    {
        // Spawn vine and parent to user
        Vector3 offset = new Vector3(0,0.5f,0);
        GameObject vineObj = Instantiate(vinePrefab, user.transform.position + offset, user.transform.rotation, user.transform);
        MutyaNgSampalok_VineDamage vineScript = vineObj.GetComponentInChildren<MutyaNgSampalok_VineDamage>();
        vineScript.damage = user.GetComponent<PlayerStats>().p_attack * (damage / 100);

        // Start from right (+90) relative to player
        float startAngle = 90f;
        float endAngle = -90f;
        float currentAngle = startAngle;

        while (currentAngle > endAngle)
        {
            float step = sweepSpeed * Time.deltaTime;
            currentAngle -= step; // sweep from right → left

            // Apply local rotation relative to player
            vineObj.transform.localRotation = Quaternion.Euler(0, currentAngle, 0);

            yield return null;
        }

        Destroy(vineObj);
    }

    #region RANDOM VALUE GENERATOR
    private float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(45, 60f),
            R_ItemRarity.Rare => Random.Range(55, 80),
            R_ItemRarity.Epic => Random.Range(65, 100f),
            R_ItemRarity.Legendary => Random.Range(75, 125),
            _ => 45f
        };
    }
    #endregion
}
