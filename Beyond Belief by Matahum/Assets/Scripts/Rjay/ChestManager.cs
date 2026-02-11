using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour, ISaveable
{
    public static ChestManager Instance { get; private set; }

    private List<Chest> chests = new List<Chest>();
    private ChestSaveData pendingData;

    // ✅ Determines if Lewenri chests are available
    public bool CanOpenLewenriChests { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SaveManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Unregister(this);
    }

    private void Start()
    {
        StartCoroutine(InitializeChests());
    }

    private System.Collections.IEnumerator InitializeChests()
    {
        // Wait one frame so everything (colliders, animations, etc.) initializes
        yield return null;

        chests.Clear();
        chests.AddRange(GetComponentsInChildren<Chest>());
        Debug.Log($"[ChestManager] Found {chests.Count} chests.");

        // Apply cached save data if loading happened before chests were ready
        if (pendingData != null)
        {
            Debug.Log("[ChestManager] Applying cached chest save data...");
            ApplyData(pendingData);
            pendingData = null;
        }
        else
        {
            foreach (var c in chests)
                c?.ApplySavedState();
        }

        // 🧩 After all chests have initialized, re-apply correct layers
        yield return null; // ensure all Start()s have run
        EnforceChestLayers();
    }

    public string SaveId => "ChestManager";

    [Serializable]
    private class ChestSaveData
    {
        public List<string> chestIds = new List<string>();
        public List<bool> openedStates = new List<bool>();
        public bool canOpenLewenriChests;
    }

    public string CaptureJson()
    {
        var data = new ChestSaveData();

        foreach (var chest in chests)
        {
            if (chest == null) continue;
            data.chestIds.Add(chest.ChestId);
            data.openedStates.Add(chest.isOpened);
        }

        data.canOpenLewenriChests = CanOpenLewenriChests;
        return JsonUtility.ToJson(data);
    }

    public void RestoreFromJson(string json)
    {
        var data = JsonUtility.FromJson<ChestSaveData>(json);
        if (data == null) return;

        if (chests == null || chests.Count == 0)
        {
            pendingData = data;
            Debug.Log("[ChestManager] Chests not ready yet — caching data for later.");
            return;
        }

        ApplyData(data);
    }

    private void ApplyData(ChestSaveData data)
    {
        for (int i = 0; i < data.chestIds.Count; i++)
        {
            string id = data.chestIds[i];
            bool opened = data.openedStates[i];

            var chest = chests.Find(c => c.ChestId == id);
            if (chest != null)
            {
                chest.isOpened = opened;
                chest.ApplySavedState();
            }
        }

        CanOpenLewenriChests = data.canOpenLewenriChests;
        EnforceChestLayers();

        Debug.Log($"[ChestManager] Restored {data.chestIds.Count} chests. Lewenri unlock = {CanOpenLewenriChests}");
    }

    // ✅ Called by your QuestManager when the Lewenri quest is completed
    public void OnLewenriQuestComplete()
    {
        CanOpenLewenriChests = true;
        EnforceChestLayers();
        Debug.Log("[ChestManager] Lewenri chests are now lootable!");
    }

    /// <summary>
    /// Ensures all chest layers match their current state after load or quest updates.
    /// </summary>
    private void EnforceChestLayers()
    {
        foreach (var chest in chests)
        {
            if (chest == null) continue;

            // Opened chests = Default (no interaction)
            if (chest.isOpened)
            {
                chest.gameObject.layer = LayerMask.NameToLayer("Default");
                Collider col = chest.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                continue;
            }

            // Lewenri chests respect quest progress
            if (chest.IsLewenriChest)
            {
                if (CanOpenLewenriChests)
                {
                    chest.gameObject.layer = LayerMask.NameToLayer("Loot");
                }
                else
                {
                    chest.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
            else
            {
                // Normal chests stay lootable until opened
                chest.gameObject.layer = LayerMask.NameToLayer("Loot");
            }
        }

        Debug.Log("[ChestManager] Chest layers enforced.");
    }

    public Chest GetChestById(string id)
    {
        return chests.Find(c => c.ChestId == id);
    }
}
