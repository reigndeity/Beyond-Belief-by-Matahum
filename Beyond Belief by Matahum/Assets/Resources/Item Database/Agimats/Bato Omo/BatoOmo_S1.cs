using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/BatoOmo/BatoOmo_S1")]
public class BatoOmo_S1 : R_AgimatAbility
{
    public GameObject shieldPrefab;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float shieldDuration = itemData.slot1RollValue + 0.5F;
        return $"Gain a barrier that blocks any damage for {shieldDuration:F1} seconds.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        float shieldDuration = itemData.slot1RollValue;
        Player player = user.GetComponent<Player>();
        CoroutineRunner.Instance.RunCoroutine(SpawnShield(player, shieldDuration));
    }

    private IEnumerator SpawnShield(Player player, float shieldDuration)
    {    
        player.isInvulnerable = true;

        Vector3 offset = new Vector3(0, 1, 0);
        GameObject shieldObj = Instantiate(shieldPrefab, player.transform.position + offset, Quaternion.identity, player.transform);
        //shieldObj.transform.localScale = Vector3.zero;

        float t = 0;
        float duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            shieldObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, normalized);
            yield return null;
        }


        yield return new WaitForSeconds(shieldDuration);

        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            shieldObj.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, normalized);
            yield return null;
        }
        player.isInvulnerable = false;
        Destroy(shieldObj);
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        // Treat as duration roll
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(0.75f, 1.5f),
            R_ItemRarity.Rare => Random.Range(1f, 2f),
            R_ItemRarity.Epic => Random.Range(1.25f, 2.5f),
            R_ItemRarity.Legendary => Random.Range(1.5f, 3f),
            _ => 0.75f
        };
    }
}
