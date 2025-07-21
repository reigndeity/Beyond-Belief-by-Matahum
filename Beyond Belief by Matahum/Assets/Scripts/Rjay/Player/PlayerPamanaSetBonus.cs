using System.Collections.Generic;
using UnityEngine;

public class PlayerPamanaSetBonus : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerStats playerStats;
    
    void Awake()
    {
        player = GetComponent<Player>();
        playerStats = GetComponent<PlayerStats>();
    }
    public void ApplySetBonuses(Dictionary<R_PamanaSlotType, R_InventoryItem> equippedPamanas)
    {
        if (playerStats == null || player == null) return;

        // Step 1: Count equipped sets
        Dictionary<R_PamanaSet, int> setCounts = new();

        foreach (var kvp in equippedPamanas)
        {
            var item = kvp.Value;
            if (item?.itemData?.pamanaData == null)
                continue;

            var set = item.itemData.set;
            if (setCounts.ContainsKey(set))
                setCounts[set]++;
            else
                setCounts[set] = 1;
        }

        // Step 2: Apply bonuses
        foreach (var kvp in setCounts)
        {
            int count = kvp.Value;
            R_PamanaSet set = kvp.Key;

            if (count >= 3)
                ApplyThreePieceBonus(set);
            else if (count >= 2)
                ApplyTwoPieceBonus(set);
        }
    }

    private void ApplyTwoPieceBonus(R_PamanaSet set)
    {
        switch (set)
        {
            case R_PamanaSet.KatiwalaNgAraw:
                playerStats.p_atkPrcnt += 10f;
                break;
        }
    }

    private void ApplyThreePieceBonus(R_PamanaSet set)
    {
        switch (set)
        {
            case R_PamanaSet.KatiwalaNgAraw:
                playerStats.p_atkPrcnt += 20f;
                break;
        }
    }

    
}
