
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(R_ItemData))]
public class R_ItemDataEditor : Editor
{
    // 🔹 Cache SerializedProperties
    private SerializedProperty itemNameProp;
    private SerializedProperty itemIconProp;
    private SerializedProperty itemBackdropIconProp;
    private SerializedProperty inventoryHeaderImageProp;
    private SerializedProperty inventoryBackdropImageProp;
    private SerializedProperty descriptionProp;
    private SerializedProperty itemTypeProp;
    private SerializedProperty isConsumableProp;
    private SerializedProperty isStackableProp;
    private SerializedProperty maxStackSizeProp;
    private SerializedProperty rarityProp;
    private SerializedProperty pamanaDataProp;
    private SerializedProperty setProp;
    private SerializedProperty twoPieceBonusDescriptionProp;
    private SerializedProperty threePieceBonusDescriptionProp;
    private SerializedProperty pamanaSlotProp;
    private SerializedProperty slot1AbilityProp;
    private SerializedProperty slot2AbilityProp;
    private SerializedProperty effectTextProp;
    private SerializedProperty consumableEffectProp;
    private SerializedProperty upgradeMaterialTypeProp;
    private SerializedProperty xpValueProp;
    private SerializedProperty itemCostProp;

    private void OnEnable()
    {
        itemNameProp = serializedObject.FindProperty("itemName");
        itemIconProp = serializedObject.FindProperty("itemIcon");
        itemBackdropIconProp = serializedObject.FindProperty("itemBackdropIcon");
        inventoryHeaderImageProp = serializedObject.FindProperty("inventoryHeaderImage");
        inventoryBackdropImageProp = serializedObject.FindProperty("inventoryBackdropImage");
        descriptionProp = serializedObject.FindProperty("description");
        itemTypeProp = serializedObject.FindProperty("itemType");
        isConsumableProp = serializedObject.FindProperty("isConsumable");
        isStackableProp = serializedObject.FindProperty("isStackable");
        maxStackSizeProp = serializedObject.FindProperty("maxStackSize");
        rarityProp = serializedObject.FindProperty("rarity");
        pamanaDataProp = serializedObject.FindProperty("pamanaData");
        setProp = serializedObject.FindProperty("set");
        pamanaSlotProp = serializedObject.FindProperty("pamanaSlot");
        twoPieceBonusDescriptionProp = serializedObject.FindProperty("twoPieceBonusDescription");
        threePieceBonusDescriptionProp = serializedObject.FindProperty("threePieceBonusDescription");
        slot1AbilityProp = serializedObject.FindProperty("slot1Ability");
        slot2AbilityProp = serializedObject.FindProperty("slot2Ability");
        effectTextProp = serializedObject.FindProperty("effectText");
        consumableEffectProp = serializedObject.FindProperty("consumableEffect");
        upgradeMaterialTypeProp = serializedObject.FindProperty("upgradeMaterialType");
        xpValueProp = serializedObject.FindProperty("xpValue");
        itemCostProp = serializedObject.FindProperty("itemCost");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        R_ItemData data = (R_ItemData)target;

        EditorGUILayout.PropertyField(itemNameProp);
        EditorGUILayout.PropertyField(itemIconProp);
        EditorGUILayout.PropertyField(itemBackdropIconProp);
        EditorGUILayout.PropertyField(inventoryHeaderImageProp);
        EditorGUILayout.PropertyField(inventoryBackdropImageProp);
        EditorGUILayout.PropertyField(descriptionProp);
        EditorGUILayout.PropertyField(itemTypeProp);

        if (data.itemType == R_ItemType.QuestItem)
        {
            EditorGUILayout.PropertyField(isConsumableProp);
        }
        if (data.itemType == R_ItemType.Consumable)
        {
            EditorGUILayout.PropertyField(effectTextProp);
            EditorGUILayout.PropertyField(consumableEffectProp);
        }

        if (data.itemType != R_ItemType.Pamana && data.itemType != R_ItemType.Agimat)
        {
            EditorGUILayout.PropertyField(isStackableProp);
            if (data.isStackable)
            {
                EditorGUILayout.PropertyField(maxStackSizeProp);
            }
        }

        if (data.itemType != R_ItemType.Pamana)
        {
            EditorGUILayout.PropertyField(rarityProp);
        }

        if (data.itemType == R_ItemType.Pamana)
        {
            EditorGUILayout.PropertyField(pamanaDataProp);
            EditorGUILayout.PropertyField(setProp);
            EditorGUILayout.PropertyField(pamanaSlotProp);
            EditorGUILayout.PropertyField(twoPieceBonusDescriptionProp);
            EditorGUILayout.PropertyField(threePieceBonusDescriptionProp);

            if (data.pamanaData == null && GUILayout.Button("Generate Pamana Stats"))
            {
                R_PamanaData pamanaData = R_PamanaGeneratorUtility.GenerateFrom(data);
                data.pamanaData = pamanaData;
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
                Debug.Log($"Generated PamanaData for: {data.name}");
            }
        }

        if (data.itemType == R_ItemType.Agimat)
        {
            EditorGUILayout.PropertyField(slot1AbilityProp);
            EditorGUILayout.PropertyField(slot2AbilityProp);
            // 🔹 Rarity is assigned automatically — no need to show
        }


        if (data.itemType == R_ItemType.UpgradeMaterial)
        {
            EditorGUILayout.PropertyField(upgradeMaterialTypeProp);
            EditorGUILayout.PropertyField(xpValueProp);
        }
        
        if (data.itemType == R_ItemType.Consumable || data.itemType == R_ItemType.UpgradeMaterial)
        {
            EditorGUILayout.PropertyField(itemCostProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
