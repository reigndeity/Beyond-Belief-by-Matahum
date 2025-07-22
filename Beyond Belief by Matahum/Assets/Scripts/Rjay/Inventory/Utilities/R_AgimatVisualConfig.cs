using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Visual Config")]
public class R_AgimatVisualConfig : ScriptableObject
{
    public List<R_AgimatVisualSet> visualSets;

    public R_AgimatVisualSet GetVisualSet(R_ItemRarity rarity)
    {
        foreach (var set in visualSets)
        {
            if (set.rarity == rarity)
                return set;
        }

        Debug.LogWarning($"No AgimatVisualSet found for rarity: {rarity}. Defaulting to Common.");
        return visualSets.Find(set => set.rarity == R_ItemRarity.Common);
    }

    public Sprite GetItemBackdrop(R_ItemRarity rarity) => GetVisualSet(rarity).itemBackdropIcon;
    public Sprite GetHeaderImage(R_ItemRarity rarity) => GetVisualSet(rarity).inventoryHeaderImage;
    public Sprite GetBackdropImage(R_ItemRarity rarity) => GetVisualSet(rarity).inventoryBackdropImage;
}
