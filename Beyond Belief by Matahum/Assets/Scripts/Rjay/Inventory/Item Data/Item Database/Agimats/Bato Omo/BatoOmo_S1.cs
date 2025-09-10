using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Agimat/Abilities/BatoOmo_S1")]
public class BatoOmo_S1 : R_AgimatAbility
{
    [HideInInspector] public float shieldDuration = 0;
    public GameObject shieldPrefab;
    public override string GetDescription(R_ItemRarity rarity)
    {
        if (shieldDuration == 0)
            shieldDuration = GetRandomDuration(rarity);

        return $"Gain a barrier that blocks any damage for {shieldDuration} seconds.";
    }
    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        Player player = FindFirstObjectByType<Player>();
        CoroutineRunner.Instance.RunCoroutine(SpawnShield(player));
    }

    IEnumerator SpawnShield(Player player)
    {
        Vector3 offset = new Vector3(0, 1, 0);
        GameObject shieldObj = Instantiate(shieldPrefab, player.transform.position + offset, Quaternion.identity, player.transform);
        player.isInvulnerable = true;
        yield return new WaitForSeconds(shieldDuration);
        player.isInvulnerable = false;
        Destroy(shieldObj);
    }

    #region RANDOM VALUE GENERATOR
    private float GetRandomDuration(R_ItemRarity rarity)
    {
        return rarity switch
        {
            R_ItemRarity.Common => Random.Range(0.75f, 1.5f),
            R_ItemRarity.Rare => Random.Range(1f, 2f),
            R_ItemRarity.Epic => Random.Range(1.25f, 2.5f),
            R_ItemRarity.Legendary => Random.Range(1.5f, 3f),
            _ => 0.75f
        };
    }
    #endregion
}
