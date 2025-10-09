using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chest))]
public class ChestContent_Editor : Editor
{
    SerializedProperty chestDropsProp;
    SerializedProperty explosionForceProp;
    SerializedProperty upwardModifierProp;
    SerializedProperty spawnOffsetProp;

    bool[] foldouts; // for collapsible entries

    private void OnEnable()
    {
        chestDropsProp = serializedObject.FindProperty("chestDrops");
        explosionForceProp = serializedObject.FindProperty("explosionForce");
        upwardModifierProp = serializedObject.FindProperty("upwardModifier");
        spawnOffsetProp = serializedObject.FindProperty("spawnOffset");

        // Initialize foldouts
        foldouts = new bool[chestDropsProp.arraySize];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // =============================
        // 🧨 Loot Physics Settings
        // =============================
        EditorGUILayout.LabelField("Loot Physics Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.PropertyField(explosionForceProp, new GUIContent("Explosion Force"));
        EditorGUILayout.PropertyField(upwardModifierProp, new GUIContent("Upward Modifier"));
        EditorGUILayout.PropertyField(spawnOffsetProp, new GUIContent("Spawn Offset"));

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // =============================
        // 💰 Chest Drops Section
        // =============================
        EditorGUILayout.LabelField("Chest Drops", EditorStyles.boldLabel);

        // Ensure foldouts array matches chestDrops size
        if (foldouts.Length != chestDropsProp.arraySize)
            foldouts = new bool[chestDropsProp.arraySize];

        EditorGUI.indentLevel++;

        for (int i = 0; i < chestDropsProp.arraySize; i++)
        {
            SerializedProperty element = chestDropsProp.GetArrayElementAtIndex(i);

            SerializedProperty lootDropPrefab = element.FindPropertyRelative("lootPrefab");
            SerializedProperty itemData = element.FindPropertyRelative("itemData");
            SerializedProperty randomPamana = element.FindPropertyRelative("randomPamana");
            SerializedProperty randomAgimat = element.FindPropertyRelative("randomAgimat");
            SerializedProperty isGold = element.FindPropertyRelative("isGold");
            SerializedProperty randomGold = element.FindPropertyRelative("randomGold");
            SerializedProperty goldAmount = element.FindPropertyRelative("goldAmount");
            SerializedProperty randomGoldMinMax = element.FindPropertyRelative("randomGoldMinMax");

            // Visual box and foldout
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Chest Drop {i + 1}", true, EditorStyles.foldoutHeader);
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                chestDropsProp.DeleteArrayElementAtIndex(i);
                break; // stop drawing deleted element
            }
            EditorGUILayout.EndHorizontal();

            if (foldouts[i])
            {
                EditorGUI.indentLevel++;

                bool hasItemData = itemData.objectReferenceValue != null;
                bool isRandomPamana = randomPamana.boolValue;
                bool isRandomAgimat = randomAgimat.boolValue;
                bool goldMode = isGold.boolValue;

                // Always show loot prefab
                EditorGUILayout.PropertyField(lootDropPrefab, new GUIContent("Loot Prefab"));

                // Conditional Logic
                if (isRandomPamana)
                {
                    EditorGUILayout.PropertyField(randomPamana, new GUIContent("Random Pamana"));
                }
                else if (isRandomAgimat)
                {
                    EditorGUILayout.PropertyField(randomAgimat, new GUIContent("Random Agimat"));
                }
                else if (hasItemData)
                {
                    EditorGUILayout.PropertyField(itemData, new GUIContent("Item Data"));
                }
                else if (goldMode)
                {
                    EditorGUILayout.PropertyField(isGold, new GUIContent("Is Gold"));

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(randomGold, new GUIContent("Random Gold"));

                    if (randomGold.boolValue)
                        EditorGUILayout.PropertyField(randomGoldMinMax, new GUIContent("Gold Range"));
                    else
                        EditorGUILayout.PropertyField(goldAmount, new GUIContent("Gold Amount"));

                    EditorGUI.indentLevel--;
                }
                else
                {
                    // Default view
                    EditorGUILayout.PropertyField(itemData, new GUIContent("Item Data"));
                    EditorGUILayout.Space(3);
                    EditorGUILayout.PropertyField(randomPamana, new GUIContent("Random Pamana"));
                    EditorGUILayout.PropertyField(randomAgimat, new GUIContent("Random Agimat"));
                    EditorGUILayout.PropertyField(isGold, new GUIContent("Is Gold"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Add New Drop"))
            chestDropsProp.InsertArrayElementAtIndex(chestDropsProp.arraySize);

        serializedObject.ApplyModifiedProperties();
    }
}
