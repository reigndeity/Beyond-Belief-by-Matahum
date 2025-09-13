using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgLinta/MutyaNgLinta_S2")]
public class MutyaNgLinta_S2 : R_AgimatAbility
{
    [HideInInspector] public float lifeStealValue = 0;

    private Coroutine buffCoroutine;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (lifeStealValue == 0)
            lifeStealValue = GetRandomLifeStealPercentage(rarity);

        return $"Heal a small amount of {lifeStealValue:F1}% of damage dealt with attacks to enemies.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        // Ensure lifesteal value is set
        if (lifeStealValue == 0)
            lifeStealValue = GetRandomLifeStealPercentage(rarity);

        // Stop any old coroutine
        if (buffCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);

        // Start passive effect
        buffCoroutine = CoroutineRunner.Instance.RunCoroutine(ApplyPassive(user));
    }

    public override void Deactivate(GameObject user)
    {
        if (buffCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);
            buffCoroutine = null;
        }

        var lifesteal = user.GetComponent<MutyaNgLinta_Lifesteal>();
        if (lifesteal != null)
            Object.Destroy(lifesteal);
    }

    private IEnumerator ApplyPassive(GameObject user)
    {
        Player player = user.GetComponent<Player>();

        // Ensure lifesteal component exists
        MutyaNgLinta_Lifesteal lifesteal = user.GetComponent<MutyaNgLinta_Lifesteal>();
        if (lifesteal == null)
            lifesteal = user.AddComponent<MutyaNgLinta_Lifesteal>();

        lifesteal.player = player;
        lifesteal.lifeStealPercent = lifeStealValue / 100f;
        lifesteal.enabled = true;

        // Passive: just wait until deactivated
        while (true)
        {
            yield return null;
        }
    }

    #region RANDOM VALUE GENERATOR
    private float GetRandomLifeStealPercentage(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(5f, 10f),
            R_ItemRarity.Rare => Random.Range(7.5f, 13.3f),
            R_ItemRarity.Epic => Random.Range(10f, 16.6f),
            R_ItemRarity.Legendary => Random.Range(12.5f, 20f),
            _ => 5f
        };
    }
    #endregion
}
