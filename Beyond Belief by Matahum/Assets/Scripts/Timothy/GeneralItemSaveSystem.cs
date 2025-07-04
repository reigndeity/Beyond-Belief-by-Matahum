/*using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GeneralItemSaveSystem : MonoBehaviour
{
    [Header("Main Inventory")]
    public Inventory inventory;

    [Header("Pamana Save and Load")]
    public List<ItemSlot> pamanaEquipmentSlots; // Pamana equipment slots
    public List<Pamana> loadedPamanas = new List<Pamana>();
    public GameObject pamanaUIPrefab; // Prefab to instantiate

    [Header("Other items Save and Load")]
    public ItemSlot miscEquipSlot; // Misc equipment slot
    public List<GameObject> allItemPrefabs; // Prefabs for general items

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/GeneralItems";
    }

    private void Start()
    {
        LoadItems();
        LoadSavedPamanas();
    }

    private void OnApplicationQuit()
    {
        SaveItems();
    }

    public void SaveItems()
    {
        List<UnifiedItemSaveData> saveDataList = new List<UnifiedItemSaveData>();

        // Save Pamana Equipment
        foreach (ItemSlot slot in pamanaEquipmentSlots)
        {
            if (slot.currentItem != null)
            {
                PamanaUI pamanaUI = slot.currentItem.GetComponent<PamanaUI>();
                if (pamanaUI != null && pamanaUI.assignedPamana != null)
                {
                    saveDataList.Add(new UnifiedItemSaveData(pamanaUI.assignedPamana.uniqueID, 1, slot.transform.name, true));
                    Debug.Log($"Saved Pamana {pamanaUI.name}");
                }
            }
        }

        // Save General Equipped Misc
        if (miscEquipSlot != null && miscEquipSlot.currentItem != null)
        {
            InventoryItem item = miscEquipSlot.currentItem;
            if (item.itemType == ItemType.Misc && item.isEquippable)
            {
                saveDataList.Add(new UnifiedItemSaveData(item.itemID, item.quantity, miscEquipSlot.transform.name, false));
                Debug.Log($"Saved Pamana {item.name}");
            }
        }

        // Save all inventory items
        for (int i = 0; i < inventory.slots.Count; i++)
        {
            ItemSlot slot = inventory.slots[i];
            if (slot.currentItem == null) continue;

            InventoryItem item = slot.currentItem;

            if (item.itemType == ItemType.Pamana)
            {
                PamanaUI pamanaUI = item.GetComponent<PamanaUI>();
                if (pamanaUI != null && pamanaUI.assignedPamana != null)
                {
                    saveDataList.Add(new UnifiedItemSaveData(pamanaUI.assignedPamana.uniqueID, 1, null, true, i));
                    Debug.Log($"Saved Pamana {item.name}");
                }
            }
            else
            {
                saveDataList.Add(new UnifiedItemSaveData(item.itemID, item.quantity, null, false, i));
                Debug.Log($"Saved Pamana {item.name}");
            }
        }

        string json = JsonUtility.ToJson(new UnifiedItemSaveList(saveDataList), true);
        File.WriteAllText(savePath, json);
    }

    public void LoadItems()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        UnifiedItemSaveList data = JsonUtility.FromJson<UnifiedItemSaveList>(json);

        foreach (UnifiedItemSaveData itemData in data.items)
        {
            if (itemData.isPamana)
            {
                Pamana foundPamana = loadedPamanas.Find(p => p.uniqueID == itemData.itemID);
                if (foundPamana == null) continue;

                GameObject pamanaUI = Instantiate(pamanaUIPrefab);
                PamanaUI pamanaUIScript = pamanaUI.GetComponent<PamanaUI>();
                pamanaUIScript.Initialize(foundPamana);
                pamanaUI.name = pamanaUIScript.pamanaName;

                InventoryItem bbItem = pamanaUI.GetComponent<InventoryItem>();

                //If Equipped
                if (!string.IsNullOrEmpty(itemData.slotName))
                {
                    ItemSlot targetSlot = FindSlotByName(itemData.slotName);
                    if (targetSlot != null && targetSlot.currentItem == null)
                    {
                        pamanaUI.transform.SetParent(targetSlot.transform, false);
                        ResetItemRect(pamanaUI);
                        targetSlot.currentItem = bbItem;
                        Debug.Log($"Loaded {pamanaUI.name}");
                    }
                }
                //If in inventory
                else if (itemData.inventoryIndex >= 0)
                {
                    while (inventory.slots.Count <= itemData.inventoryIndex)
                    {
                        inventory.AddSlot();
                        Debug.Log($"Added Slot to {inventory.name}");
                    }

                    ItemSlot slot = inventory.slots[itemData.inventoryIndex];
                    pamanaUI.transform.SetParent(slot.transform, false);
                    ResetItemRect(pamanaUI);
                    slot.currentItem = bbItem;
                    Debug.Log($"Loaded {pamanaUI.name}");
                }
            }
            else
            {
                GameObject prefab = GetItemPrefabByID(itemData.itemID);
                if (prefab == null) continue;

                GameObject itemGO = Instantiate(prefab);
                InventoryItem invItem = itemGO.GetComponent<InventoryItem>();
                invItem.itemID = itemData.itemID;
                invItem.SetQuantity(itemData.quantity);

                //If Equipped in Misc Slot
                if (!string.IsNullOrEmpty(itemData.slotName))
                {
                    itemGO.transform.SetParent(miscEquipSlot.transform, false);
                    ResetItemRect(itemGO);
                    miscEquipSlot.currentItem = invItem;
                    Debug.Log($"Loaded {invItem.name}");
                }
                //Items in Inventory
                else if (itemData.inventoryIndex >= 0)
                {
                    while (inventory.slots.Count <= itemData.inventoryIndex)
                    {
                        inventory.AddSlot(false);
                    }

                    ItemSlot slot = inventory.slots[itemData.inventoryIndex];
                    itemGO.transform.SetParent(slot.transform, false);
                    ResetItemRect(itemGO);
                    slot.currentItem = invItem;
                    Debug.Log($"Loaded {invItem.name}");
                }
            }
        }
    }
    void LoadSavedPamanas()
    {
        if (!Directory.Exists(savePath))
        {
            Debug.LogWarning("Pamana save directory not found.");
            return;
        }

        string[] files = Directory.GetFiles(savePath, "*.json");

        foreach (string file in files)
        {


            string json = File.ReadAllText(file);
            PamanaData data = PamanaData.FromJson(json);
            Pamana pamana = ScriptableObject.CreateInstance<Pamana>();

            pamana.icon = data.icon;
            pamana.uniqueID = data.uniqueID;
            pamana.pamanaName = data.pamanaName;
            pamana.type = (PamanaType)System.Enum.Parse(typeof(PamanaType), data.type);
            pamana.rarity = (PamanaRarity)System.Enum.Parse(typeof(PamanaRarity), data.rarity);
            pamana.setPiece = data.setPiece;
            pamana.pamanaLevel = data.pamanaLevel;
            pamana.mainStat = data.mainStat;
            pamana.subStat = data.subStat;
            pamana.randomMainValue = data.randomMainValue;
            pamana.randomSubValue = data.randomSubValue;

            loadedPamanas.Add(pamana);
        }
    }

    private void ResetItemRect(GameObject item)
    {
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private ItemSlot FindSlotByName(string slotName)
    {
        foreach (ItemSlot slot in pamanaEquipmentSlots)
            if (slot.transform.name == slotName) return slot;

        foreach (ItemSlot slot in inventory.slots)
            if (slot.transform.name == slotName) return slot;

        if (miscEquipSlot != null && miscEquipSlot.transform.name == slotName)
            return miscEquipSlot;

        return null;
    }

    private GameObject GetItemPrefabByID(string id)
    {
        foreach (GameObject prefab in allItemPrefabs)
        {
            InventoryItem item = prefab.GetComponent<InventoryItem>();
            if (item != null && item.itemID == id)
                return prefab;
        }
        return null;
    }
}*/


// GeneralItemSaveSystem.cs

using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GeneralItemSaveSystem : MonoBehaviour
{
    [Header("Main Inventory")]
    public Inventory inventory;

    [Header("Pamana Save and Load")]
    public List<ItemSlot> pamanaEquipmentSlots;
    public List<Pamana> loadedPamanas = new List<Pamana>();
    public GameObject pamanaUIPrefab;

    [Header("Other items Save and Load")]
    public ItemSlot miscEquipSlot;
    public List<GameObject> allItemPrefabs;

    private string pamanaSaveFolder;
    private string generalItemsSaveFile;

    private void Awake()
    {
        pamanaSaveFolder = Path.Combine(Application.persistentDataPath, "GeneralItems");
        if (!Directory.Exists(pamanaSaveFolder))
            Directory.CreateDirectory(pamanaSaveFolder);

        generalItemsSaveFile = Path.Combine(Application.persistentDataPath, "GeneralItems.json");
    }

    private void Start()
    {
        LoadSavedPamanas();
        LoadItems();
    }

    private void OnApplicationQuit()
    {
        SaveItems();
    }

    public void SaveItems()
    {
        List<UnifiedItemSaveData> saveDataList = new List<UnifiedItemSaveData>();

        foreach (ItemSlot slot in pamanaEquipmentSlots)
        {
            if (slot.currentItem != null)
            {
                PamanaUI pamanaUI = slot.currentItem.GetComponent<PamanaUI>();
                if (pamanaUI != null && pamanaUI.assignedPamana != null)
                {
                    saveDataList.Add(new UnifiedItemSaveData(pamanaUI.assignedPamana.uniqueID, 1, slot.transform.name, true));
                }
            }
        }

        if (miscEquipSlot != null && miscEquipSlot.currentItem != null)
        {
            InventoryItem item = miscEquipSlot.currentItem;
            if (item.itemType == ItemType.Misc && item.isEquippable)
            {
                saveDataList.Add(new UnifiedItemSaveData(item.itemID, item.quantity, miscEquipSlot.transform.name, false));
            }
        }

        for (int i = 0; i < inventory.slots.Count; i++)
        {
            ItemSlot slot = inventory.slots[i];
            if (slot.currentItem == null) continue;

            InventoryItem item = slot.currentItem;

            if (item.itemType == ItemType.Pamana)
            {
                PamanaUI pamanaUI = item.GetComponent<PamanaUI>();
                if (pamanaUI != null && pamanaUI.assignedPamana != null)
                {
                    saveDataList.Add(new UnifiedItemSaveData(pamanaUI.assignedPamana.uniqueID, 1, null, true, i));
                }
            }
            else
            {
                saveDataList.Add(new UnifiedItemSaveData(item.itemID, item.quantity, null, false, i));
            }
        }

        string json = JsonUtility.ToJson(new UnifiedItemSaveList(saveDataList), true);
        File.WriteAllText(generalItemsSaveFile, json);
    }

    public void LoadItems()
    {
        if (!File.Exists(generalItemsSaveFile)) return;

        string json = File.ReadAllText(generalItemsSaveFile);
        UnifiedItemSaveList data = JsonUtility.FromJson<UnifiedItemSaveList>(json);

        foreach (UnifiedItemSaveData itemData in data.items)
        {
            if (itemData.isPamana)
            {
                Pamana foundPamana = loadedPamanas.Find(p => p.uniqueID == itemData.itemID);
                if (foundPamana == null) continue;

                GameObject pamanaUI = Instantiate(pamanaUIPrefab);
                PamanaUI pamanaUIScript = pamanaUI.GetComponent<PamanaUI>();
                pamanaUIScript.Initialize(foundPamana);
                pamanaUI.name = pamanaUIScript.pamanaName;

                InventoryItem bbItem = pamanaUI.GetComponent<InventoryItem>();

                if (!string.IsNullOrEmpty(itemData.slotName))
                {
                    ItemSlot targetSlot = FindSlotByName(itemData.slotName);
                    if (targetSlot != null && targetSlot.currentItem == null)
                    {
                        pamanaUI.transform.SetParent(targetSlot.transform, false);
                        ResetItemRect(pamanaUI);
                        targetSlot.currentItem = bbItem;
                    }
                }
                else if (itemData.inventoryIndex >= 0)
                {
                    while (inventory.slots.Count <= itemData.inventoryIndex)
                        inventory.AddSlot();

                    ItemSlot slot = inventory.slots[itemData.inventoryIndex];
                    pamanaUI.transform.SetParent(slot.transform, false);
                    ResetItemRect(pamanaUI);
                    slot.currentItem = bbItem;
                }
            }
            else
            {
                GameObject prefab = GetItemPrefabByID(itemData.itemID);
                if (prefab == null) continue;

                GameObject itemGO = Instantiate(prefab);
                InventoryItem invItem = itemGO.GetComponent<InventoryItem>();
                invItem.itemID = itemData.itemID;
                invItem.SetQuantity(itemData.quantity);

                if (!string.IsNullOrEmpty(itemData.slotName))
                {
                    itemGO.transform.SetParent(miscEquipSlot.transform, false);
                    ResetItemRect(itemGO);
                    miscEquipSlot.currentItem = invItem;
                }
                else if (itemData.inventoryIndex >= 0)
                {
                    while (inventory.slots.Count <= itemData.inventoryIndex)
                        inventory.AddSlot(false);

                    ItemSlot slot = inventory.slots[itemData.inventoryIndex];
                    itemGO.transform.SetParent(slot.transform, false);
                    ResetItemRect(itemGO);
                    slot.currentItem = invItem;
                }
            }
        }
    }

    public void LoadSavedPamanas()
    {
        if (!Directory.Exists(pamanaSaveFolder)) return;

        string[] files = Directory.GetFiles(pamanaSaveFolder, "*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            PamanaData data = PamanaData.FromJson(json);
            Pamana pamana = ScriptableObject.CreateInstance<Pamana>();

            pamana.icon = data.icon;
            pamana.uniqueID = data.uniqueID;
            pamana.pamanaName = data.pamanaName;
            pamana.type = (PamanaType)System.Enum.Parse(typeof(PamanaType), data.type);
            pamana.rarity = (PamanaRarity)System.Enum.Parse(typeof(PamanaRarity), data.rarity);
            pamana.setPiece = data.setPiece;
            pamana.pamanaLevel = data.pamanaLevel;
            pamana.mainStat = data.mainStat;
            pamana.subStat = data.subStat;
            pamana.randomMainValue = data.randomMainValue;
            pamana.randomSubValue = data.randomSubValue;

            loadedPamanas.Add(pamana);
        }
    }

    private void ResetItemRect(GameObject item)
    {
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private ItemSlot FindSlotByName(string slotName)
    {
        foreach (ItemSlot slot in pamanaEquipmentSlots)
            if (slot.transform.name == slotName) return slot;

        foreach (ItemSlot slot in inventory.slots)
            if (slot.transform.name == slotName) return slot;

        if (miscEquipSlot != null && miscEquipSlot.transform.name == slotName)
            return miscEquipSlot;

        return null;
    }

    private GameObject GetItemPrefabByID(string id)
    {
        foreach (GameObject prefab in allItemPrefabs)
        {
            InventoryItem item = prefab.GetComponent<InventoryItem>();
            if (item != null && item.itemID == id)
                return prefab;
        }
        return null;
    }
}


[System.Serializable]
public class UnifiedItemSaveData
{
    public string itemID;
    public int quantity;
    public string slotName;
    public bool isPamana;
    public int inventoryIndex = -1;

    public UnifiedItemSaveData(string id, int qty = 1, string slot = null, bool pamana = false, int index = -1)
    {
        itemID = id;
        quantity = qty;
        slotName = slot;
        isPamana = pamana;
        inventoryIndex = index;
    }
}

[System.Serializable]
public class UnifiedItemSaveList
{
    public List<UnifiedItemSaveData> items = new List<UnifiedItemSaveData>();

    public UnifiedItemSaveList(List<UnifiedItemSaveData> list)
    {
        items = list;
    }
}
