// GeneralItemSaveSystem.cs

/*using UnityEngine;
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

        LoadSavedPamanas();
    }

    private void Start()
    {
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
}*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GeneralItemSaveSystem : MonoBehaviour
{
    [Header("All Inventory Slots")]
    public Inventory mainInventory;
    public Inventory agimatInventory;

    [Header("Equipment Slots")]
    public List<ItemSlot> pamanaEquipSlots;
    public List<ItemSlot> agimatEquipSlots;
    public ItemSlot miscEquipSlot;

    [Header("Prefabs")]
    public GameObject pamanaUIPrefab;
    public List<GameObject> allGeneralItemPrefabs;
    public List<GameObject> allAgimatPrefabs;

    private string pamanaFolder;
    private string savePath;

    public List<Pamana> loadedPamanas = new();

    void Awake()
    {
        pamanaFolder = Path.Combine(Application.persistentDataPath, "GeneralItems");
        if (!Directory.Exists(pamanaFolder)) Directory.CreateDirectory(pamanaFolder);

        savePath = Path.Combine(Application.persistentDataPath, "UnifiedItems.json");

        LoadSavedPamanas(); // Load ScriptableObjects first
    }

    void Start() => LoadItems();

    void OnApplicationQuit() => SaveItems();

    public void SaveItems()
    {
        List<UnifiedItemSaveData> savedItems = new();

        // Save Pamana equipped
        foreach (var slot in pamanaEquipSlots)
        {
            if (slot.currentItem != null)
            {
                var pamanaUI = slot.currentItem.GetComponent<PamanaUI>();
                if (pamanaUI?.assignedPamana != null)
                {
                    savedItems.Add(new UnifiedItemSaveData(
                        pamanaUI.assignedPamana.uniqueID, 1, slot.transform.name, true, false
                    ));
                }
            }
        }

        // Save Agimat equipped
        for (int i = 0; i < agimatEquipSlots.Count; i++)
        {
            var item = agimatEquipSlots[i].currentItem;
            if (item != null)
            {
                savedItems.Add(new UnifiedItemSaveData(
                    item.name.Replace("(Clone)", "").Trim(), 1, agimatEquipSlots[i].transform.name, false, true
                ));
            }
        }

        // Save misc equipped
        if (miscEquipSlot.currentItem != null)
        {
            var item = miscEquipSlot.currentItem;
            savedItems.Add(new UnifiedItemSaveData(item.itemID, item.quantity, miscEquipSlot.transform.name, false, false));
        }

        // Save inventory: Main (Pamana + General)
        for (int i = 0; i < mainInventory.slots.Count; i++)
        {
            var item = mainInventory.slots[i].currentItem;
            if (item == null) continue;

            if (item.itemType == ItemType.Pamana)
            {
                var pamanaUI = item.GetComponent<PamanaUI>();
                if (pamanaUI?.assignedPamana != null)
                {
                    savedItems.Add(new UnifiedItemSaveData(pamanaUI.assignedPamana.uniqueID, 1, null, true, false, i));
                }
            }
            else
            {
                savedItems.Add(new UnifiedItemSaveData(item.itemID, item.quantity, null, false, false, i));
            }
        }

        // Save inventory: Agimat
        for (int i = 0; i < agimatInventory.slots.Count; i++)
        {
            var item = agimatInventory.slots[i].currentItem;
            if (item == null) continue;

            savedItems.Add(new UnifiedItemSaveData(item.name.Replace("(Clone)", "").Trim(), 1, null, false, true, i));
        }

        File.WriteAllText(savePath, JsonUtility.ToJson(new UnifiedItemSaveList(savedItems), true));
    }

    public void LoadItems()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<UnifiedItemSaveList>(json);

        foreach (var itemData in data.items)
        {
            GameObject instance = null;
            InventoryItem itemScript = null;

            // PAMANA
            if (itemData.isPamana)
            {
                Pamana matchedPamana = loadedPamanas.Find(p => p.uniqueID == itemData.itemID);
                if (matchedPamana == null) continue;

                instance = Instantiate(pamanaUIPrefab);
                var ui = instance.GetComponent<PamanaUI>();
                ui.Initialize(matchedPamana);
                itemScript = instance.GetComponent<InventoryItem>();
            }
            // AGIMAT
            else if (itemData.isAgimat)
            {
                var prefab = FindPrefabByName(allAgimatPrefabs, itemData.itemID);
                if (prefab == null) continue;

                instance = Instantiate(prefab);
                itemScript = instance.GetComponent<InventoryItem>();
            }
            // GENERAL
            else
            {
                var prefab = FindPrefabByID(allGeneralItemPrefabs, itemData.itemID);
                if (prefab == null) continue;

                instance = Instantiate(prefab);
                itemScript = instance.GetComponent<InventoryItem>();
                itemScript.SetQuantity(itemData.quantity);
            }

            // Assign slot
            if (!string.IsNullOrEmpty(itemData.slotName))
            {
                ItemSlot target = FindSlotByName(itemData.slotName);
                if (target != null)
                {
                    instance.transform.SetParent(target.transform, false);
                    ResetItemRect(instance);
                    target.currentItem = itemScript;
                    itemScript.name = itemScript.itemID;
                }
            }
            else if (itemData.inventoryIndex >= 0)
            {
                bool isAgimat = itemData.isAgimat;
                Inventory inv = isAgimat ? agimatInventory : mainInventory;

                while (inv.slots.Count <= itemData.inventoryIndex)
                    inv.AddSlot(isAgimat);

                var target = inv.slots[itemData.inventoryIndex];
                instance.transform.SetParent(target.transform, false);
                ResetItemRect(instance);
                target.currentItem = itemScript;
                if (!inv.items.Contains(itemScript))
                    inv.items.Add(itemScript); // or Insert at itemData.inventoryIndex if ordering matters
                itemScript.name = itemScript.itemID;                           
            }

            if (itemScript.itemType == ItemType.Agimat)
            {
                if (!InventoryManager.Instance.agimatItems.Contains(itemScript))
                    InventoryManager.Instance.agimatItems.Add(itemScript);
            }
            else
            {
                if (!InventoryManager.Instance.mainInventoryItems.Contains(itemScript))
                    InventoryManager.Instance.mainInventoryItems.Add(itemScript);
            }

        }


    }

    private GameObject FindPrefabByID(List<GameObject> list, string id)
    {
        foreach (var go in list)
        {
            var item = go.GetComponent<InventoryItem>();
            if (item != null && item.itemID == id) return go;
        }
        return null;
    }

    private GameObject FindPrefabByName(List<GameObject> list, string name)
    {
        foreach (var go in list)
            if (go.name == name) return go;
        return null;
    }

    private ItemSlot FindSlotByName(string slotName)
    {
        foreach (var s in pamanaEquipSlots) if (s.transform.name == slotName) return s;
        foreach (var s in agimatEquipSlots) if (s.transform.name == slotName) return s;
        if (miscEquipSlot != null && miscEquipSlot.transform.name == slotName) return miscEquipSlot;
        foreach (var s in mainInventory.slots) if (s.transform.name == slotName) return s;
        foreach (var s in agimatInventory.slots) if (s.transform.name == slotName) return s;
        return null;
    }

    private void ResetItemRect(GameObject item)
    {
        var rt = item.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
    }

    private void LoadSavedPamanas()
    {
        if (!Directory.Exists(pamanaFolder)) return;

        string[] files = Directory.GetFiles(pamanaFolder, "*.json");
        foreach (var file in files)
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

    [System.Serializable]
    public class UnifiedItemSaveData
    {
        public string itemID;
        public int quantity;
        public string slotName;
        public bool isPamana;
        public bool isAgimat;
        public int inventoryIndex;

        public UnifiedItemSaveData(
            string id,
            int qty = 1,
            string slot = null,
            bool pamana = false,
            bool agimat = false,
            int index = -1)
        {
            itemID = id;
            quantity = qty;
            slotName = slot;
            isPamana = pamana;
            isAgimat = agimat;
            inventoryIndex = index;
        }
    }

    [System.Serializable]
    public class UnifiedItemSaveList
    {
        public List<UnifiedItemSaveData> items;

        public UnifiedItemSaveList(List<UnifiedItemSaveData> list)
        {
            items = list;
        }
    }

}
