
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Inventory/Pamana Visual Config")]
public class R_PamanaVisualConfig : ScriptableObject
{
    public List<R_PamanaVisualSet> visualSets;

    public R_PamanaVisualSet GetVisuals(R_ItemRarity rarity)
    {
        return visualSets.Find(set => set.rarity == rarity);
    }
}
