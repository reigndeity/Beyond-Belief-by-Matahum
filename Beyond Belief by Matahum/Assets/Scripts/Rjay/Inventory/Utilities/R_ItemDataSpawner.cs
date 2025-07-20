using UnityEngine;

public static class R_ItemDataSpawner
{
    public static R_ItemData InstantiatePamanaTemplate(R_ItemData template)
    {
        if (template == null || template.itemType != R_ItemType.Pamana)
        {
            Debug.LogError("Template must be a valid Pamana item.");
            return null;
        }

        // Create a runtime clone of the ScriptableObject (non-asset instance)
        R_ItemData clone = ScriptableObject.CreateInstance<R_ItemData>();

        // Copy over metadata fields
        clone.itemName = template.itemName;
        clone.itemIcon = template.itemIcon;
        clone.itemBackdropIcon = template.itemBackdropIcon;
        clone.inventoryHeaderImage = template.inventoryHeaderImage;
        clone.inventoryBackdropImage = template.inventoryBackdropImage;
        clone.description = template.description;
        clone.itemType = R_ItemType.Pamana;
        clone.rarity = template.rarity;
        clone.set = template.set;
        clone.pamanaSlot = template.pamanaSlot;
        clone.twoPieceBonusDescription = template.twoPieceBonusDescription;
        clone.threePieceBonusDescription = template.threePieceBonusDescription;

        // Generate new stat block
        clone.pamanaData = R_PamanaGeneratorUtility.GenerateFrom(clone);

        return clone;
    }
}

