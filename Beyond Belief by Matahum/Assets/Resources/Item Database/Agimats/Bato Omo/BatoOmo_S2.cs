using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Agimat/Abilities/BatoOmo/BatoOmo_S2")]
public class BatoOmo_S2 : R_AgimatAbility
{
    private Coroutine buffCoroutine;
    private bool isBuffActive = false;
    private float originalValue = 0;
    public GameObject defUpVFX;
    private GameObject activeVFX;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float increasedDefenses = itemData.slot2RollValue[0];
        float healthThreshold = itemData.slot2RollValue[1];
        return $"When HP is below {healthThreshold:F1}%, gain {increasedDefenses:F1}% defense.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float increasedDefenses = itemData.slot2RollValue[0];
        float healthThreshold = itemData.slot2RollValue[1];

        PlayerStats stats = user.GetComponent<PlayerStats>();
        float threshold = stats.p_maxHealth * (healthThreshold / 100f);

        if (buffCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);

        buffCoroutine = CoroutineRunner.Instance.RunCoroutine(CheckBuff(stats, threshold, increasedDefenses));
    }

    public override void Deactivate(GameObject user)
    {
        isBuffActive = false;
        PlayerStats stats = user.GetComponent<PlayerStats>();
        stats.p_defense = originalValue;

        if (buffCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(buffCoroutine);
            buffCoroutine = null;
        }

        if(activeVFX != null) Destroy(activeVFX);
    }

    private IEnumerator CheckBuff(PlayerStats stats, float threshold, float increasedDefenses)
    {
        while (true)
        {
            if (stats.p_currentHealth <= threshold)
            {
                if (!isBuffActive)
                {
                    originalValue = stats.p_defense;
                    stats.p_defense = Mathf.RoundToInt(originalValue * (1f + (increasedDefenses / 100f)));
                    isBuffActive = true;

                    Vector3 offset = new Vector3(0, 1, 0);
                    if(activeVFX == null)
                        activeVFX = Instantiate(defUpVFX, stats.transform.position + offset, Quaternion.identity, stats.transform);
                    activeVFX.SetActive(true);
                }
            }
            else
            {
                if (isBuffActive)
                {
                    stats.p_defense = originalValue;
                    isBuffActive = false;

                    if (activeVFX != null)
                        activeVFX.SetActive(false);

                }
            }

            yield return new WaitForSeconds(passiveTriggerDelay);
        }
    }

    public float GetRandomDefensePercent(R_ItemRarity rarity)
    {
        // Treat as increased defense roll
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(15f, 40f),
            R_ItemRarity.Rare => Random.Range(30f, 60f),
            R_ItemRarity.Epic => Random.Range(45f, 80f),
            R_ItemRarity.Legendary => Random.Range(60f, 100f),
            _ => 15f
        };
    }

    public float GetRandomHealthThreshold(R_ItemRarity rarity)
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
}
