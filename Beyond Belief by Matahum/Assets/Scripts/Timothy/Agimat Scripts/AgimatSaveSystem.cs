using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SavedAgimatEntry
{
    public string agimatName;
    public int slotIndex;
    public bool isEquipped;
}

[System.Serializable]
public class SavedAgimatData
{
    public List<SavedAgimatEntry> items = new List<SavedAgimatEntry>();
}

public class AgimatSaveSystem : MonoBehaviour
{
    public GameObject[] allAgimatPrefabs;
    public Inventory agimatInventory;
    public List<ItemSlot> equippedSlots = new List<ItemSlot>();

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SavedAgimatData.json");
    }

    private void Start()
    {
        LoadAgimats();
    }

    private void OnApplicationQuit()
    {
        SaveAgimats();
        Debug.Log("Saved Agimat Positions");
    }

    public void SaveAgimats()
    {
        SavedAgimatData saveData = new SavedAgimatData();

        // Save equipped Agimats
        for (int i = 0; i < equippedSlots.Count; i++)
        {
            InventoryItem item = equippedSlots[i].currentItem;
            if (item != null)
            {
                string agimatName = item.name.Replace("(Clone)", "").Trim();
                saveData.items.Add(new SavedAgimatEntry
                {
                    agimatName = agimatName,
                    slotIndex = i,
                    isEquipped = true
                });
            }
        }

        // Save Agimats in inventory
        for (int i = 0; i < agimatInventory.slots.Count; i++)
        {
            InventoryItem item = agimatInventory.slots[i].currentItem;
            if (item != null)
            {
                string agimatName = item.name.Replace("(Clone)", "").Trim();
                saveData.items.Add(new SavedAgimatEntry
                {
                    agimatName = agimatName,
                    slotIndex = i,
                    isEquipped = false
                });
            }
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadAgimats()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        SavedAgimatData loadedData = JsonUtility.FromJson<SavedAgimatData>(json);

        foreach (var entry in loadedData.items)
        {
            GameObject prefab = FindAgimatPrefab(entry.agimatName);
            if (prefab == null) continue;

            GameObject instance = Instantiate(prefab);
            SetupRectTransform(instance);
            InventoryItem agimatItem = instance.GetComponent<InventoryItem>();

            if (entry.isEquipped)
            {
                if (entry.slotIndex >= 0 && entry.slotIndex < equippedSlots.Count)
                {
                    ItemSlot targetSlot = equippedSlots[entry.slotIndex];
                    instance.transform.SetParent(targetSlot.transform, false);
                    targetSlot.currentItem = agimatItem;
                }
            }
            else
            {
                while (agimatInventory.slots.Count <= entry.slotIndex)
                    agimatInventory.AddSlot(true);

                ItemSlot targetSlot = agimatInventory.slots[entry.slotIndex];
                instance.transform.SetParent(targetSlot.transform, false);
                targetSlot.currentItem = agimatItem;
            }
        }
    }

    private GameObject FindAgimatPrefab(string name)
    {
        foreach (GameObject prefab in allAgimatPrefabs)
        {
            if (prefab.name == name)
                return prefab;
        }
        return null;
    }

    private void SetupRectTransform(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;
        }
    }
}
