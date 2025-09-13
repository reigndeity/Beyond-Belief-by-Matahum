using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

[CreateAssetMenu(menuName = "Agimat/Abilities/BatoOmo/BatoOmo_S2")]

public class BatoOmo_S2 : R_AgimatAbility
{
    [HideInInspector] public float increasedDefenses;
    [HideInInspector] public float healthThreshold;

    public float originalValue = 0;
    public bool isBuffActive = false;
    private Coroutine buffCoroutine;

    public override string GetDescription(R_ItemRarity rarity)
    {
        if (increasedDefenses == 0)
            increasedDefenses = GetRandomIncreasedDefense(rarity);

        if(healthThreshold == 0)
            healthThreshold = GetRandomHealthThreshold(rarity);

        return $"When HP is below {healthThreshold:F1}, gain {increasedDefenses:F1}% of defense.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();
        float threshold = stats.p_maxHealth * (healthThreshold / 100f);

        // Stop previous coroutine before starting a new one
        if (buffCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);

        buffCoroutine = CoroutineRunner.Instance.RunCoroutine(CheckBuff(stats, threshold));
    }

    public override void Deactivate(GameObject user)
    {
        isBuffActive = false;
        PlayerStats stats = user.GetComponent<PlayerStats>();
        stats.p_defense = originalValue;
    }

    private IEnumerator CheckBuff(PlayerStats stats, float threshold)
    {
        while (true) // keep checking every interval
        {
            if (stats.p_currentHealth <= threshold)
            {
                if (!isBuffActive)
                {
                    originalValue = stats.p_defense;
                    stats.p_defense = Mathf.RoundToInt(originalValue * (1f + (increasedDefenses / 100f)));
                    isBuffActive = true;
                }
            }
            else
            {
                if (isBuffActive)
                {
                    stats.p_defense = originalValue;
                    isBuffActive = false;
                }
            }

            yield return new WaitForSeconds(passiveTriggerDelay);
        }
    }

    #region RANDOM VALUE GENERATOR
    private float GetRandomIncreasedDefense(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(15f, 40f),
            R_ItemRarity.Rare => Random.Range(30f, 60f),
            R_ItemRarity.Epic => Random.Range(45f, 80f),
            R_ItemRarity.Legendary => Random.Range(60f, 100f),
            _ => 15f
        };
    }

    private float GetRandomHealthThreshold(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(20f, 30f),
            R_ItemRarity.Rare => Random.Range(30f, 45f),
            R_ItemRarity.Epic => Random.Range(40f, 60f),
            R_ItemRarity.Legendary => Random.Range(50f, 75f),
            _ => 20f
        };
    }
    #endregion
}
