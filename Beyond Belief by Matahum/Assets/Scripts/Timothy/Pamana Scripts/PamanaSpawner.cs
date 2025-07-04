using UnityEngine;
using System.IO;

public class PamanaSpawner : MonoBehaviour
{
    [Header("Pamana Item Prefab and Main Inventory")]
    public GameObject pamanaUIPrefab;
    public Inventory mainInventory;

    [Header("Pamana that can spawn")]
    public Pamana[] availablePamanas; //Scriptable Object
    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/GeneralItems";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    private void Start()
    {
        mainInventory = GameObject.Find("Main Inventory").GetComponent<Inventory>();
    }
    public PamanaUI SpawnPamana(GameObject prefabParent)
    {
        if (availablePamanas.Length == 0)
        {
            Debug.LogError("No Pamana assets assigned to the spawner!");
        }

        // 1️⃣ Pick a random base Pamana
        Pamana basePamana = availablePamanas[Random.Range(0, availablePamanas.Length)];

        // 2️⃣ Create a unique instance
        Pamana newPamana = ScriptableObject.CreateInstance<Pamana>();

        // 3️⃣ Assign a unique ID immediately
        newPamana.uniqueID = System.Guid.NewGuid().ToString();

        // 4️⃣ Copy data from the base Pamana
        newPamana.icon = basePamana.icon;
        newPamana.pamanaName = basePamana.pamanaName;
        newPamana.type = basePamana.type;
        newPamana.setPiece = basePamana.setPiece;

        // 5️⃣ Generate unique stats
        newPamana.DetermineRarity();
        newPamana.GenerateRandomStats();

        // 6️⃣ Save as JSON with correct UID
        SavePamanaJson(newPamana);

        // 7️⃣ Instantiate UI prefab
        GameObject pamanaUI = Instantiate(pamanaUIPrefab, prefabParent.transform);

        // 8️⃣ Assign the saved Pamana to the UI
        PamanaUI pamanaUIScript = pamanaUI.GetComponent<PamanaUI>();
        pamanaUIScript.Initialize(newPamana);
        pamanaUI.name = pamanaUIScript.pamanaName;

        return pamanaUIScript;
    }

    public void InsertToInventory(PamanaUI pamanaUIScript)
    {
        GameObject pamanaUI = pamanaUIScript.gameObject;

        // 🔍 Check if all slots are occupied
        bool allOccupied = true;
        foreach (var slot in mainInventory.slots)
        {
            if (slot.GetComponentInChildren<InventoryItem>() == null)
            {
                allOccupied = false;
                break;
            }
        }

        // ➕ Add a slot only if all are full
        if (allOccupied)
        {
            mainInventory.AddSlot();
        }

        // 🔁 Try to place the item into the first empty slot
        foreach (var slot in mainInventory.slots)
        {
            if (slot.GetComponentInChildren<InventoryItem>() == null)
            {
                slot.currentItem = pamanaUI.GetComponent<InventoryItem>();

                pamanaUI.transform.SetParent(slot.transform, false);
                pamanaUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                pamanaUI.GetComponent<RectTransform>().localScale = Vector3.one;
                return;
            }
        }

        Debug.LogWarning("InsertToInventory: No empty slots found after adding one.");
    }

    public void SpawnPamanaInventoryItem()
    {
        PamanaUI spawnedPamanaUI = SpawnPamana(transform.gameObject);
        InsertToInventory(spawnedPamanaUI);
    }


    void SavePamanaJson(Pamana pamana)
    {
        if (string.IsNullOrEmpty(pamana.uniqueID))
        {
            Debug.LogError("Pamana has no unique ID! Cannot save.");
            return;
        }

        PamanaData data = new PamanaData(pamana);
        string json = data.ToJson();
        string filePath = Path.Combine(savePath, $"{pamana.uniqueID}.json");
        File.WriteAllText(filePath, json);
        Debug.Log($"Pamana saved: {filePath}");
    }

}