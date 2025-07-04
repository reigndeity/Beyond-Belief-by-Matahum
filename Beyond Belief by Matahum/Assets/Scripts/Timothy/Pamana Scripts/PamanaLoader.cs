using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PamanaLoader : MonoBehaviour
{
    public List<Pamana> loadedPamanas = new List<Pamana>();
    public GameObject pamanaUIPrefab;
    public Inventory inventory;
    public List<ItemSlot> PamanaEquipmentSlots = new List<ItemSlot>();
    private GeneralItemSaveSystem saveSystem;

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/PamanaInventory";
    }

    private void Start()
    {
        LoadSavedPamanas();

        saveSystem = GetComponent<GeneralItemSaveSystem>();
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
}