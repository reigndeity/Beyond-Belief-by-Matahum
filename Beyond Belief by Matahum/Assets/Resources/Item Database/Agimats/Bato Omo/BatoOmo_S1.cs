using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/BatoOmo/BatoOmo_S1")]
public class BatoOmo_S1 : R_AgimatAbility
{
    public GameObject shieldPrefab;

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        float shieldDuration = itemData.slot1RollValue;
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
        Vector3 offset = new Vector3(0, 1, 0);
        GameObject shieldObj = Instantiate(shieldPrefab, player.transform.position + offset, Quaternion.identity, player.transform);
        player.isInvulnerable = true;
        yield return new WaitForSeconds(shieldDuration);
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
