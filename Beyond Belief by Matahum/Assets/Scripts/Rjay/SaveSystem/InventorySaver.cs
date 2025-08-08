using System;
using System.Collections.Generic;
using UnityEngine;
using SaveSystemDTO;   // from InventoryDTOs.cs (InvItemDTO / InventoryDTO)

[DefaultExecutionOrder(-50)] // restore inventory before equipment
[DisallowMultipleComponent]
public class InventorySaver : MonoBehaviour, ISaveable
{
    [SerializeField] private R_Inventory inventory; // drag your Player's R_Inventory

    void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<R_Inventory>();
        if (!inventory) inventory = FindFirstObjectByType<R_Inventory>();
        SaveManager.Instance.Register(this);
    }

    void OnDestroy() => SaveManager.Instance?.Unregister(this);

    public string SaveId => "Inventory.Main";

    public string CaptureJson()
    {
        var list = new List<InvItemDTO>();

        if (inventory != null && inventory.items != null)
        {
            foreach (var slot in inventory.items)
            {
                if (slot == null || slot.itemData == null || slot.quantity <= 0) continue;

                var data = slot.itemData;

                if (data.isStackable)
                {
                    // one stacked record
                    list.Add(new InvItemDTO
                    {
                        uid         = null,
                        count       = slot.quantity,
                        soType      = data.GetType().AssemblyQualifiedName,
                        soJson      = JsonUtility.ToJson(data, false),
                        displayName = data.itemName
                    });
                }
                else
                {
                    // one record per instance
                    if (string.IsNullOrEmpty(slot.runtimeID))
                        slot.runtimeID = Guid.NewGuid().ToString();

                    for (int i = 0; i < slot.quantity; i++)
                    {
                        list.Add(new InvItemDTO
                        {
                            uid         = (i == 0) ? slot.runtimeID : Guid.NewGuid().ToString(),
                            count       = 1,
                            soType      = data.GetType().AssemblyQualifiedName,
                            soJson      = JsonUtility.ToJson(data, false),
                            displayName = data.itemName
                        });
                    }
                }
            }
        }

        var dto = new InventoryDTO { items = list.ToArray() };
        return JsonUtility.ToJson(dto, true);
    }

    public void RestoreFromJson(string json)
    {
        if (string.IsNullOrEmpty(json) || inventory == null) return;

        var dto = JsonUtility.FromJson<InventoryDTO>(json);
        if (dto?.items == null) return;

        inventory.items.Clear();

        // Track UIDs we’ve assigned to avoid duplicating them
        var assigned = new HashSet<string>();

        foreach (var it in dto.items)
        {
            // Rebuild a ScriptableObject instance from the saved type + JSON
            var itemData = RebuildItemData(it.soType, it.soJson);
            if (itemData == null)
            {
                Debug.LogWarning($"[InventorySaver] Rebuild failed: {it.displayName} ({it.soType})");
                continue;
            }

            if (itemData.isStackable)
            {
                inventory.AddItem(itemData, Mathf.Max(1, it.count));
            }
            else
            {
                // Non-stackable: add 1-by-1, then overwrite the runtimeID with the saved UID
                int count = Mathf.Max(1, it.count);
                for (int i = 0; i < count; i++)
                {
                    // Create a fresh instance per non-stackable item to avoid shared state
                    var perItemData = (count == 1) ? itemData : RebuildItemData(it.soType, it.soJson);
                    inventory.AddItem(perItemData, 1);

                    var slot = FindNextUnassignedInstance(inventory, perItemData, assigned);
                    if (slot != null)
                    {
                        var uidToUse = (i == 0 && !string.IsNullOrEmpty(it.uid))
                                       ? it.uid
                                       : Guid.NewGuid().ToString();

                        slot.runtimeID = uidToUse; // overwrite whatever constructor set
                        assigned.Add(uidToUse);
                    }
                }
            }
        }

        // If your UI doesn't auto-refresh on AddItem, trigger it here.
        // e.g., R_InventoryUI.Instance?.Refresh();
    }

    static R_ItemData RebuildItemData(string typeName, string dataJson)
    {
        if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(dataJson)) return null;

        var type = Type.GetType(typeName);
        if (type == null)
        {
            Debug.LogWarning($"[InventorySaver] Unknown type: {typeName}");
            return null;
        }

        var instance = ScriptableObject.CreateInstance(type) as R_ItemData;
        if (!instance) return null;

        JsonUtility.FromJsonOverwrite(dataJson, instance);
        return instance;
    }

    static R_InventoryItem FindNextUnassignedInstance(R_Inventory inv, R_ItemData data, HashSet<string> assigned)
    {
        if (inv?.items == null) return null;

        foreach (var s in inv.items)
        {
            if (s == null || s.itemData != data) continue;

            // Either no UID yet, or a UID we haven't set during this restore pass
            if (string.IsNullOrEmpty(s.runtimeID) || !assigned.Contains(s.runtimeID))
                return s;
        }
        return null;
    }
}
